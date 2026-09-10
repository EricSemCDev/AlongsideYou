using UnityEngine;

// Objeto físico que o Al interage para confirmar a tentativa de
// equação na Estação de Cálculo correspondente.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ConfirmationLever : MonoBehaviour
{
    [SerializeField] private CalculationStationDisplay targetStation;
    [SerializeField] private Color highlightColor = Color.yellow;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    public void SetHighlighted(bool highlighted)
    {
        _spriteRenderer.color = highlighted ? highlightColor : _originalColor;
    }

    public void Confirm()
    {
        if (targetStation != null)
        {
            targetStation.ConfirmAttempt();
        }
    }
}