using UnityEngine;
using UnityEngine.InputSystem;

public class AlMovement : MonoBehaviour
{
    public float speed = 5f;
    float x = 0f;
    float y = 0f;
    Vector3 direction = new Vector3(0, 0, 0);

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        direction = new Vector3(input.x, input.y, 0);
    }

    void Update()
    {   
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}