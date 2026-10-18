using TMPro;
using UnityEngine;

// Tocha da Estação de Cálculo, onde o Finn acende um operador coletado.
[RequireComponent(typeof(SpriteRenderer))]
public class OperatorSlotMarker : MonoBehaviour
{
    [SerializeField] private Color litColor = Color.yellow;
    [SerializeField] private Color highlightColor = new Color(0.6f, 0.9f, 1f); // azul claro
    [SerializeField] private TextMeshPro symbolLabel;

    [Header("Animator (optional)")]
    [SerializeField] private Animator animator;
    [Tooltip("Parametro Int do Animator: 0 = sem operador, 1 = +, 2 = -, 3 = x, 4 = divisao.")]
    [SerializeField] private string operatorParameter = "Operator";
    [Tooltip("Sprite da tocha vazia. Quando a animacao do operador termina, o Animator e desligado e este sprite fica no lugar, para so o simbolo flutuante aparecer. Nao precisa de estado nem clipe extra.")]
    [SerializeField] private Sprite emptyTorchSprite;
    [Tooltip("Opcional. Se preenchido, o simbolo aparece quando o Animator chega a este estado (em vez de detectar o fim da animacao).")]
    [SerializeField] private string symbolReadyStateName = "";

    [Header("Floating Symbol (optional)")]
    [Tooltip("SpriteRenderer do simbolo flutuante. Se vazio, e criado por codigo como filho.")]
    [SerializeField] private SpriteRenderer symbolRenderer;
    [SerializeField] private Sprite plusSprite;
    [SerializeField] private Sprite minusSprite;
    [SerializeField] private Sprite timesSprite;
    [SerializeField] private Sprite divideSprite;
    [Tooltip("Posicao (local) do simbolo em relacao a tocha.")]
    [SerializeField] private Vector2 symbolOffset;
    [SerializeField] private int position; // posição na sequência da equação (1º, 2º, 3º...)

    public int Position => position;

    private SpriteRenderer _spriteRenderer;
    private Color _unlitColor;
    private bool _isHighlighted;
    private int _operatorParameterId;
    private bool _waitingForSymbol;
    private bool _leftReadyState;

    public bool IsLit { get; private set; }
    public char CurrentSymbol { get; private set; }

    private Color CurrentBaseColor => IsLit ? litColor : _unlitColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unlitColor = _spriteRenderer.color;

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
        _isHighlighted = highlighted;
        _spriteRenderer.color = highlighted ? highlightColor : CurrentBaseColor;
    }

    // Acende com o símbolo dado. Se já estiver aceso com outro símbolo,
    // quem chama é responsável por devolver o antigo antes (a troca é
    // decidida pelo menu, não pela tocha).
    public void Light(char symbol)
    {
        IsLit = true;
        CurrentSymbol = symbol;

        // O simbolo flutuante some enquanto a animacao do operador toca e e criado
        // no fim dela (Animation Event OnSymbolAnimationEnd). Sem Animator, aparece direto.
        HideSymbol();
        SetOperatorParameter(symbol);
        _waitingForSymbol = animator != null;
        _leftReadyState = false;

        if (animator == null)
        {
            ShowSymbol();
        }

        _spriteRenderer.color = _isHighlighted ? highlightColor : CurrentBaseColor;

        if (symbolLabel != null)
        {
            symbolLabel.text = OperatorSymbolDisplay.ToDisplayChar(symbol).ToString();
        }
    }

    public void Extinguish()
    {
        IsLit = false;

        HideSymbol();
        SetOperatorParameter('\0'); // tambem religa o Animator
        _waitingForSymbol = false;

        _spriteRenderer.color = _isHighlighted ? highlightColor : CurrentBaseColor;

        if (symbolLabel != null)
        {
            symbolLabel.text = string.Empty;
        }
    }

    // Animation Event: coloque no fim de cada clipe de operador (Mais, Menos,
    // Vezes, Divido). Mostra o simbolo flutuante logo apos a animacao.
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

        // Pode ter sido desligado ao fim da animacao anterior.
        animator.enabled = true;
        animator.SetInteger(_operatorParameterId, SymbolToParameter(symbol));
    }

    // Simbolo interno do jogo -> valor do parametro Int do Animator.
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

        // Depois de posicionar: o FloatingBob guarda a posicao atual como base.
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

    // Espera o Animator sair do estado final antigo (se estava nele), passar
    // pela animacao do operador e chegar de novo ao estado final: so entao mostra.
    private void CheckSymbolReady()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        // Modo por nome de estado (opcional).
        if (!string.IsNullOrEmpty(symbolReadyStateName))
        {
            if (!info.IsName(symbolReadyStateName))
            {
                _leftReadyState = true;
                return;
            }

            if (_leftReadyState && IsLit)
            {
                _waitingForSymbol = false;
                ShowSymbol();
            }

            return;
        }

        // Modo padrao: o clipe do operador nao tem loop, entao termina em
        // normalizedTime >= 1. Antes, precisa ter visto uma animacao tocando
        // (evita confundir com o clipe antigo, numa troca de operador).
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

    // Fim da animacao: deixa a tocha vazia (sprite fixo) e mostra o simbolo.
    private void FinishOperatorAnimation()
    {
        if (emptyTorchSprite != null)
        {
            animator.enabled = false;
            _spriteRenderer.sprite = emptyTorchSprite;
        }

        ShowSymbol();
    }

    // Fica logo acima da tocha na ordem de desenho (acompanha o Y-sorting).
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