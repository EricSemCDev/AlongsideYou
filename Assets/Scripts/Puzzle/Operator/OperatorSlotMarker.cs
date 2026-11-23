using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OperatorSlotMarker : MonoBehaviour
{
    [SerializeField] private Color highlightColor = new Color(0.6f, 0.9f, 1f);

    [Header("Animator")]

    [SerializeField] private Animator animator;
    [SerializeField] private string operatorParameter = "Operator";
    [SerializeField] private Sprite emptyTorchSprite;

    [Header("Floating Symbol")]

    [SerializeField] private SpriteRenderer symbolRenderer;
    [SerializeField] private Sprite plusSprite;
    [SerializeField] private Sprite minusSprite;
    [SerializeField] private Sprite timesSprite;
    [SerializeField] private Sprite divideSprite;
    [SerializeField] private Vector2 symbolOffset;
    [SerializeField] private int position;

    public int Position => position;

    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private int _operatorParameterId;
    private bool _waitingForSymbol;
    private bool _leftReadyState;

    public bool IsLit { get; private set; }
    public char CurrentSymbol { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _spriteRenderer.color;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _operatorParameterId = Animator.StringToHash(operatorParameter);
        EnsureSymbolRenderer();
        HideSymbol();
    }

    public void SetHighlighted(bool highlighted)
    {
        _spriteRenderer.color = highlighted ? highlightColor : _baseColor;
    }

    public void Light(char symbol)
    {
        IsLit = true;
        CurrentSymbol = symbol;

        HideSymbol();
        SetOperatorParameter(symbol);
        _waitingForSymbol = animator != null;
        _leftReadyState = false;

        if (animator == null)
        {
            ShowSymbol();
        }

        GameJuice.TorchLit(transform.position);
    }

    public void Extinguish()
    {
        IsLit = false;

        HideSymbol();
        SetOperatorParameter('\0');
        _waitingForSymbol = false;
    }

    public void OnSymbolAnimationEnd()
    {
        if (IsLit)
        {
            _waitingForSymbol = false;
            ShowSymbol();
        }
    }

    private void SetOperatorParameter(char symbol)
    {
        if (animator == null)
        {
            return;
        }

        animator.enabled = true;
        animator.SetInteger(_operatorParameterId, SymbolToParameter(symbol));
    }

    private static int SymbolToParameter(char symbol)
    {
        switch (symbol)
        {
            case '+':
                return 1;
            case '-':
                return 2;
            case '*':
                return 3;
            case '/':
                return 4;
            default:
                return 0;
        }
    }

    private Sprite GetSymbolSprite(char symbol)
    {
        switch (symbol)
        {
            case '+':
                return plusSprite;
            case '-':
                return minusSprite;
            case '*':
                return timesSprite;
            case '/':
                return divideSprite;
            default:
                return null;
        }
    }

    private void EnsureSymbolRenderer()
    {
        if (symbolRenderer != null)
        {
            return;
        }

        var go = new GameObject("OperatorSymbol");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = symbolOffset;

        symbolRenderer = go.AddComponent<SpriteRenderer>();
        symbolRenderer.sharedMaterial = _spriteRenderer.sharedMaterial;

        go.AddComponent<FloatingBob>();
    }

    private void ShowSymbol()
    {
        if (symbolRenderer == null)
        {
            return;
        }

        symbolRenderer.sprite = GetSymbolSprite(CurrentSymbol);
        symbolRenderer.gameObject.SetActive(symbolRenderer.sprite != null);
        UpdateSymbolSorting();
    }

    private void HideSymbol()
    {
        if (symbolRenderer != null)
        {
            symbolRenderer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_waitingForSymbol)
        {
            CheckSymbolReady();
        }
    }

    private void CheckSymbolReady()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (animator.IsInTransition(0))
        {
            return;
        }

        bool playing = info.loop || info.normalizedTime < 1f;

        if (playing)
        {
            if (!info.loop)
            {
                _leftReadyState = true;
            }

            return;
        }

        if (_leftReadyState && IsLit)
        {
            _waitingForSymbol = false;
            FinishOperatorAnimation();
        }
    }

    private void FinishOperatorAnimation()
    {
        if (emptyTorchSprite != null)
        {
            animator.enabled = false;
            _spriteRenderer.sprite = emptyTorchSprite;
        }

        ShowSymbol();
    }

    private void LateUpdate()
    {
        if (symbolRenderer != null && symbolRenderer.gameObject.activeSelf)
        {
            UpdateSymbolSorting();
        }
    }

    private void UpdateSymbolSorting()
    {
        symbolRenderer.sortingLayerID = _spriteRenderer.sortingLayerID;
        symbolRenderer.sortingOrder = _spriteRenderer.sortingOrder + 1;
    }
}