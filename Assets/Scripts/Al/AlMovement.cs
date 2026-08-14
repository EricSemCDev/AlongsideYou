using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 input = InputManager.Instance != null
            ? InputManager.Instance.AlMoveInput
            : Vector2.zero;

        // Limita a 1 sem forçar magnitude cheia: preserva input analógico
        // parcial do gamepad e ainda corrige diagonais de WASD (que somam
        // até ~1.41 sem isso).
        Vector2 direction = Vector2.ClampMagnitude(input, 1f);

        Vector2 newPosition = _rigidbody.position + direction * speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(newPosition);
    }
}