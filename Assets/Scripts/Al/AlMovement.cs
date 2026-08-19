using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D _rigidbody;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 input = InputManager.Instance != null
            ? InputManager.Instance.AlMoveInput
            : Vector2.zero;

        Vector2 direction = Vector2.ClampMagnitude(input, 1f);


        if (direction != Vector2.zero)
        {
            FacingDirection = direction.normalized;
            Debug.Log(direction);
        }

        Vector2 newPosition = _rigidbody.position + direction * speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(newPosition);
    }
}