using UnityEngine;

public class AlMovement : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        Vector2 input = InputManager.Instance != null
            ? InputManager.Instance.AlMoveInput
            : Vector2.zero;

        Vector3 direction = new Vector3(input.x, input.y, 0f);
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}