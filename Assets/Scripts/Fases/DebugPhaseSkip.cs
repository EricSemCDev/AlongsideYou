using UnityEngine;
using UnityEngine.InputSystem;

// Atalho de teste: F9 pula para a proxima fase. Funciona no Editor e na build.
// Se cria sozinho ao abrir o jogo (nao precisa colocar em cena).
public class DebugPhaseSkip : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        var go = new GameObject("DebugPhaseSkip");
        go.AddComponent<DebugPhaseSkip>();
        DontDestroyOnLoad(go);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f9Key.wasPressedThisFrame)
        {
            PhaseProgressionManager.Instance?.GoToNextPhase();
        }
    }
}
