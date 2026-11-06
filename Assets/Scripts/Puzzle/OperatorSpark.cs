using UnityEngine;

// Faísca-operador coletável na fase. O símbolo é um placeholder editável
// no Inspector por enquanto; o PuzzleGenerator (issue futura) vai
// sobrescrever via SetSymbol() com base na equação gerada para a fase.
[RequireComponent(typeof(Collider2D))]
public class OperatorSpark : MonoBehaviour
{
    [SerializeField] private char symbol = '+';

    [Header("Sprite por operador")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite plusSprite;
    [SerializeField] private Sprite minusSprite;
    [SerializeField] private Sprite timesSprite;
    [SerializeField] private Sprite divideSprite;

    public char Symbol => symbol;

    private void Start()
    {
        // O PuzzleGenerator pode ter trocado o simbolo antes ou depois do Awake
        // desta faisca; garante que o desenho bate com o simbolo final.
        RefreshVisual();
    }

    public void SetSymbol(char newSymbol)
    {
        symbol = newSymbol;
        RefreshVisual();
    }

    // Tambem atualiza no Editor ao mudar o simbolo no Inspector.
    private void OnValidate()
    {
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            return;
        }

        Sprite sprite = GetSpriteFor(symbol);

        // Sem sprite para esse simbolo, mantem o desenho que ja estava.
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    private Sprite GetSpriteFor(char operatorSymbol)
    {
        switch (operatorSymbol)
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
}