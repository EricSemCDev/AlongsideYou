using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DadoBlock : MonoBehaviour
{
    // Placeholder: valor fixo por enquanto. Será substituído pelo
    // PuzzleGenerator/PuzzleConfig quando a geração procedural existir.
    [SerializeField] private int value = 3;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public int Value => value;
    public bool IsHeld { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    public void SetHighlighted(bool highlighted)
    {
        _spriteRenderer.color = highlighted ? highlightColor : _originalColor;
    }

    public void Pickup(Transform holdPoint)
    {
        IsHeld = true;
        _collider.enabled = false;
        SetHighlighted(false);

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    public void Drop(Vector3 worldPosition)
    {
        IsHeld = false;
        _collider.enabled = true;

        transform.SetParent(null);
        transform.position = worldPosition;
    }
}