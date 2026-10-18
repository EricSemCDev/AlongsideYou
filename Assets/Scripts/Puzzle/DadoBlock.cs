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
    [Tooltip("Cor do contorno quando o bloco esta em destaque (com o material de contorno).")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private TextMeshPro valueLabel;
    [Tooltip("Deslocamento do numero em relacao a posicao normal, quando o bloco esta encaixado num slot.")]
    [SerializeField] private Vector2 valueLabelSlotOffset;
    [Header("Carregado pelo Al")]
    [Tooltip("Troca o lado em que o bloco fica (direita <-> esquerda). Ligue se ele estiver no lado errado.")]
    [SerializeField] private bool invertHeldSide;
    [Tooltip("Troca quando o sprite do bloco e invertido. Ligue se o desenho estiver virado ao contrario.")]
    [SerializeField] private bool invertHeldSpriteFlip;
    [Tooltip("Troca o lado do numero (independente do bloco). Ligue se o numero estiver do lado errado.")]
    [SerializeField] private bool invertHeldLabelSide;
    [Tooltip("Ajuste fino da posicao do bloco, para o Al olhando para a direita. O X e espelhado para a esquerda.")]
    [SerializeField] private Vector2 heldPositionOffset;
    [Tooltip("Ajuste do numero enquanto o bloco e carregado, para o Al olhando para a direita. O X e espelhado para a esquerda.")]
    [SerializeField] private Vector2 heldLabelOffset;

    [Header("Ordem de desenho")]
    [Tooltip("Quando carregado, o bloco fica na mesma Sorting Layer do Al e este tanto acima da ordem dele.")]
    [SerializeField] private int heldOrderOffset = 2;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [Tooltip("Parametro Bool do Animator ligado enquanto o bloco esta encaixado num slot.")]
    [SerializeField] private string inSlotParameter = "Preenchido";

    private static readonly int OutlineEnabledId = Shader.PropertyToID("_OutlineEnabled");
    private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");

    private int _currentFaceIndex;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private MaterialPropertyBlock _propertyBlock;
    private bool _supportsOutline;
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
    public bool IsHeld { get; private set; }
    public NumericSlot CurrentSlot { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _baseLayerId = _spriteRenderer.sortingLayerID;
        _baseOrder = _spriteRenderer.sortingOrder;

        // Se o material do bloco for o de contorno, o destaque vira contorno.
        // Senão, cai no comportamento antigo (pintar o sprite).
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
        Material material = _spriteRenderer.sharedMaterial;
        _supportsOutline = material != null && material.HasProperty(OutlineEnabledId);
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

    // Troca o desenho do bloco (cubo solto <-> quadrado encaixado) pelo Animator.
    private void SetInSlotVisual(bool inSlot)
    {
        if (animator != null)
        {
            animator.SetBool(_inSlotParameterId, inSlot);
        }

        // O numero muda de lugar junto com o desenho, na hora (sem animar).
        if (valueLabel != null)
        {
            Vector3 offset = inSlot ? (Vector3)valueLabelSlotOffset : Vector3.zero;
            valueLabel.transform.localPosition = _valueLabelBasePosition + offset;
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        if (_supportsOutline)
        {
            // MaterialPropertyBlock: não cria uma cópia do material por bloco.
            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(OutlineEnabledId, highlighted ? 1f : 0f);
            _propertyBlock.SetColor(OutlineColorId, highlightColor);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
            return;
        }

        _spriteRenderer.color = highlighted ? highlightColor : _originalColor;
    }

    // Chamado pelo PuzzleGenerator para sobrescrever as faces placeholder
    // com valores gerados de verdade, no início da fase.
    public void SetFaces(List<int> newFaces)
    {
        faces = newFaces;
        _currentFaceIndex = 0;
        UpdateValueLabel();
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
        IsHeld = false;
        _collider.enabled = true;
        SetInSlotVisual(false);

        transform.SetParent(null);
        transform.position = worldPosition;

        RestoreSorting();
    }

    // Ancora o bloco visualmente no slot, mantendo o Collider2D ativo —
    // isso permite que o Al detecte e pegue o bloco de volta depois,
    // reaproveitando a mesma zona de detecção usada para blocos no chão.
    // localOffset permite posicionar 2 blocos lado a lado num slot
    // composto, em vez de sobrepostos.
    public void PlaceInSlot(Transform slotAnchor, NumericSlot slot, Vector3 localOffset = default)
    {
        IsHeld = false;
        CurrentSlot = slot;
        _collider.enabled = true;
        SetInSlotVisual(true);

        transform.SetParent(slotAnchor);
        transform.localPosition = localOffset;

        RestoreSorting();
    }

    // Carregado: desenha na frente do Al. Acompanha a ordem dele todo frame
    // (que muda conforme ele anda, pelo Y-sorting).
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

    // O Al so inverte o sprite (flipX) ao andar para a esquerda. O bloco acompanha:
    // inverte o proprio sprite, vai para o lado espelhado e leva o numero junto.
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
            // O bloco fica sempre do lado para onde o Al olha, nao importa de qual
            // lado do Al o HoldPoint foi posicionado: usa so a distancia dele.
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