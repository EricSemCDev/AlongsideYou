using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;

    public Vector2 AlMoveInput { get; private set; }
    public Vector2 FinnPointerScreenPosition { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            inputActions.Dispose();
        }
    }

    private void Update()
    {
        bool shouldAcceptInput = GameManager.Instance != null
            && InputAccessPolicy.CanAcceptInput(GameManager.Instance.CurrentState);

        SetInputMapsEnabled(shouldAcceptInput);

        if (!shouldAcceptInput)
        {
            AlMoveInput = Vector2.zero;
            return;
        }

        AlMoveInput = inputActions.Al.Move.ReadValue<Vector2>();
        FinnPointerScreenPosition = inputActions.Finn.Point.ReadValue<Vector2>();
    }

    private void SetInputMapsEnabled(bool enabled)
    {
        if (enabled)
        {
            if (!inputActions.Al.enabled) inputActions.Al.Enable();
            if (!inputActions.Finn.enabled) inputActions.Finn.Enable();
        }
        else
        {
            if (inputActions.Al.enabled) inputActions.Al.Disable();
            if (inputActions.Finn.enabled) inputActions.Finn.Disable();
        }
    }
}