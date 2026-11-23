using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NumericSlot : MonoBehaviour
{
    [SerializeField] private bool isComposite;
    [SerializeField] private int position;

    [Header("Block Position")]

    [SerializeField] private Vector2 blockOffset;
    [SerializeField] private Vector2 unitsBlockOffset;

    [Header("Slot composto (2 blocos)")]

    [SerializeField] private RuntimeAnimatorController compositeAnimatorController;
    [SerializeField] private Transform singleAnchor;
    [SerializeField] private Transform tensAnchor;
    [SerializeField] private Transform unitsAnchor;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string filledParameter = "Preenchido";

    private DadoBlock _tensBlock;
    private DadoBlock _unitsBlock;

    private int _filledParameterId;
    private bool _hasFilledParameter;

    public bool IsComposite => isComposite;
    public int Position => position;

    public bool IsFilled => isComposite
        ? (_tensBlock != null && _unitsBlock != null)
        : (_tensBlock != null);

    public int CurrentValue
    {
        get
        {
            if (!isComposite)
            {
                return _tensBlock != null ? _tensBlock.Value : 0;
            }

            if (_tensBlock == null || _unitsBlock == null)
            {
                return 0;
            }

            return (_tensBlock.Value * 10) + _unitsBlock.Value;
        }
    }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (isComposite && compositeAnimatorController != null && animator != null)
        {
            animator.runtimeAnimatorController = compositeAnimatorController;
        }

        _filledParameterId = Animator.StringToHash(filledParameter);
        _hasFilledParameter = HasFilledParameter();

        UpdateFilledState();
    }

    public bool TryInsertBlock(DadoBlock block)
    {
        if (!isComposite)
        {
            if (_tensBlock != null)
            {
                return false;
            }

            _tensBlock = block;
            block.PlaceInSlot(singleAnchor != null ? singleAnchor : transform, this, blockOffset);
            UpdateFilledState();
            return true;
        }

        if (_tensBlock == null)
        {
            _tensBlock = block;
            block.PlaceInSlot(tensAnchor != null ? tensAnchor : transform, this, blockOffset);
            UpdateFilledState();
            return true;
        }

        if (_unitsBlock == null)
        {
            _unitsBlock = block;
            block.PlaceInSlot(unitsAnchor != null ? unitsAnchor : transform, this, unitsBlockOffset);
            UpdateFilledState();
            return true;
        }

        return false;
    }

    public void NotifyBlockRemoved(DadoBlock block)
    {
        if (_tensBlock == block)
        {
            _tensBlock = null;
        }
        else if (_unitsBlock == block)
        {
            _unitsBlock = null;
        }

        UpdateFilledState();
    }

    private void UpdateFilledState()
    {
        if (animator == null || !_hasFilledParameter)
        {
            return;
        }

        animator.SetBool(_filledParameterId, IsFilled);
    }

    private bool HasFilledParameter()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == _filledParameterId && parameter.type == AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }
}