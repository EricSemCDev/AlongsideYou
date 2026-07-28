using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

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

    void Update()
    {
        UpdateTargetPosition();
    }

    void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    private void UpdateTargetPosition()
    {
        // Pega posição do mouse em pixels
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // Converte para world space mantendo o Z do Finn
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, _mainCamera.nearClipPlane)
        );

        // Trava o Z para não sair do plano 2D
        _targetPosition = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }

    private void MoveTowardsTarget()
    {
        _mainRigidbody.MovePosition(Vector3.Lerp(
            _mainRigidbody.position,
            _targetPosition,
            followSpeed * Time.deltaTime
        ));
        Debug.Log("Finn pos: " + _mainRigidbody.position);
    }
}
