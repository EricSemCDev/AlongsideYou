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

    [Header("Visual")]
    [SerializeField] private Transform targetIndicator;

    private Camera _mainCamera;
    private Rigidbody2D _mainRigidbody;
    private Vector2 _targetOffset;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _mainRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsOperatorMenuBlockingMovement())
        {
            return;
        }

        UpdateTargetOffset();
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

        Vector2 directionToPointer = CalculateDirectionToPointer();

        _targetOffset = FinnTargetOffsetCalculator.CalculateNewOffset(
            currentOffset: _targetOffset,
            currentVelocity: ref _targetOffsetVelocity,
            stickInput: InputManager.Instance.FinnStickInput,
            directionToPointer: directionToPointer,
            pointerHeld: InputManager.Instance.FinnPointerHeld,
            maxRadius: targetOffsetMaxRadius,
            smoothTime: targetOffsetSmoothTime,
            maxSpeed: targetOffsetMaxSpeed,
            deltaTime: Time.deltaTime
        );

        UpdateTargetIndicator();
    }

    private void UpdateTargetIndicator()
    {
        if (targetIndicator == null)
        {
            return;
        }

        targetIndicator.position = transform.position + (Vector3)_targetOffset;
    }

    private Vector2 CalculateDirectionToPointer()
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

        return direction.normalized;
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