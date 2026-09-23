using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DadoBlock : MonoBehaviour
{
    [Header("Faces (placeholder até o PuzzleGenerator existir)")]
    [SerializeField] private List<int> faces = new() { 3, 7, 1, 9 };

    [Header("Visual")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private TextMeshPro valueLabel;

    private int _currentFaceIndex;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public int Value => faces[_currentFaceIndex];
    public bool IsHeld { get; private set; }
    public NumericSlot CurrentSlot { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    private void Start()
    {
        UpdateValueLabel();
    }

    public void RotateFaceLeft()
    {
        _currentFaceIndex = (_currentFaceIndex - 1 + faces.Count) % faces.Count;
        UpdateValueLabel();
    }

    public void RotateFaceRight()
    {
        _currentFaceIndex = (_currentFaceIndex + 1) % faces.Count;
        UpdateValueLabel();
    }

    private void UpdateValueLabel()
    {
        if (valueLabel != null)
        {
            valueLabel.text = Value.ToString();
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        _spriteRenderer.color = highlighted ? highlightColor : _originalColor;
    }

    public void Pickup(Transform holdPoint)
    {
        if (CurrentSlot != null)
        {
            CurrentSlot.NotifyBlockRemoved(this);
            CurrentSlot = null;
        }

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

    // Ancora o bloco visualmente no slot, mantendo o Collider2D ativo —
    // isso permite que o Al detecte e pegue o bloco de volta depois,
    // reaproveitando a mesma zona de detecção usada para blocos no chão.
    public void PlaceInSlot(Transform slotAnchor, NumericSlot slot)
    {
        IsHeld = false;
        CurrentSlot = slot;
        _collider.enabled = true;

        transform.SetParent(slotAnchor);
        transform.localPosition = Vector3.zero;
    }
}