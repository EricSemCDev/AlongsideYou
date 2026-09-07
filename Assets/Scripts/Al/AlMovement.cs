using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minMagnitudeForFacing = 0.2f;

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

        // Limita a 1 sem forçar magnitude cheia: preserva input analógico
        // parcial do gamepad e ainda corrige diagonais de WASD (que somam
        // até ~1.41 sem isso).
        Vector2 direction = Vector2.ClampMagnitude(input, 1f);

        // Só atualiza a direção "olhada" com input intencional o
        // suficiente. Sem isso, resíduo de analógico (que nunca volta
        // exatamente a zero) sobrescreve a última direção real com ruído.
        if (direction.magnitude > minMagnitudeForFacing)
        {
            FacingDirection = direction.normalized;
        }

        Vector2 newPosition = _rigidbody.position + direction * speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(newPosition);
    }
}