using TMPro;
using UnityEngine;

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

    [Header("Áudio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip errorSound;

    private Camera _mainCamera;
    private OperatorSlotMarker _targetSlot;
    private FinnOperatorCollector _collector;
    private char _hoveredOperator;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    public void Show(Vector3 worldPosition, OperatorSlotMarker targetSlot, FinnOperatorCollector collector)
    {
        _targetSlot = targetSlot;
        _collector = collector;

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

        UpdateHoveredOperator();
        UpdateSliceVisuals();

        if (InputManager.Instance != null && InputManager.Instance.FinnSelectOperatorReleased)
        {
            ConfirmSelection();
        }
    }

    private void UpdateHoveredOperator()
    {
        Vector2 pointerScreenPosition = InputManager.Instance.FinnPointerScreenPosition;

        Vector3 pointerWorldPosition = _mainCamera.ScreenToWorldPoint(
            new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, _mainCamera.nearClipPlane)
        );

        _hoveredOperator = OperatorMenuSelector.GetSelectedOperator(transform.position, pointerWorldPosition);
    }

    private void UpdateSliceVisuals()
    {
        SetSliceHighlighted(slicePlus, _hoveredOperator == '+');
        SetSliceHighlighted(sliceMinus, _hoveredOperator == '-');
        SetSliceHighlighted(sliceTimes, _hoveredOperator == '*');
        SetSliceHighlighted(sliceDivide, _hoveredOperator == '/');
    }

    private void SetSliceHighlighted(RectTransform slice, bool highlighted)
    {
        if (slice == null)
        {
            return;
        }

        slice.localScale = highlighted ? Vector3.one * highlightScale : Vector3.one;
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

        label.text = $"{displaySymbol}{count}";
        label.color = count > 0 ? Color.white : dimmedColor;
    }

    private void ConfirmSelection()
    {
        char selected = _hoveredOperator;

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