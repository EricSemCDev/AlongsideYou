using UnityEngine;

// Faísca-operador coletável na fase. O símbolo é um placeholder editável
// no Inspector por enquanto; o PuzzleGenerator (issue futura) vai
// sobrescrever via SetSymbol() com base na equação gerada para a fase.
[RequireComponent(typeof(Collider2D))]
public class OperatorSpark : MonoBehaviour
{
    [SerializeField] private char symbol = '+';

    public char Symbol => symbol;

    public void SetSymbol(char newSymbol)
    {
        symbol = newSymbol;
    }
}