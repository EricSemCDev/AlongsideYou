using UnityEngine;
using UnityEngine.InputSystem;

public class AlMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 10f;

    [Header("Pulo")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTimeDuration = 0.15f;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferDuration = 0.15f;

    [Header("Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    private bool _isGrounded;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGrounded();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleJump();
        HandleGravity();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // --- Input System callbacks ---

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            _jumpBufferCounter = jumpBufferDuration;
    }

    // --- Lógica ---

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleCoyoteTime()
    {
        if (_isGrounded)
            _coyoteTimeCounter = coyoteTimeDuration;
        else
            _coyoteTimeCounter -= Time.deltaTime;
    }

    private void HandleJumpBuffer()
    {
        _jumpBufferCounter -= Time.deltaTime;
    }

    private void HandleJump()
    {
        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;
        }
    }

    private void HandleGravity()
    {
        if (_rb.velocity.y < 0f)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0f && !Keyboard.current.spaceKey.isPressed)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = _moveInput.x * moveSpeed;
        float speedDiff = targetSpeed - _rb.velocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * accelRate;

        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }
}