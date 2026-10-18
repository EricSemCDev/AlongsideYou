using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Faz os menus (principal e de pausa) funcionarem com controle e teclado, sem
// precisar configurar nada em cada cena. Se cria sozinho ao abrir o jogo.
//
// 1. Troca o modulo de entrada da UI (o antigo "Standalone", que le o sistema
//    antigo de input) pelo InputSystemUIInputModule, que entende qualquer
//    controle (Xbox, PlayStation, Switch): analogico / direcional navega,
//    botao de baixo (A / Cruz) confirma, botao de leste (B / Bola) cancela.
// 2. Quando um menu aparece (ou o jogador volta a usar controle / setas) e nada
//    esta selecionado, seleciona o primeiro botao, para a navegacao ter de onde
//    comecar. Com o mouse nada e forcado.
public class MenuGamepadNavigation : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        if (FindFirstObjectByType<MenuGamepadNavigation>() != null)
        {
            return;
        }

        var go = new GameObject("MenuGamepadNavigation");
        go.AddComponent<MenuGamepadNavigation>();
        DontDestroyOnLoad(go);
    }

    private void Update()
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            return;
        }

        EnsureInputSystemModule(eventSystem);
        EnsureSelection(eventSystem);
    }

    private static void EnsureInputSystemModule(EventSystem eventSystem)
    {
        if (eventSystem.GetComponent<InputSystemUIInputModule>() != null)
        {
            return;
        }

        foreach (BaseInputModule module in eventSystem.GetComponents<BaseInputModule>())
        {
            Destroy(module);
        }

        var uiModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        uiModule.AssignDefaultActions();
    }

    private static void EnsureSelection(EventSystem eventSystem)
    {
        GameObject selected = eventSystem.currentSelectedGameObject;
        bool hasValidSelection = selected != null && selected.activeInHierarchy;

        if (hasValidSelection)
        {
            return;
        }

        if (!InputPromptResolver.IsGamepad && !KeyboardNavigationPressed())
        {
            return; // usando o mouse: nao forca selecao
        }

        Selectable first = FindFirstSelectable();

        if (first != null)
        {
            eventSystem.SetSelectedGameObject(first.gameObject);
        }
    }

    private static bool KeyboardNavigationPressed()
    {
        Keyboard keyboard = Keyboard.current;

        return keyboard != null
            && (keyboard.upArrowKey.wasPressedThisFrame
                || keyboard.downArrowKey.wasPressedThisFrame
                || keyboard.leftArrowKey.wasPressedThisFrame
                || keyboard.rightArrowKey.wasPressedThisFrame
                || keyboard.wKey.wasPressedThisFrame
                || keyboard.sKey.wasPressedThisFrame
                || keyboard.enterKey.wasPressedThisFrame);
    }

    // O botao mais "de cima" do canvas que esta na frente (menu de pausa sobre o jogo, etc.).
    private static Selectable FindFirstSelectable()
    {
        Selectable best = null;
        int bestOrder = int.MinValue;
        float bestY = float.MinValue;
        float bestX = float.MaxValue;

        foreach (Selectable candidate in Selectable.allSelectablesArray)
        {
            if (candidate == null
                || !candidate.isActiveAndEnabled
                || !candidate.interactable
                || candidate.navigation.mode == Navigation.Mode.None)
            {
                continue;
            }

            Canvas canvas = candidate.GetComponentInParent<Canvas>();
            int order = canvas != null ? canvas.rootCanvas.sortingOrder : 0;
            Vector3 position = candidate.transform.position;

            bool better = order > bestOrder
                || (order == bestOrder && position.y > bestY + 0.01f)
                || (order == bestOrder && Mathf.Abs(position.y - bestY) <= 0.01f && position.x < bestX);

            if (better)
            {
                best = candidate;
                bestOrder = order;
                bestY = position.y;
                bestX = position.x;
            }
        }

        return best;
    }
}
