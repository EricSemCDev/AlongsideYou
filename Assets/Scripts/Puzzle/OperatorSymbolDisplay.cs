using UnityEngine;

// Traduz o símbolo interno (fácil de digitar: +, -, *, /) para o
// símbolo bonito exibido ao jogador (+, -, ×, ÷). Mantém a lógica
// interna livre de caracteres Unicode difíceis de digitar/comparar.
public static class OperatorSymbolDisplay
{
    public static char ToDisplayChar(char canonicalSymbol)
    {
        switch (canonicalSymbol)
        {
            case '*':
                return '×';
            case '/':
                return '÷';
            default:
                return canonicalSymbol;
        }
    }
}