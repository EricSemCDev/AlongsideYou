using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;

// Descobre qual dispositivo o jogador usou por ultimo (teclado/mouse ou controle,
// e de que tipo) e avisa o InputPromptResolver. Se cria sozinho ao abrir o jogo,
// nao precisa colocar em cena.
public class InputPromptTracker : MonoBehaviour
{
    private const float MouseMoveThreshold = 6f; // pixels por frame
    private const float StickThreshold = 0.5f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        if (FindFirstObjectByType<InputPromptTracker>() != null)
        {
            return;
        }

        var go = new GameObject("InputPromptTracker");
        go.AddComponent<InputPromptTracker>();
        DontDestroyOnLoad(go);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.anyKey.wasPressedThisFrame)
        {
            InputPromptResolver.SetDevice(InputPromptResolver.Device.KeyboardMouse);
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse != null
            && (mouse.leftButton.wasPressedThisFrame
                || mouse.rightButton.wasPressedThisFrame
                || mouse.delta.ReadValue().sqrMagnitude > MouseMoveThreshold * MouseMoveThreshold))
        {
            InputPromptResolver.SetDevice(InputPromptResolver.Device.KeyboardMouse);
            return;
        }

        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (!gamepad.wasUpdatedThisFrame)
            {
                continue;
            }

            if (WasUsed(gamepad))
            {
                InputPromptResolver.SetDevice(TypeOf(gamepad));
                return;
            }
        }
    }

    private static bool WasUsed(Gamepad gamepad)
    {
        if (gamepad.leftStick.ReadValue().sqrMagnitude > StickThreshold * StickThreshold
            || gamepad.rightStick.ReadValue().sqrMagnitude > StickThreshold * StickThreshold)
        {
            return true;
        }

        foreach (InputControl control in gamepad.allControls)
        {
            if (control is ButtonControl button && button.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    private static InputPromptResolver.Device TypeOf(Gamepad gamepad)
    {
        if (gamepad is DualShockGamepad)
        {
            return InputPromptResolver.Device.PlayStation;
        }

        if (gamepad is SwitchProControllerHID)
        {
            return InputPromptResolver.Device.Switch;
        }

        // XInput e qualquer outro controle generico: icones de Xbox.
        return InputPromptResolver.Device.Xbox;
    }
}
