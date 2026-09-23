using TMPro;
using UnityEngine;

// Tocha da Estação de Cálculo, onde o Finn acende um operador coletado.
[RequireComponent(typeof(SpriteRenderer))]
public class OperatorSlotMarker : MonoBehaviour
{
    [SerializeField] private Color litColor = Color.yellow;
    [SerializeField] private Color highlightColor = new Color(0.6f, 0.9f, 1f); // azul claro
    [SerializeField] private TextMeshPro symbolLabel;
    [SerializeField] private int position; // posição na sequência da equação (1º, 2º, 3º...)

    public int Position => position;

    private SpriteRenderer _spriteRenderer;
    private Color _unlitColor;
    private bool _isHighlighted;

    public bool IsLit { get; private set; }
    public char CurrentSymbol { get; private set; }

    private Color CurrentBaseColor => IsLit ? litColor : _unlitColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unlitColor = _spriteRenderer.color;
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

        _spriteRenderer.color = _isHighlighted ? highlightColor : CurrentBaseColor;

        if (symbolLabel != null)
        {
            symbolLabel.text = OperatorSymbolDisplay.ToDisplayChar(symbol).ToString();
        }
    }

    public void Extinguish()
    {
        IsLit = false;

        _spriteRenderer.color = _isHighlighted ? highlightColor : CurrentBaseColor;

        if (symbolLabel != null)
        {
            symbolLabel.text = string.Empty;
        }
    }
}