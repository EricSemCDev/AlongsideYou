using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Caixa de dialogo estilo RPG: retrato a esquerda, nome e texto que aparece
// letra por letra. Monta o Canvas inteiro por codigo (nao precisa de prefab) e
// usa tempo nao escalado, entao funciona com o jogo congelado.
// Fica em um objeto da fase; o TutorialSequencePlayer usa esta caixa.
public class TutorialDialogueUI : MonoBehaviour
{
    [Header("Texto")]
    [Tooltip("Fonte pixelada. Vazio = fonte padrao do TextMeshPro do projeto.")]
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private float textSize = 8f;
    [SerializeField] private float nameSize = 8f;
    [Tooltip("Espaco extra entre as linhas do texto. Maior = linhas mais afastadas. Em Play Mode da para ajustar vendo o resultado.")]
    [SerializeField] private float lineSpacing = 24f;
    [SerializeField] private Color textColor = new Color(1f, 0.95f, 0.85f, 1f);
    [SerializeField] private Color nameColor = new Color(1f, 0.72f, 0.3f, 1f);

    [Header("Icones de botao")]
    [Tooltip("Tamanho dos icones dentro do texto (100 = natural). Aumente se ficarem pequenos.")]
    [SerializeField] private int iconSizePercent = 100;

    [Header("Digitando")]
    [Tooltip("Letras por segundo.")]
    [SerializeField] private float charactersPerSecond = 38f;
    [Tooltip("Pausa extra apos , . ! ? (segundos).")]
    [SerializeField] private float punctuationPause = 0.14f;
    [Tooltip("Som opcional tocado a cada poucas letras.")]
    [SerializeField] private AudioClip typeSound;
    [Range(0f, 1f)] [SerializeField] private float typeSoundVolume = 0.35f;

    [Header("Caixa (em unidades do canvas 480x270)")]
    [SerializeField] private float portraitSize = 64f;
    [SerializeField] private float sideMargin = 16f;
    [SerializeField] private float bottomMargin = 12f;
    [SerializeField] private Color fillColor = new Color(0.09f, 0.05f, 0.16f, 0.94f);
    [SerializeField] private Color borderColor = new Color(0.95f, 0.62f, 0.25f, 1f);
    [SerializeField] private int canvasSortingOrder = 80;

    [Header("Animacao")]
    [SerializeField] private float openDuration = 0.35f;

    private Canvas _canvas;
    private CanvasGroup _group;
    private RectTransform _panel;
    private Image _portrait;
    private GameObject _portraitFrame;
    private TMP_Text _nameLabel;
    private TMP_Text _textLabel;
    private RectTransform _indicator;
    private AudioSource _audio;
    private Texture2D _indicatorTexture;
    private Sprite _indicatorSprite;
    private Coroutine _typing;
    private bool _open;
    private float _panelHeight;
    private string _rawText = "";
    private string _rawGamepadText = "";

    public bool IsOpen => _open;
    public bool IsTyping => _typing != null;

    private void Awake()
    {
        Build();
        _canvas.enabled = false;
    }

    // Ajustar o espacamento no Inspector durante o Play Mode vale na hora.
    private void OnValidate()
    {
        if (_textLabel != null)
        {
            _textLabel.lineSpacing = lineSpacing;
        }
    }

    private void OnEnable()
    {
        InputPromptResolver.DeviceChanged += HandleDeviceChanged;
    }

    private void OnDisable()
    {
        InputPromptResolver.DeviceChanged -= HandleDeviceChanged;
    }

    // O jogador trocou de teclado para controle (ou vice-versa) no meio da fala:
    // mostra a fala inteira ja com os icones novos.
    private void HandleDeviceChanged()
    {
        if (!_open || string.IsNullOrEmpty(_rawText) || _textLabel == null)
        {
            return;
        }

        CompleteNow();
        _textLabel.text = Compose();
        _textLabel.maxVisibleCharacters = int.MaxValue;
    }

    private string Compose()
    {
        string raw = InputPromptResolver.IsGamepad && !string.IsNullOrEmpty(_rawGamepadText) ? _rawGamepadText : _rawText;
        return InputPromptResolver.Resolve(raw, true, iconSizePercent);
    }

    private void OnDestroy()
    {
        if (_indicatorSprite != null) Destroy(_indicatorSprite);
        if (_indicatorTexture != null) Destroy(_indicatorTexture);
    }

    private void Update()
    {
        // Some enquanto o menu de pausa esta aberto, para nao ficar por cima dele.
        if (_open)
        {
            bool paused = GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused;
            _canvas.enabled = !paused;
        }

        if (_open && _indicator != null && _indicator.gameObject.activeSelf)
        {
            // Setinha "pulando" 1 pixel, em passos (pixel art).
            float step = Mathf.Floor(Time.unscaledTime * 3f) % 2f;
            _indicator.anchoredPosition = new Vector2(-8f, 6f + step);
        }
    }

