using UnityEngine;

public class FinnMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float followSpeed = 5f;

    private Camera _mainCamera;
    private Rigidbody2D _mainRigidbody;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _mainRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTargetPosition();
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    private void UpdateTargetPosition()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        Vector2 pointerScreenPosition = InputManager.Instance.FinnPointerScreenPosition;

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, _mainCamera.nearClipPlane)
        );

        _targetPosition = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }

    private void MoveTowardsTarget()
    {
        _mainRigidbody.MovePosition(Vector3.Lerp(
            _mainRigidbody.position,
            _targetPosition,
            followSpeed * Time.deltaTime
        ));
    }
}