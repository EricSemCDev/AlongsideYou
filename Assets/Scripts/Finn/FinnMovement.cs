using UnityEngine;

public class FinnMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float followSmoothTime = 0.1f;
    [SerializeField] private float followMaxSpeed = 10f;

    [Header("Target Offset")]
    [SerializeField] private float targetOffsetSmoothTime = 0.15f;
    [SerializeField] private float targetOffsetMaxSpeed = 8f;
    [SerializeField] private float targetOffsetMaxRadius = 1.5f;
    [SerializeField] private float pointerDeadZone = 0.2f;

    private Vector2 _targetOffsetVelocity;
    private Vector2 _followVelocity;

    public float CurrentSpeed => _followVelocity.magnitude;

    [Header("Visual")]
    [SerializeField] private Transform targetIndicator;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flipVelocityThreshold = 0.05f;

    // Hash do parâmetro em vez de string todo frame — SetFloat("Speed", ...)
    // faz um lookup por string internamente; StringToHash faz isso uma vez só.
    private static readonly int SpeedParam = Animator.StringToHash("Speed");

    private Camera _mainCamera;
    private Rigidbody2D _mainRigidbody;
    private Vector2 _targetOffset;

    // Referência estática simples (não é singleton completo, só existe
    // um Finn na cena) para outros scripts (ex: ProximityVisibility)
    // encontrarem a posição dele sem precisar arrastar manualmente em
    // cada objeto da fase.
    public static Transform FinnTransform { get; private set; }

    private void Awake()
    {
        FinnTransform = transform;
        _mainCamera = Camera.main;
        _mainRigidbody = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (IsOperatorMenuBlockingMovement())
        {
            // Congela a animação em Idle enquanto o menu de operador
            // está aberto, já que o Finn não se move nesse estado.
            UpdateAnimatorSpeed(0f);
            return;
        }

        UpdateTargetOffset();
        UpdateAnimatorSpeed(CurrentSpeed);
        UpdateSpriteFlip(_followVelocity.x);
    }

    private void FixedUpdate()
    {
        if (IsOperatorMenuBlockingMovement())
        {
            return;
        }

        MoveTowardsTarget();
    }

    // Enquanto o menu de operador está aberto, o Finn fica parado —
    // isso não afeta o Al, que continua jogando normalmente.
    private bool IsOperatorMenuBlockingMovement()
    {
        return OperatorMenuController.Instance != null && OperatorMenuController.Instance.IsOpen;
    }

    private void UpdateTargetOffset()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        Vector2 pointerOffset = CalculatePointerOffset();

        _targetOffset = FinnTargetOffsetCalculator.CalculateNewOffset(
            currentOffset: _targetOffset,
            currentVelocity: ref _targetOffsetVelocity,
            stickInput: InputManager.Instance.FinnStickInput,
            pointerOffset: pointerOffset,
            pointerHeld: InputManager.Instance.FinnPointerHeld,
            maxRadius: targetOffsetMaxRadius,
            smoothTime: targetOffsetSmoothTime,
            maxSpeed: targetOffsetMaxSpeed,
            deltaTime: Time.deltaTime
        );

        UpdateTargetIndicator();
    }

    private void UpdateAnimatorSpeed(float speed)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(SpeedParam, speed);
    }

    private void UpdateSpriteFlip(float velocityX)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        // Zona morta: evita o sprite "tremer" invertendo toda hora quando
        // o movimento no eixo X é quase zero (ex: andando só na vertical).
        if (Mathf.Abs(velocityX) < flipVelocityThreshold)
        {
            return;
        }

        spriteRenderer.flipX = velocityX < 0f;
    }

    private void UpdateTargetIndicator()
    {
        if (targetIndicator == null)
        {
            return;
        }

        targetIndicator.position = transform.position + (Vector3)_targetOffset;
    }

    // Vetor do Finn ate o ponteiro (nao normalizado). Zero dentro da zona morta.
    private Vector2 CalculatePointerOffset()
    {
        Vector2 pointerScreenPosition = InputManager.Instance.FinnPointerScreenPosition;

        Vector3 pointerWorldPosition = _mainCamera.ScreenToWorldPoint(
            new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, _mainCamera.nearClipPlane)
        );

        Vector2 direction = (Vector2)pointerWorldPosition - (Vector2)transform.position;

        if (direction.magnitude < pointerDeadZone)
        {
            return Vector2.zero;
        }

        return direction;
    }

    private void MoveTowardsTarget()
    {
        Vector2 targetPosition = (Vector2)transform.position + _targetOffset;

        float velocityX = _followVelocity.x;
        float velocityY = _followVelocity.y;

        float newX = Mathf.SmoothDamp(_mainRigidbody.position.x, targetPosition.x, ref velocityX, followSmoothTime, followMaxSpeed, Time.fixedDeltaTime);
        float newY = Mathf.SmoothDamp(_mainRigidbody.position.y, targetPosition.y, ref velocityY, followSmoothTime, followMaxSpeed, Time.fixedDeltaTime);

        _followVelocity = new Vector2(velocityX, velocityY);

        _mainRigidbody.MovePosition(new Vector2(newX, newY));
    }

    private void OnDrawGizmos()
    {
        Vector3 targetPosition = transform.position + (Vector3)_targetOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.15f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}