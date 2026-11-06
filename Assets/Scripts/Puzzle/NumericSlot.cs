using UnityEngine;

// Local de encaixe numérico na Estação de Cálculo. Quando isComposite,
// aceita 2 blocos (dezena + unidade), formando um valor de 2 dígitos.
// Visual ainda é placeholder: os 2 blocos ficam ancorados na mesma
// posição por enquanto — melhorar quando a arte estiver pronta.
[RequireComponent(typeof(SpriteRenderer))]
public class NumericSlot : MonoBehaviour
{
    [SerializeField] private bool isComposite;
    [SerializeField] private int position; // posição na sequência da equação (1º, 2º, 3º...)
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Block Position")]
    [Tooltip("Deslocamento (local) do bloco encaixado. Em slot composto, vale para a dezena.")]
    [SerializeField] private Vector2 blockOffset;
    [Tooltip("Deslocamento (local) da unidade, so em slot composto.")]
    [SerializeField] private Vector2 unitsBlockOffset;

    [Header("Slot composto (2 blocos)")]
    [Tooltip("Animacao do slot composto. Quando Is Composite esta ligado, este controller substitui o do slot simples (a idle do composto no lugar da idle normal).")]
    [SerializeField] private RuntimeAnimatorController compositeAnimatorController;
    [Tooltip("OPCIONAL, so se voce quiser um desenho estatico em vez de animacao: substitui o desenho do slot e desliga o Animator. Para usar animacao, deixe vazio.")]
    [SerializeField] private Sprite compositeEmptySprite;
    [Tooltip("Opcional: desenho quando os 2 blocos estao encaixados.")]
    [SerializeField] private Sprite compositeFilledSprite;
    [Tooltip("Onde fica o bloco quando o slot NAO e composto (1 bloco so). Crie um filho vazio e mova na Scene. Se vazio, usa o centro do slot + Block Offset.")]
    [SerializeField] private Transform singleAnchor;
    [Tooltip("Opcional: onde fica o bloco da DEZENA. Crie um filho vazio e mova na Scene. Se vazio, usa o centro do slot + Block Offset.")]
    [SerializeField] private Transform tensAnchor;
    [Tooltip("Opcional: onde fica o bloco da UNIDADE. Crie um filho vazio e mova na Scene. Se vazio, usa o centro do slot + Units Block Offset.")]
    [SerializeField] private Transform unitsAnchor;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [Tooltip("Parametro Bool do Animator ligado quando o slot esta preenchido.")]
    [SerializeField] private string filledParameter = "Preenchido";

    private DadoBlock _tensBlock;  // único bloco, se não for composto
    private DadoBlock _unitsBlock; // só usado se isComposite

    private static readonly int OutlineEnabledId = Shader.PropertyToID("_OutlineEnabled");
    private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");

    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private bool _supportsOutline;
    private int _filledParameterId;
    private Color _originalColor;
    private bool _usesCompositeSprites;
    private bool _hasFilledParameter;

    public bool IsComposite => isComposite;
    public int Position => position;

    public bool IsFilled => isComposite
        ? (_tensBlock != null && _unitsBlock != null)
        : (_tensBlock != null);

    public int CurrentValue
    {
        get
        {
            if (!isComposite)
            {
                return _tensBlock != null ? _tensBlock.Value : 0;
            }

            if (_tensBlock == null || _unitsBlock == null)
            {
                return 0; // incompleto
            }

            return (_tensBlock.Value * 10) + _unitsBlock.Value;
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;

        // Material de contorno: o destaque vira contorno. Senao, mantem a tintura antiga.
        _propertyBlock = new MaterialPropertyBlock();
        Material material = _spriteRenderer.sharedMaterial;
        _supportsOutline = material != null && material.HasProperty(OutlineEnabledId);

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Slot composto: troca a animacao (idle do simples -> idle do composto).
        if (isComposite && compositeAnimatorController != null && animator != null)
        {
            animator.runtimeAnimatorController = compositeAnimatorController;
        }

        _filledParameterId = Animator.StringToHash(filledParameter);
        _hasFilledParameter = HasFilledParameter();
        _usesCompositeSprites = isComposite && compositeEmptySprite != null;

        if (_usesCompositeSprites && animator != null)
        {
            // O desenho do slot composto e trocado por codigo; o Animator do slot
            // simples nao pode sobrescrever.
            animator.enabled = false;
        }

        UpdateFilledState();
    }

    // No Editor, mostra o desenho do slot composto para voce posicionar os
    // pontos da dezena e da unidade olhando a Scene.
    private void OnValidate()
    {
        if (Application.isPlaying || !isComposite || compositeEmptySprite == null)
        {
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = compositeEmptySprite;
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        if (_supportsOutline)
        {
            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(OutlineEnabledId, highlighted ? 1f : 0f);
            _propertyBlock.SetColor(OutlineColorId, highlightColor);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
            return;
        }

        _spriteRenderer.color = highlighted ? highlightColor : _originalColor;
    }

    public bool TryInsertBlock(DadoBlock block)
    {
        if (!isComposite)
        {
            if (_tensBlock != null)
            {
                return false;
            }

            _tensBlock = block;
            block.PlaceInSlot(singleAnchor != null ? singleAnchor : transform, this, blockOffset);
            UpdateFilledState();
            return true;
        }

        if (_tensBlock == null)
        {
            _tensBlock = block;
            block.PlaceInSlot(tensAnchor != null ? tensAnchor : transform, this, blockOffset);
            UpdateFilledState();
            return true;
        }

        if (_unitsBlock == null)
        {
            _unitsBlock = block;
            block.PlaceInSlot(unitsAnchor != null ? unitsAnchor : transform, this, unitsBlockOffset);
            UpdateFilledState();
            return true;
        }

        return false; // os dois já preenchidos
    }

    // Chamado pelo próprio DadoBlock quando o Al pega ele de volta.
    // Precisa saber QUAL bloco (dezena ou unidade) foi retirado.
    public void NotifyBlockRemoved(DadoBlock block)
    {
        if (_tensBlock == block)
        {
            _tensBlock = null;
        }
        else if (_unitsBlock == block)
        {
            _unitsBlock = null;
        }

        UpdateFilledState();
    }

    // Avisa o Animator se o slot esta preenchido (em slot composto, so quando
    // os dois blocos estao encaixados).
    private void UpdateFilledState()
    {
        if (_usesCompositeSprites)
        {
            _spriteRenderer.sprite = IsFilled && compositeFilledSprite != null
                ? compositeFilledSprite
                : compositeEmptySprite;
            return;
        }

        if (animator == null || !_hasFilledParameter)
        {
            return;
        }

        animator.SetBool(_filledParameterId, IsFilled);
    }

    // O controller do slot composto nao precisa ter o parametro Bool. Sem ele,
    // simplesmente nao liga (evita aviso a cada encaixe).
    private bool HasFilledParameter()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == _filledParameterId && parameter.type == AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }
}