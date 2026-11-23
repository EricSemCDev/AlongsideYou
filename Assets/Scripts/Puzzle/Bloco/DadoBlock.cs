using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Executa depois do DepthSorter do Al, para o bloco carregado ler a ordem de
// desenho ja atualizada do Al (sem piscar atras dele).
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DadoBlock : MonoBehaviour
{
    [Header("Faces (placeholder até o PuzzleGenerator existir)")]
    [SerializeField] private List<int> faces = new() { 3, 7, 1, 9 };

    [Header("Visual")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private TextMeshPro valueLabel;
    [SerializeField] private Vector2 valueLabelSlotOffset;

    [Header("Carregado pelo Al")]
    [SerializeField] private bool invertHeldSide;
    [SerializeField] private bool invertHeldSpriteFlip;
    [SerializeField] private bool invertHeldLabelSide;
    [SerializeField] private Vector2 heldPositionOffset;
    [SerializeField] private Vector2 heldLabelOffset;

    [Header("Ordem de desenho")]
    [SerializeField] private int heldOrderOffset = 2;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string inSlotParameter = "Preenchido";

    private static readonly int OutlineEnabledId = Shader.PropertyToID("_OutlineEnabled");
    private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");

    private int _currentFaceIndex;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private int _inSlotParameterId;
    private Vector3 _valueLabelBasePosition;
    private SpriteRenderer _holderRenderer;
    private AlMovement _holder;
    private Transform _holdPoint;
    private int _baseLayerId;
    private int _baseOrder;
    private int _labelBaseLayerId;
    private int _labelBaseOrder;

    public int Value => faces[_currentFaceIndex];
    public int FaceCount => faces.Count;
    public bool IsHeld { get; private set; }
    public NumericSlot CurrentSlot { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseLayerId = _spriteRenderer.sortingLayerID;
        _baseOrder = _spriteRenderer.sortingOrder;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _inSlotParameterId = Animator.StringToHash(inSlotParameter);

        if (valueLabel != null)
        {
            _valueLabelBasePosition = valueLabel.transform.localPosition;
            _labelBaseLayerId = valueLabel.sortingLayerID;
            _labelBaseOrder = valueLabel.sortingOrder;
        }

        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        UpdateValueLabel();
    }

    public void RotateFaceLeft()
    {
        FeedbackAudio.Play(FeedbackSounds.Cue.BlockRotate);
        _currentFaceIndex = (_currentFaceIndex - 1 + faces.Count) % faces.Count;
        UpdateValueLabel();
    }

    public void RotateFaceRight()
    {
        FeedbackAudio.Play(FeedbackSounds.Cue.BlockRotate);
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

    private void SetInSlotVisual(bool inSlot)
    {
        if (animator != null)
        {
            animator.SetBool(_inSlotParameterId, inSlot);
        }

        if (valueLabel != null)
        {
            Vector3 offset = inSlot ? (Vector3)valueLabelSlotOffset : Vector3.zero;
            valueLabel.transform.localPosition = _valueLabelBasePosition + offset;
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        _spriteRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(OutlineEnabledId, highlighted ? 1f : 0f);
        _propertyBlock.SetColor(OutlineColorId, highlightColor);
        _spriteRenderer.SetPropertyBlock(_propertyBlock);
    }

    public void SetFaces(List<int> newFaces)
    {
        faces = newFaces;
        _currentFaceIndex = 0;
        UpdateValueLabel();
    }

    public void Pickup(Transform holdPoint)
    {
        FeedbackAudio.Play(FeedbackSounds.Cue.BlockPickup);
        if (CurrentSlot != null)
        {
            CurrentSlot.NotifyBlockRemoved(this);
            CurrentSlot = null;
        }

        IsHeld = true;
        _collider.enabled = false;
        SetInSlotVisual(false);
        SetHighlighted(false);

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;

        _holdPoint = holdPoint;
        _holder = holdPoint.GetComponentInParent<AlMovement>();
        _holderRenderer = _holder != null ? _holder.Visual : null;
        ApplyHeldSorting();
        ApplyHeldFacing();
    }

    public void Drop(Vector3 worldPosition)
    {
        FeedbackAudio.Play(FeedbackSounds.Cue.BlockDrop);
        IsHeld = false;
        _collider.enabled = true;
        SetInSlotVisual(false);

        transform.SetParent(null);
        transform.position = worldPosition;

        RestoreSorting();
    }

    public void PlaceInSlot(Transform slotAnchor, NumericSlot slot, Vector3 localOffset = default)
    {
        IsHeld = false;
        CurrentSlot = slot;
        _collider.enabled = true;
        SetInSlotVisual(true);

        transform.SetParent(slotAnchor);
        transform.localPosition = localOffset;

        RestoreSorting();
        GameJuice.BlockSnapped(transform);
    }

    private void LateUpdate()
    {
        if (IsHeld)
        {
            ApplyHeldSorting();
            ApplyHeldFacing();
        }
    }

    private void ApplyHeldSorting()
    {
        if (_holderRenderer == null)
        {
            return;
        }

        int layerId = _holderRenderer.sortingLayerID;
        int order = _holderRenderer.sortingOrder + heldOrderOffset;

        _spriteRenderer.sortingLayerID = layerId;
        _spriteRenderer.sortingOrder = order;

        if (valueLabel != null)
        {
            valueLabel.sortingLayerID = layerId;
            valueLabel.sortingOrder = order + 1;
        }
    }

    private void ApplyHeldFacing()
    {
        if (_holderRenderer == null || _holder == null)
        {
            return;
        }

        bool facingLeft = _holderRenderer.flipX;
        _spriteRenderer.flipX = facingLeft != invertHeldSpriteFlip;

        if (_holdPoint != null)
        {
            Vector3 local = _holder.transform.InverseTransformPoint(_holdPoint.position);
            bool blockOnLeft = facingLeft != invertHeldSide;
            float sign = blockOnLeft ? -1f : 1f;
            float x = (Mathf.Abs(local.x) + heldPositionOffset.x) * sign;
            transform.position = _holder.transform.TransformPoint(new Vector3(x, local.y + heldPositionOffset.y, local.z));
        }

        if (valueLabel != null)
        {
            bool labelOnLeft = facingLeft != invertHeldLabelSide;
            float x = _valueLabelBasePosition.x + heldLabelOffset.x;
            x = labelOnLeft ? -x : x;
            valueLabel.transform.localPosition = new Vector3(x, _valueLabelBasePosition.y + heldLabelOffset.y, _valueLabelBasePosition.z);
        }
    }

    private void RestoreSorting()
    {
        _holderRenderer = null;
        _holder = null;
        _holdPoint = null;
        _spriteRenderer.flipX = false;
        _spriteRenderer.sortingLayerID = _baseLayerId;
        _spriteRenderer.sortingOrder = _baseOrder;

        if (valueLabel != null)
        {
            valueLabel.sortingLayerID = _labelBaseLayerId;
            valueLabel.sortingOrder = _labelBaseOrder;
        }
    }
}