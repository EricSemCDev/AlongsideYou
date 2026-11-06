using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorMenuController : MonoBehaviour
{
    public static OperatorMenuController Instance { get; private set; }

    [Header("Pedaços (sentido horário desde cima-direita)")]
    [SerializeField] private RectTransform slicePlus;
    [SerializeField] private RectTransform sliceMinus;
    [SerializeField] private RectTransform sliceTimes;
    [SerializeField] private RectTransform sliceDivide;

    [Header("Rótulos de contagem")]
    [SerializeField] private TextMeshProUGUI countLabelPlus;
    [SerializeField] private TextMeshProUGUI countLabelMinus;
    [SerializeField] private TextMeshProUGUI countLabelTimes;
    [SerializeField] private TextMeshProUGUI countLabelDivide;

    [Header("Destaque visual")]
    [SerializeField] private float highlightScale = 1.15f;
    [SerializeField] private Color dimmedColor = new Color(1f, 1f, 1f, 0.35f);

    [Header("Visual da roda")]
    [Tooltip("Imagem da fatia em repouso.")]
    [SerializeField] private Sprite wedgeNormal;
    [Tooltip("Imagem da fatia apontada pelo mouse / analogico.")]
    [SerializeField] private Sprite wedgeHighlight;
    [Tooltip("Imagem da fatia de um operador que acabou (quantidade 0).")]
    [SerializeField] private Sprite wedgeEmpty;
    [SerializeField] private Color activeTextColor = new Color(1f, 0.93f, 0.81f, 1f);
    [Tooltip("Duracao da animacao de abertura (segundos).")]
    [SerializeField] private float popDuration = 0.12f;
    [Range(0.3f, 1f)] [SerializeField] private float popStartScale = 0.7f;

    [Header("Áudio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip errorSound;

    private Camera _mainCamera;
    private OperatorSlotMarker _targetSlot;
    private FinnOperatorCollector _collector;
    private char _hoveredOperator;
    private bool _gamepadMode;
    private Vector3 _baseScale = Vector3.one;
    private float _popElapsed;

    // Quanto o analogico precisa se afastar do centro para contar como escolha.
    private const float StickSelectThreshold = 0.5f;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _baseScale = transform.localScale;
        _mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    // openedByGamepad: a roda foi aberta pelo botao do controle, entao a escolha
    // vem do analogico esquerdo (e nao da posicao do mouse).
    public void Show(Vector3 worldPosition, OperatorSlotMarker targetSlot, FinnOperatorCollector collector, bool openedByGamepad = false)
    {
        _targetSlot = targetSlot;
        _collector = collector;
        _gamepadMode = openedByGamepad;
        _hoveredOperator = '\0'; // nenhum escolhido ate o analogico apontar
        _popElapsed = 0f;
        transform.localScale = _baseScale * popStartScale;

        transform.position = worldPosition;
        IsOpen = true;
        gameObject.SetActive(true);

        RefreshCounts();
    }

    private void Update()
    {
        if (!IsOpen)
        {
            return;
        }

        UpdatePop();
        UpdateHoveredOperator();
        UpdateSliceVisuals();

        if (InputManager.Instance != null && InputManager.Instance.FinnSelectOperatorReleased)
        {
            ConfirmSelection();
        }
    }

    // A roda "pula" ate o tamanho final ao abrir (tempo nao escalado).
    private void UpdatePop()
    {
        if (_popElapsed >= popDuration)
        {
            return;
        }

        _popElapsed += Time.unscaledDeltaTime;
        float t = popDuration <= 0f ? 1f : Mathf.Clamp01(_popElapsed / popDuration);
        float eased = 1f - Mathf.Pow(1f - t, 3f);
        transform.localScale = _baseScale * Mathf.Lerp(popStartScale, 1f, eased);
    }

    private void UpdateHoveredOperator()
    {
        if (_gamepadMode)
        {
            // A escolha fica "grudada": ao soltar o analogico (que volta ao centro
            // junto com o botao) o ultimo operador apontado continua valendo.
            Vector2 stick = InputManager.Instance.FinnStickInput;

            if (stick.magnitude >= StickSelectThreshold)
            {
                _hoveredOperator = OperatorMenuSelector.GetSelectedOperator(Vector2.zero, stick);
            }

            return;
        }

        Vector2 pointerScreenPosition = InputManager.Instance.FinnPointerScreenPosition;

        Vector3 pointerWorldPosition = _mainCamera.ScreenToWorldPoint(
            new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, _mainCamera.nearClipPlane)
        );

        _hoveredOperator = OperatorMenuSelector.GetSelectedOperator(transform.position, pointerWorldPosition);
    }

    private void UpdateSliceVisuals()
    {
        SetSliceVisual(slicePlus, '+');
        SetSliceVisual(sliceMinus, '-');
        SetSliceVisual(sliceTimes, '*');
        SetSliceVisual(sliceDivide, '/');
    }

    private void SetSliceVisual(RectTransform slice, char symbol)
    {
        if (slice == null)
        {
            return;
        }

        bool highlighted = _hoveredOperator == symbol;
        slice.localScale = highlighted ? Vector3.one * highlightScale : Vector3.one;

        var image = slice.GetComponent<Image>();

        if (image == null)
        {
            return;
        }

        bool empty = _collector != null && _collector.GetCount(symbol) <= 0;
        Sprite wanted = highlighted ? wedgeHighlight : (empty ? wedgeEmpty : wedgeNormal);

        if (wanted != null && image.sprite != wanted)
        {
            image.sprite = wanted;
        }
    }

    private void RefreshCounts()
    {
        SetCountLabel(countLabelPlus, '+');
        SetCountLabel(countLabelMinus, '-');
        SetCountLabel(countLabelTimes, '*');
        SetCountLabel(countLabelDivide, '/');
    }

    private void SetCountLabel(TextMeshProUGUI label, char symbol)
    {
        if (label == null)
        {
            return;
        }

        int count = _collector.GetCount(symbol);
        char displaySymbol = OperatorSymbolDisplay.ToDisplayChar(symbol);

        // O numero vira um pequeno expoente (ex: +2 com o 2 levantado), com o mesmo tamanho do simbolo.
        label.text = $"{displaySymbol}<voffset=0.5em>{count}</voffset>";
        label.color = count > 0 ? activeTextColor : dimmedColor;
    }

    private void ConfirmSelection()
    {
        char selected = _hoveredOperator;

        // Soltou sem apontar o analogico para nenhum operador: cancela.
        if (selected == '\0')
        {
            Hide();
            return;
        }

        if (_targetSlot.IsLit)
        {
            HandleSwap(selected);
        }
        else
        {
            HandleFirstInsert(selected);
        }

        Hide();
    }

    private void HandleSwap(char selected)
    {
        if (selected == _targetSlot.CurrentSymbol)
        {
            return; // escolheu o mesmo operador que já estava aceso; sem efeito
        }

        if (_collector.GetCount(selected) <= 0)
        {
            PlayErrorSound();
            return; // tocha mantém o operador antigo
        }

        _collector.Return(_targetSlot.CurrentSymbol);
        _collector.TryConsume(selected);
        _targetSlot.Light(selected);
    }

    private void HandleFirstInsert(char selected)
    {
        if (!_collector.TryConsume(selected))
        {
            PlayErrorSound();
            return; // tocha continua apagada
        }

        _targetSlot.Light(selected);
    }

    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    private void Hide()
    {
        IsOpen = false;
        _targetSlot = null;
        _collector = null;
        gameObject.SetActive(false);
    }
}