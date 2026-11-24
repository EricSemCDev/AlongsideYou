using System;
using UnityEngine;

public class ThornBarrier : MonoBehaviour
{
    [SerializeField] private CalculationStationDisplay station;

    [Header("Animator")]

    [SerializeField] private string openParameter = "Open";

    private Collider2D[] _colliders;
    private Animator[] _animators;
    private int _openParameterId;

    public bool IsOpen { get; private set; }

    public event Action OnOpened;

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider2D>();
        _animators = GetComponentsInChildren<Animator>();
        _openParameterId = Animator.StringToHash(openParameter);

        if (station == null)
        {
            station = FindFirstObjectByType<CalculationStationDisplay>();
        }
    }

    private void OnEnable()
    {
        if (station != null)
        {
            station.OnEquationValidated += HandleEquationValidated;
        }
    }

    private void OnDisable()
    {
        if (station != null)
        {
            station.OnEquationValidated -= HandleEquationValidated;
        }
    }

    private void HandleEquationValidated(bool isCorrect)
    {
        if (isCorrect)
        {
            Open();
        }
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;

        foreach (Collider2D barrierCollider in _colliders)
        {
            barrierCollider.enabled = false;
        }

        foreach (Animator animator in _animators)
        {
            if (HasBoolParameter(animator))
            {
                animator.SetBool(_openParameterId, true);
            }
        }

        OnOpened?.Invoke();
    }

    private bool HasBoolParameter(Animator animator)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == _openParameterId && parameter.type == AnimatorControllerParameterType.Bool)
            {
                return true;
            }
        }

        return false;
    }
}
