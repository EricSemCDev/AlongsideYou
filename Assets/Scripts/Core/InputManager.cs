using UnityEngine;
using UnityEngine.InputSystem;

public enum GamepadActiveCharacter
{
    Al,
    Finn
}

public enum InputMode
{
    Solo,
    TwoPlayers
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Modo de controle")]
    [SerializeField] private InputMode mode = InputMode.Solo;

    [Header("Modo Solo — gamepad único controlando os dois personagens")]
    [SerializeField] private GamepadActiveCharacter activeCharacter = GamepadActiveCharacter.Al;

    private PlayerInputActions inputActions;

    public Vector2 AlMoveInput { get; private set; }
    public bool AlInteractPressed { get; private set; }
    public bool AlRotateFaceLeftPressed { get; private set; }
    public bool AlRotateFaceRightPressed { get; private set; }

    public Vector2 FinnPointerScreenPosition { get; private set; }
    public Vector2 FinnStickInput { get; private set; }
    public bool FinnPointerHeld { get; private set; }
    public bool FinnSelectOperatorPressed { get; private set; }
    public bool FinnSelectOperatorReleased { get; private set; }

    public GamepadActiveCharacter ActiveCharacter => activeCharacter;
    public InputMode CurrentMode => mode;

    // Disparado no momento exato da troca de personagem (só no Modo Solo),
    // com o novo personagem ativo. Um script futuro de VFX/animação pode
    // assinar isso para tocar o efeito de "alma" viajando entre os
    // personagens, sem o InputManager precisar saber nada sobre visual.
    public event System.Action<GamepadActiveCharacter> OnCharacterSwapped;

    // Chamado por um futuro menu de configurações para trocar de modo.
    public void SetInputMode(InputMode newMode)
    {
        mode = newMode;
    }

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
            ZeroAllInputs();
            return;
        }

        ReadKeyboardAndMouseInputs();

        if (mode == InputMode.Solo)
        {
            HandleCharacterSwap();
            ReadGamepadInputsIntoActiveCharacter();
        }
        else
        {
            ReadDualGamepadInputs();
        }
    }

    private void HandleCharacterSwap()
    {
        if (inputActions.Gamepad.SwapCharacter.WasPressedThisFrame())
        {
            activeCharacter = activeCharacter == GamepadActiveCharacter.Al
                ? GamepadActiveCharacter.Finn
                : GamepadActiveCharacter.Al;

            OnCharacterSwapped?.Invoke(activeCharacter);
        }
    }

    private void ReadKeyboardAndMouseInputs()
    {
        // Teclado (Al) e mouse (Finn) sempre ativos, independente do modo
        // e de qual personagem o(s) gamepad(s) está(ão) controlando.
        AlMoveInput = inputActions.Al.Move.ReadValue<Vector2>();
        AlInteractPressed = inputActions.Al.Interact.WasPressedThisFrame();
        AlRotateFaceLeftPressed = inputActions.Al.RotateFaceLeft.WasPressedThisFrame();
        AlRotateFaceRightPressed = inputActions.Al.RotateFaceRight.WasPressedThisFrame();

        FinnPointerScreenPosition = inputActions.Finn.Point.ReadValue<Vector2>();
        FinnPointerHeld = inputActions.Finn.Hold.IsPressed();
        FinnSelectOperatorPressed = inputActions.Finn.SelectOperator.WasPressedThisFrame();
        FinnSelectOperatorReleased = inputActions.Finn.SelectOperator.WasReleasedThisFrame();

        // Zerado aqui e preenchido depois, conforme o modo (Solo ou
        // TwoPlayers), em ReadGamepadInputsIntoActiveCharacter ou
        // ReadDualGamepadInputs.
        FinnStickInput = Vector2.zero;
    }

    private void ReadGamepadInputsIntoActiveCharacter()
    {
        Vector2 gamepadMove = inputActions.Gamepad.Move.ReadValue<Vector2>();
        bool gamepadInteract = inputActions.Gamepad.Interact.WasPressedThisFrame();
        bool gamepadShoulderLeft = inputActions.Gamepad.ButtonShoulderLeft.WasPressedThisFrame();
        bool gamepadShoulderRight = inputActions.Gamepad.ButtonShoulderRight.WasPressedThisFrame();

        if (activeCharacter == GamepadActiveCharacter.Al)
        {
            AlMoveInput += gamepadMove;
            AlInteractPressed |= gamepadInteract;
            AlRotateFaceLeftPressed |= gamepadShoulderLeft;
            AlRotateFaceRightPressed |= gamepadShoulderRight;
        }
        else
        {
            FinnStickInput = gamepadMove;
            // Interact/ShoulderLeft/ShoulderRight não têm equivalente no
            // Finn ainda — quando existirem, tratamos aqui.
        }
    }

    private void ReadDualGamepadInputs()
    {
        // Modo 2 jogadores: bypassa o Action Map "Gamepad" (que trata
        // qualquer controle genericamente) e lê direto dos dispositivos
        // físicos via Gamepad.all, na ordem em que foram conectados.
        // Regra fixa por enquanto (sem menu de escolha): primeiro = Al,
        // segundo = Finn.
        var gamepads = Gamepad.all;

        Gamepad gamepadForAl = gamepads.Count > 0 ? gamepads[0] : null;
        Gamepad gamepadForFinn = gamepads.Count > 1 ? gamepads[1] : null;

        if (gamepadForAl != null)
        {
            AlMoveInput += gamepadForAl.leftStick.ReadValue();
            AlInteractPressed |= gamepadForAl.buttonSouth.wasPressedThisFrame;
            AlRotateFaceLeftPressed |= gamepadForAl.leftShoulder.wasPressedThisFrame;
            AlRotateFaceRightPressed |= gamepadForAl.rightShoulder.wasPressedThisFrame;
        }

        if (gamepadForFinn != null)
        {
            FinnStickInput = gamepadForFinn.leftStick.ReadValue();
            // Botões do Finn (quando existirem) entram aqui.
        }
    }

    private void ZeroAllInputs()
    {
        AlMoveInput = Vector2.zero;
        AlInteractPressed = false;
        AlRotateFaceLeftPressed = false;
        AlRotateFaceRightPressed = false;
        FinnStickInput = Vector2.zero;
        FinnPointerHeld = false;
        FinnSelectOperatorPressed = false;
        FinnSelectOperatorReleased = false;
    }

    private void SetInputMapsEnabled(bool enabled)
    {
        bool enableSharedGamepadMap = enabled && mode == InputMode.Solo;

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

        if (enableSharedGamepadMap)
        {
            if (!inputActions.Gamepad.enabled) inputActions.Gamepad.Enable();
        }
        else
        {
            if (inputActions.Gamepad.enabled) inputActions.Gamepad.Disable();
        }
    }
}