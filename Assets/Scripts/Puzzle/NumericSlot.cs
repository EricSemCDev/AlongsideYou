using UnityEngine;

// Local de encaixe numérico na Estação de Cálculo. Al insere um
// DadoBlock aqui (issue #19); o valor mostrado é o valor atual do
// próprio bloco, sem duplicar estado.
[RequireComponent(typeof(SpriteRenderer))]
public class NumericSlot : MonoBehaviour
{
    [SerializeField] private bool isComposite;
    [SerializeField] private Color highlightColor = Color.yellow;

    private DadoBlock _insertedBlock;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public bool IsComposite => isComposite;
    public bool IsFilled => _insertedBlock != null;
    public int CurrentValue => _insertedBlock != null ? _insertedBlock.Value : 0;

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
        if (IsFilled)
        {
            return false;
        }

        _insertedBlock = block;
        block.PlaceInSlot(transform, this);
        return true;
    }

    // Chamado pelo próprio DadoBlock quando o Al pega ele de volta,
    // avisando o slot que voltou a ficar vazio.
    public void NotifyBlockRemoved()
    {
        _insertedBlock = null;
    }
}