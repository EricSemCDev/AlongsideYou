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

    private DadoBlock _tensBlock;  // único bloco, se não for composto
    private DadoBlock _unitsBlock; // só usado se isComposite

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

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
    }

    public void SetHighlighted(bool highlighted)
    {
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
            block.PlaceInSlot(transform, this);
            return true;
        }

        if (_tensBlock == null)
        {
            _tensBlock = block;
            block.PlaceInSlot(transform, this);
            return true;
        }

        if (_unitsBlock == null)
        {
            _unitsBlock = block;
            block.PlaceInSlot(transform, this);
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
    }
}