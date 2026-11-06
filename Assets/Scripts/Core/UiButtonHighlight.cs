using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Destaque suave para botoes de UI, sem Animator: muda a cor do texto e faz uma
// camada de brilho (glow) aparecer e sumir devagar. Funciona com mouse e com
// gamepad/teclado (quando o botao fica selecionado), e usa tempo nao escalado,
// entao funciona com o jogo pausado.
//
// Como montar: no botao, adicione este componente e arraste:
//  - Label: o texto (TextMeshPro) filho do botao;
//  - Glow: uma Image filha do botao, esticada, com a moldura de destaque,
//    cor com alpha 0 e Raycast Target desmarcado (opcional).
[RequireComponent(typeof(Selectable))]
public class UiButtonHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text label;
    [Tooltip("Camada de brilho. Opcional.")]
    [SerializeField] private Graphic glow;

    [Header("Texto")]
    [SerializeField] private Color normalColor = new Color(1f, 0.93f, 0.8f, 1f);
    [SerializeField] private Color highlightedColor = new Color(1f, 0.8f, 0.27f, 1f);
    [SerializeField] private Color pressedColor = new Color(0.92f, 0.38f, 0.08f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.45f, 0.4f, 1f);
    [Tooltip("Quanto o texto sobe no destaque (em unidades do canvas). 1 = 1 pixel.")]
    [SerializeField] private Vector2 highlightedLabelOffset = new Vector2(0f, 1f);

    [Header("Brilho")]
    [Range(0f, 1f)] [SerializeField] private float glowAlpha = 1f;

    [Header("Transicao")]
    [Tooltip("Maior = mais rapido. Em torno de 12 a 20 fica suave sem parecer lento.")]
    [SerializeField] private float speed = 14f;
    [Tooltip("Ao passar o mouse, seleciona o botao (assim so um botao fica em destaque por vez).")]
    [SerializeField] private bool selectOnHover = true;

    private Selectable _selectable;
    private Vector2 _labelBasePosition;
    private bool _hovered;
    private bool _selected;
    private bool _pressed;

    // Valores atuais (suavizados) e alvos.
    private Color _currentColor;
    private float _currentGlow;
    private Vector2 _currentOffset;

    private void Awake()
    {
        _selectable = GetComponent<Selectable>();

        if (label != null)
        {
            _labelBasePosition = label.rectTransform.anchoredPosition;
        }
    }

    private void OnEnable()
    {
        // Sempre comeca no estado normal (evita destaque "preso" ao reabrir o menu).
        _hovered = false;
        _selected = false;
        _pressed = false;
        _currentColor = normalColor;
        _currentGlow = 0f;
        _currentOffset = Vector2.zero;
        Apply();
    }

    private void Update()
    {
        Color targetColor;
        float targetGlow = 0f;
        Vector2 targetOffset = Vector2.zero;

        if (!_selectable.interactable)
        {
            targetColor = disabledColor;
        }
        else if (_pressed)
        {
            targetColor = pressedColor;
            targetGlow = glowAlpha;
            targetOffset = highlightedLabelOffset;
        }
        else if (_hovered || _selected)
        {
            targetColor = highlightedColor;
            targetGlow = glowAlpha;
            targetOffset = highlightedLabelOffset;
        }
        else
        {
            targetColor = normalColor;
        }

        float t = 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime);
        _currentColor = Color.Lerp(_currentColor, targetColor, t);
        _currentGlow = Mathf.Lerp(_currentGlow, targetGlow, t);
        _currentOffset = Vector2.Lerp(_currentOffset, targetOffset, t);

        Apply();
    }

    private void Apply()
    {
        if (label != null)
        {
            label.color = _currentColor;
            label.rectTransform.anchoredPosition = _labelBasePosition + _currentOffset;
        }

        if (glow != null)
        {
            Color c = glow.color;
            c.a = _currentGlow;
            glow.color = c;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;

        if (selectOnHover && _selectable.interactable && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        _pressed = false;

        if (selectOnHover && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        _selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _selected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }
}