    // --- API para o TutorialSequencePlayer --------------------------------

    public IEnumerator Open()
    {
        _open = true;
        _canvas.enabled = true;
        SetContinueVisible(false);
        _textLabel.text = "";

        float hiddenY = -(_panelHeight + 8f);
        float shownY = bottomMargin;
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            SetPanelY(Mathf.Lerp(hiddenY, shownY, eased));
            _group.alpha = Mathf.Clamp01(t * 2f);
            yield return null;
        }

        SetPanelY(shownY);
        _group.alpha = 1f;
    }

    public IEnumerator Close()
    {
        SetContinueVisible(false);

        float hiddenY = -(_panelHeight + 8f);
        float shownY = bottomMargin;
        float elapsed = 0f;
        float duration = openDuration * 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = t * t;

            SetPanelY(Mathf.Lerp(shownY, hiddenY, eased));
            _group.alpha = 1f - Mathf.Clamp01((t - 0.4f) / 0.6f);
            yield return null;
        }

        _open = false;
        _canvas.enabled = false;
    }

    public void SetSpeaker(string speakerName, Sprite portrait)
    {
        _nameLabel.text = speakerName ?? "";

        bool hasPortrait = portrait != null;
        _portraitFrame.SetActive(hasPortrait);
        _portrait.sprite = portrait;
    }

    // Comeca a digitar a fala. Acompanhe por IsTyping.
    // text / gamepadText aceitam marcas de botao ({interact} etc.). gamepadText e
    // opcional e so e usado quando o jogador esta com o controle.
    public void BeginLine(string text, string gamepadText = null)
    {
        _rawText = text ?? "";
        _rawGamepadText = gamepadText ?? "";

        if (_typing != null)
        {
            StopCoroutine(_typing);
        }

        SetContinueVisible(false);
        _typing = StartCoroutine(TypeRoutine(Compose()));
    }

    // Mostra a fala inteira de uma vez (jogador apertou durante a digitacao).
    public void CompleteNow()
    {
        if (_typing == null)
        {
            return;
        }

        StopCoroutine(_typing);
        _typing = null;
        _textLabel.maxVisibleCharacters = int.MaxValue;
    }

    public void SetContinueVisible(bool visible)
    {
        if (_indicator != null)
        {
            _indicator.gameObject.SetActive(visible);
        }
    }

    // --- Internos ----------------------------------------------------------

    private IEnumerator TypeRoutine(string text)
    {
        _textLabel.text = text;
        _textLabel.maxVisibleCharacters = 0;
        _textLabel.ForceMeshUpdate();

        int total = _textLabel.textInfo.characterCount;
        float visible = 0f;
        int lastShown = 0;
        float pause = 0f;

        while (lastShown < total)
        {
            if (pause > 0f)
            {
                pause -= Time.unscaledDeltaTime;
            }
            else
            {
                visible += charactersPerSecond * Time.unscaledDeltaTime;
                int shown = Mathf.Min(total, Mathf.FloorToInt(visible));

                if (shown > lastShown)
                {
                    char last = _textLabel.textInfo.characterInfo[shown - 1].character;

                    if (last == ',' || last == '.' || last == '!' || last == '?')
                    {
                        pause = punctuationPause;
                    }

                    if (typeSound != null && shown % 2 == 0 && !char.IsWhiteSpace(last))
                    {
                        _audio.PlayOneShot(typeSound, typeSoundVolume);
                    }

                    lastShown = shown;
                    _textLabel.maxVisibleCharacters = shown;
                }
            }

            yield return null;
        }

        _textLabel.maxVisibleCharacters = int.MaxValue;
        _typing = null;
    }

    private void SetPanelY(float y)
    {
        _panel.anchoredPosition = new Vector2(0f, Mathf.Round(y));
    }

    private void Build()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;

        // Canvas (480x270, igual aos outros do jogo).
        var canvasObject = new GameObject("TutorialDialogueCanvas");
        canvasObject.transform.SetParent(transform, false);

        _canvas = canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = canvasSortingOrder;

        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(480f, 270f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Sem GraphicRaycaster: a caixa nunca rouba cliques.
        _group = canvasObject.AddComponent<CanvasGroup>();
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;

        TMP_FontAsset usedFont = font != null ? font : TMP_Settings.defaultFontAsset;

        // Painel: borda, linha interna suave e fundo.
        _panelHeight = portraitSize + 16f;
        _panel = NewRect("Panel", canvasObject.transform);
        _panel.anchorMin = new Vector2(0f, 0f);
        _panel.anchorMax = new Vector2(1f, 0f);
        _panel.pivot = new Vector2(0.5f, 0f);
        _panel.offsetMin = new Vector2(sideMargin, 0f);
        _panel.offsetMax = new Vector2(-sideMargin, _panelHeight);
        SetPanelY(-(_panelHeight + 8f));

        AddImage(_panel, "Border", borderColor, 0f);
        AddImage(_panel, "Fill", fillColor, 2f);
        AddImage(_panel, "InnerLine", new Color(borderColor.r, borderColor.g, borderColor.b, 0.25f), 3f);
        AddImage(_panel, "InnerFill", fillColor, 4f);

        // Retrato (quadro + imagem).
        float frame = portraitSize + 4f;
        RectTransform frameRect = NewRect("PortraitFrame", _panel);
        frameRect.anchorMin = frameRect.anchorMax = new Vector2(0f, 0.5f);
        frameRect.pivot = new Vector2(0f, 0.5f);
        frameRect.anchoredPosition = new Vector2(8f, 0f);
        frameRect.sizeDelta = new Vector2(frame, frame);
        _portraitFrame = frameRect.gameObject;

        AddImage(frameRect, "FrameBorder", borderColor, 0f);
        AddImage(frameRect, "FrameFill", new Color(0.16f, 0.1f, 0.26f, 1f), 1f);

        RectTransform portraitRect = NewRect("Portrait", frameRect);
        portraitRect.anchorMin = portraitRect.anchorMax = new Vector2(0.5f, 0.5f);
        portraitRect.sizeDelta = new Vector2(portraitSize, portraitSize);
        _portrait = portraitRect.gameObject.AddComponent<Image>();
        _portrait.preserveAspect = true;
        _portrait.raycastTarget = false;

        // Nome e texto, a direita do retrato.
        float textLeft = 8f + frame + 8f;

        _nameLabel = NewLabel("Name", _panel, usedFont, nameSize, nameColor);
        _nameLabel.rectTransform.anchorMin = new Vector2(0f, 1f);
        _nameLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
        _nameLabel.rectTransform.pivot = new Vector2(0f, 1f);
        _nameLabel.rectTransform.offsetMin = new Vector2(textLeft, -(nameSize + 9f));
        _nameLabel.rectTransform.offsetMax = new Vector2(-10f, -8f);

        _textLabel = NewLabel("Text", _panel, usedFont, textSize, textColor);
        _textLabel.rectTransform.anchorMin = new Vector2(0f, 0f);
        _textLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
        _textLabel.rectTransform.offsetMin = new Vector2(textLeft, 8f);
        _textLabel.rectTransform.offsetMax = new Vector2(-12f, -(nameSize + 12f));
        _textLabel.alignment = TextAlignmentOptions.TopLeft;
        _textLabel.lineSpacing = lineSpacing;

        // Setinha de "continuar" no canto de baixo.
        CreateIndicatorSprite();
        RectTransform indicatorRect = NewRect("Continue", _panel);
        indicatorRect.anchorMin = indicatorRect.anchorMax = new Vector2(1f, 0f);
        indicatorRect.pivot = new Vector2(1f, 0f);
        indicatorRect.sizeDelta = new Vector2(7f, 4f);
        indicatorRect.anchoredPosition = new Vector2(-8f, 6f);
        var indicatorImage = indicatorRect.gameObject.AddComponent<Image>();
        indicatorImage.sprite = _indicatorSprite;
        indicatorImage.color = borderColor;
        indicatorImage.raycastTarget = false;
        _indicator = indicatorRect;
        _indicator.gameObject.SetActive(false);

        // Sem retrato definido o quadro fica escondido (senao aparece um quadrado branco).
        _portraitFrame.SetActive(false);
    }

    private static RectTransform NewRect(string objectName, Transform parent)
    {
        var go = new GameObject(objectName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static void AddImage(RectTransform parent, string objectName, Color color, float inset)
    {
        RectTransform rect = NewRect(objectName, parent);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);

        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private static TMP_Text NewLabel(string objectName, RectTransform parent, TMP_FontAsset font, float size, Color color)
    {
        RectTransform rect = NewRect(objectName, parent);
        var label = rect.gameObject.AddComponent<TextMeshProUGUI>();

        if (font != null)
        {
            label.font = font;
        }

        label.fontSize = size;
        label.color = color;
        label.enableWordWrapping = true;
        label.richText = true;
        label.raycastTarget = false;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.overflowMode = TextOverflowModes.Overflow;
        return label;
    }

    private void CreateIndicatorSprite()
    {
        _indicatorTexture = new Texture2D(7, 4, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        // Triangulo apontando para baixo (linha 0 = embaixo, a mais estreita).
        var pixels = new Color32[7 * 4];

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                bool on = x >= 3 - y && x <= 3 + y;
                pixels[(y * 7) + x] = on ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
            }
        }

        _indicatorTexture.SetPixels32(pixels);
        _indicatorTexture.Apply();
        _indicatorSprite = Sprite.Create(_indicatorTexture, new Rect(0f, 0f, 7f, 4f), new Vector2(0.5f, 0.5f), 100f);
    }
}
