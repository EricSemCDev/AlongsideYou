using System;
using UnityEngine;

// Barreira de espinhos que abre de vez quando o Al acerta a equacao da
// Estacao de Calculo. Escuta o evento OnEquationValidated: ao receber
// "acertou", desliga os colliders (a passagem fica livre) e liga o
// parametro Bool do Animator em todos os espinhos filhos, que tocam a
// animacao de murchar juntos. Fica aberta para sempre.
//
// Coloque no objeto pai, que tem os espinhos como filhos e o collider.
// O collider deve estar na layer AlOnlyObstacles para o Finn passar.
public class ThornBarrier : MonoBehaviour
{
    [Tooltip("Estacao cuja equacao abre a barreira. Se vazio, usa a primeira da cena.")]
    [SerializeField] private CalculationStationDisplay station;

    [Header("Animator")]
    [Tooltip("Parametro Bool ligado ao abrir. Os Animators dos filhos sao encontrados sozinhos.")]
    [SerializeField] private string openParameter = "Open";

    private Collider2D[] _colliders;
    private Animator[] _animators;
    private int _openParameterId;

    public bool IsOpen { get; private set; }

    // Disparado uma unica vez, quando a barreira abre.
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

    // Evita o aviso "Parameter does not exist" enquanto o Animator dos
    // espinhos ainda nao tem o parametro (animacao de murchar a fazer).
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
