using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Ferramenta de debug temporária para alternar o GameState manualmente
/// enquanto o menu principal (#26) e o menu de pausa (#33) não existem.
/// Remover ou desabilitar quando essas issues forem implementadas.
/// </summary>
public class DebugStateToggle : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TryStartOrResume();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TryPause();
        }
    }

    private void TryStartOrResume()
    {
        var state = GameManager.Instance.CurrentState;

        if (state == GameState.Menu || state == GameState.GameOver || state == GameState.Complete)
        {
            GameManager.Instance.TryTransitionTo(GameState.Playing);
        }
        else if (state == GameState.Paused)
        {
            GameManager.Instance.TryTransitionTo(GameState.Playing);
        }
    }

    private void TryPause()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.TryTransitionTo(GameState.Paused);
        }
    }
}