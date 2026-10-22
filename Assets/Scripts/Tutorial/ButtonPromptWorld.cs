using TMPro;
using UnityEngine;

// Aviso de botao flutuando em cima de um objeto: o ICONE do botao mais uma
// palavra curta (ex: [Espaco] pegar). O icone alterna entre contornado e
// preenchido, como se estivesse sendo apertado, e troca sozinho entre teclado
// e controle (Xbox / PlayStation / Switch) conforme o ultimo dispositivo usado.
//
// Exemplos (o texto padrao ja vem pronto para cada modo):
//  - No bloco, "Near Player Not Carrying": {interact} pegar.
//  - No bloco, "Block Is Held": {rotate} girar, enquanto o Al segura aquele bloco.
//  - No slot, "Near Player Carrying": {interact} encaixar, com o Al perto segurando um bloco.
//
// Precisa do Sprite Asset dos icones como "Default Sprite Asset" do TextMeshPro
// (Project Settings > TextMeshPro > Settings). Veja InputPromptResolver.
public class ButtonPromptWorld : MonoBehaviour
{
    public enum ShowWhen
    {
        NearPlayer,
        NearPlayerNotCarrying,
        NearPlayerCarrying,
        BlockIsHeld
    }

    [Header("Quando mostrar")]
    [SerializeField] private ShowWhen showWhen = ShowWhen.NearPlayerNotCarrying;
    [SerializeField] private float radius = 1.8f;
    [Tooltip("Usado em 'Block Is Held'. Vazio = procura no proprio objeto.")]
    [SerializeField] private DadoBlock block;

    [Header("Texto")]
    [Tooltip("Aceita marcas de botao: {interact} {rotate} {move} {finn_wheel}... Vazio = texto padrao do modo escolhido.")]
    [SerializeField] private string promptText = "";
    [Tooltip("Opcional: texto so para quem joga de controle. Vazio = usa o texto acima.")]
    [SerializeField] private string promptTextGamepad = "";
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private float fontSize = 2.2f;
    [Tooltip("Tamanho dos icones dentro do texto (100 = natural).")]
    [SerializeField] private int iconSizePercent = 100;
    [SerializeField] private Color textColor = new Color(1f, 0.93f, 0.7f, 1f);
    [SerializeField] private Color outlineColor = new Color(0.08f, 0.04f, 0.14f, 1f);

    [Header("Animacao do icone")]
    [Tooltip("Segundos entre alternar contornado / preenchido (efeito de apertar). 0 = parado.")]
    [SerializeField] private float pressInterval = 0.5f;

    [Header("Posicao e movimento")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);
    [Tooltip("Quanto o aviso sobe e desce (unidades). Suave, sem arredondar para pixel.")]
    [SerializeField] private float bobAmplitude = 0.1f;
    [SerializeField] private float bobSpeed = 2.4f;
    [SerializeField] private float fadeSpeed = 10f;

    [Header("Render")]
    [SerializeField] private string sortingLayerName = "Foreground";
    [SerializeField] private int sortingOrder = 100;

    private TextMeshPro _label;
    private AlBlockCarrier _carrier;
    private float _alpha;
    private float _nextFindTime;
    private bool _filledFrame;
    private string _shownText;

    private void Awake()
    {
        if (block == null)
        {
            block = GetComponentInParent<DadoBlock>();
        }

        var go = new GameObject("ButtonPrompt_" + name);
        _label = go.AddComponent<TextMeshPro>();

        if (font != null)
        {
            _label.font = font;
        }
        else if (TMP_Settings.defaultFontAsset != null)
        {
            _label.font = TMP_Settings.defaultFontAsset;
        }

        _label.fontSize = fontSize;
        _label.alignment = TextAlignmentOptions.Center;
        _label.enableWordWrapping = false;
        _label.richText = true;
        _label.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        _label.outlineWidth = 0.28f;
        _label.outlineColor = outlineColor;
        _label.rectTransform.sizeDelta = new Vector2(10f, 2f);

        var meshRenderer = go.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
        meshRenderer.sortingOrder = sortingOrder;

        go.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_label != null)
        {
            Destroy(_label.gameObject);
        }
    }

    private void Update()
    {
        bool shouldShow = ShouldShow();
        float target = shouldShow ? 1f : 0f;
        float t = 1f - Mathf.Exp(-fadeSpeed * Time.unscaledDeltaTime);
        _alpha = Mathf.Lerp(_alpha, target, t);

        bool visible = _alpha > 0.01f;

        if (_label.gameObject.activeSelf != visible)
        {
            _label.gameObject.SetActive(visible);
        }

        if (!visible)
        {
            return;
        }

        UpdateText();

        Color c = textColor;
        c.a = _alpha;
        _label.color = c;

        // Sobe e desce devagar, sem arredondar (movimento fluido).
        float bob = Mathf.Sin(Time.unscaledTime * bobSpeed) * bobAmplitude;
        _label.transform.position = transform.position + (Vector3)offset + new Vector3(0f, bob, 0f);
    }

    // So reescreve o texto quando algo mudou (icone alternou ou trocou de dispositivo).
    private void UpdateText()
    {
        bool filled = pressInterval > 0f && Mathf.FloorToInt(Time.unscaledTime / pressInterval) % 2 == 1;
        _filledFrame = filled;

        string raw = InputPromptResolver.IsGamepad && !string.IsNullOrEmpty(promptTextGamepad)
            ? promptTextGamepad
            : (string.IsNullOrEmpty(promptText) ? DefaultText() : promptText);

        string resolved = InputPromptResolver.Resolve(raw, _filledFrame, iconSizePercent);

        if (resolved != _shownText)
        {
            _shownText = resolved;
            _label.text = resolved;
        }
    }

    private string DefaultText()
    {
        switch (showWhen)
        {
            case ShowWhen.NearPlayerNotCarrying: return "{interact} pegar";
            case ShowWhen.NearPlayerCarrying: return "{interact} encaixar";
            case ShowWhen.BlockIsHeld: return "{rotate} girar";
            default: return "{interact}";
        }
    }

    private bool ShouldShow()
    {
        // Esconde enquanto o dialogo do tutorial esta aberto.
        if (TutorialSequencePlayer.IsAnyPlaying)
        {
            return false;
        }

        if (showWhen == ShowWhen.BlockIsHeld)
        {
            // So faz sentido avisar de girar se o bloco tem mais de uma face.
            return block != null && block.IsHeld && block.FaceCount > 1;
        }

        if (_carrier == null)
        {
            if (Time.unscaledTime >= _nextFindTime)
            {
                _nextFindTime = Time.unscaledTime + 1f;
                _carrier = FindFirstObjectByType<AlBlockCarrier>();
            }

            if (_carrier == null)
            {
                return false;
            }
        }

        float distance = Vector2.Distance(_carrier.transform.position, transform.position);

        if (distance > radius)
        {
            return false;
        }

        bool carrying = _carrier.HeldBlock != null;

        switch (showWhen)
        {
            case ShowWhen.NearPlayerNotCarrying:
                // O proprio bloco nao mostra "pegar" enquanto esta na mao ou num slot.
                return !carrying && (block == null || (!block.IsHeld && block.CurrentSlot == null));
            case ShowWhen.NearPlayerCarrying:
                return carrying;
            default:
                return true;
        }
    }
}
