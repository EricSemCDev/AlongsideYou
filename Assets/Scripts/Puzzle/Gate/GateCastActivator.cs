using UnityEngine;

public enum CastCharacterRequirement
{
    Al,
    Finn,
    Either
}

// Zona de "cast": o personagem certo precisa ficar parado dentro
// dela, com a condição matemática satisfeita, por castDuration
// segundos seguidos, pra barreira abrir. Qualquer movimento ou saída
// da zona reinicia o progresso do zero. A barreira só permanece
// aberta enquanto ambas as condições continuarem verdadeiras.
[RequireComponent(typeof(Collider2D))]
public class GateCastActivator : MonoBehaviour
{
    [SerializeField] private GateController gate;
    [SerializeField] private MonoBehaviour conditionSource; // precisa implementar IGateCondition
    [SerializeField] private CastCharacterRequirement requiredCaster = CastCharacterRequirement.Either;
    [SerializeField] private float castDuration = 3f;
    [SerializeField] private float movementCancelThreshold = 0.05f;

    private IGateCondition _condition;
    private float _castProgress;
    private bool _alPresent;
    private bool _finnPresent;

    private void Awake()
    {
        _condition = conditionSource as IGateCondition;

        if (_condition == null)
        {
            Debug.LogWarning(
                $"GateCastActivator em '{name}': o objeto atribuído em Condition Source " +
                "não implementa IGateCondition. A barreira nunca vai abrir."
            );
        }
    }

    private void Update()
    {
        bool casterPresent = IsRequiredCasterPresent();
        bool casterStill = casterPresent && !IsCasterMoving();
        bool conditionMet = _condition != null && _condition.ConditionMet;

        if (casterStill && conditionMet)
        {
            _castProgress += Time.deltaTime;
        }
        else
        {
            _castProgress = 0f;
        }

        bool shouldBeOpen = casterPresent && conditionMet && _castProgress >= castDuration;
        gate?.SetOpen(shouldBeOpen);
    }

    private bool IsRequiredCasterPresent()
    {
        return requiredCaster switch
        {
            CastCharacterRequirement.Al => _alPresent,
            CastCharacterRequirement.Finn => _finnPresent,
            CastCharacterRequirement.Either => _alPresent || _finnPresent,
            _ => false,
        };
    }

    private bool IsCasterMoving()
    {
        if (InputManager.Instance == null)
        {
            return false;
        }

        bool checkAl = requiredCaster == CastCharacterRequirement.Al
            || (requiredCaster == CastCharacterRequirement.Either && _alPresent);

        if (checkAl && InputManager.Instance.AlMoveInput.magnitude > movementCancelThreshold)
        {
            return true;
        }

        bool checkFinn = requiredCaster == CastCharacterRequirement.Finn
            || (requiredCaster == CastCharacterRequirement.Either && _finnPresent);

        if (checkFinn)
        {
            if (InputManager.Instance.FinnStickInput.magnitude > movementCancelThreshold)
            {
                return true;
            }

            if (InputManager.Instance.FinnPointerHeld)
            {
                return true; // segurando o clique também conta como "tentando se mover"
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AlMovement>() != null)
        {
            _alPresent = true;
        }

        if (other.GetComponent<FinnMovement>() != null)
        {
            _finnPresent = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<AlMovement>() != null)
        {
            _alPresent = false;
        }

        if (other.GetComponent<FinnMovement>() != null)
        {
            _finnPresent = false;
        }
    }
}