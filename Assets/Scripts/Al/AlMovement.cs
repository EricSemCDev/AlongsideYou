using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minMagnitudeForFacing = 0.2f;

    [Header("Visual")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flipInputThreshold = 0.05f;
    [SerializeField] private float minWalkAnimationSpeed = 0.25f;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");

    private Rigidbody2D _rigidbody;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    public float SpeedMultiplier { get; set; } = 1f;
    public bool MovementDisabled { get; set; }

    public SpriteRenderer Visual => spriteRenderer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void FixedUpdate()
    {
        Vector2 input = InputManager.Instance != null && !MovementDisabled
            ? InputManager.Instance.AlMoveInput
            : Vector2.zero;

        Vector2 direction = Vector2.ClampMagnitude(input, 1f);

        if (direction.magnitude > minMagnitudeForFacing)
        {
            FacingDirection = direction.normalized;
        }

        Vector2 newPosition = _rigidbody.position + direction * speed * SpeedMultiplier * Time.fixedDeltaTime;
        _rigidbody.MovePosition(newPosition);

        UpdateAnimatorSpeed(direction.magnitude);
        UpdateSpriteFlip(direction.x);
    }

    private void UpdateAnimatorSpeed(float speed)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(SpeedParam, speed);
        bool isMoving = speed > flipInputThreshold;
        animator.speed = isMoving ? Mathf.Max(SpeedMultiplier, minWalkAnimationSpeed) : 1f;
    }

    private void UpdateSpriteFlip(float inputX)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (Mathf.Abs(inputX) < flipInputThreshold)
        {
            return;
        }

        spriteRenderer.flipX = inputX < 0f;
    }
}