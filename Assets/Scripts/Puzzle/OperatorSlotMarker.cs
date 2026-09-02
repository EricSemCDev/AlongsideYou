using TMPro;
using UnityEngine;

// Tocha da Estação de Cálculo, onde o Finn acende um operador coletado.
[RequireComponent(typeof(SpriteRenderer))]
public class OperatorSlotMarker : MonoBehaviour
{
    [SerializeField] private Color litColor = Color.yellow;
    [SerializeField] private TextMeshPro symbolLabel;

    private SpriteRenderer _spriteRenderer;
    private Color _unlitColor;

    public bool IsLit { get; private set; }
    public char CurrentSymbol { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unlitColor = _spriteRenderer.color;
    }

    // Acende com o símbolo dado. Se já estiver aceso com outro símbolo,
    // quem chama é responsável por devolver o antigo antes (a troca é
    // decidida pelo menu, não pela tocha).
    public void Light(char symbol)
    {
        IsLit = true;
        CurrentSymbol = symbol;

        _spriteRenderer.color = litColor;

        if (symbolLabel != null)
        {
            symbolLabel.text = OperatorSymbolDisplay.ToDisplayChar(symbol).ToString();
        }
    }

    public void Extinguish()
    {
        IsLit = false;

        _spriteRenderer.color = _unlitColor;

        if (symbolLabel != null)
        {
            symbolLabel.text = string.Empty;
        }
    }
}