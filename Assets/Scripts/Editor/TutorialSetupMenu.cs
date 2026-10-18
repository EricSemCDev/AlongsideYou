using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Atalhos de menu para montar o tutorial na cena aberta, sem arrastar tudo a mao.
// Menu: Alongside You > Tutorial.
public static class TutorialSetupMenu
{
    private const string RootName = "Tutorial";
    private const string FinnSheetPath = "Assets/Art/Sprites/Finn/Finn_idle.png";

    [MenuItem("Alongside You/Tutorial/Montar tour do Prologo na cena aberta")]
    private static void BuildPrologueTour()
    {
        if (GameObject.Find(RootName) != null)
        {
            EditorUtility.DisplayDialog(
                "Tutorial",
                "Ja existe um objeto 'Tutorial' nesta cena. Apague ou renomeie ele antes de montar de novo.",
                "OK");
            return;
        }

        var result = FindFirst<ExpectedResultDisplay>();
        var slots = FindAll<NumericSlot>();
        var marker = FindFirst<OperatorSlotMarker>();
        var spark = FindFirst<OperatorSpark>();
        var blocks = FindAll<DadoBlock>();
        var fagulha = FindFirst<FagulhaDeLuz>();
        var thorn = FindFirst<ThornBarrier>();
        var al = FindFirst<AlMovement>();
        var finn = FindFirst<FinnMovement>();

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Montar tutorial");

        root.AddComponent<TutorialDialogueUI>();

        // O zoom precisa mirar UM objeto so (senao nao da para saber qual e o assunto).
        Transform slotFocus = slots.OrderBy(x => x.transform.position.x).Select(x => x.transform).FirstOrDefault();
        Transform blockFocus = blocks.OrderBy(x => x.transform.position.x).Select(x => x.transform).FirstOrDefault();

        var tourObject = new GameObject("Tour_Prologo");
        tourObject.transform.SetParent(root.transform, false);
        var player = tourObject.AddComponent<TutorialSequencePlayer>();

        Sprite portrait = LoadFinnPortrait();

        var steps = new List<StepData>
        {
            new StepData("Oi! Eu sou o <color=#FFB84D>Finn</color>, a chama que segue o seu mouse. Vou te mostrar como as coisas funcionam por aqui.", finn != null ? finn.transform : null, 4f),
            new StepData("Esse é o <color=#6FE3E3>Al</color>. Ele se assusta fácil e fica mais ansioso com o tempo, então vamos ajudá-lo a chegar longe!", al != null ? al.transform : null, 4f),
            new StepData("Nosso objetivo é chegar naquela <color=#FFE14A>Fagulha de Luz</color>, lá do outro lado.", fagulha != null ? fagulha.transform : null, 4.5f),
            new StepData("Mas uma barreira de espinhos fecha o caminho. Ela só abre se a gente resolver a conta certa.", thorn != null ? thorn.transform : null, 4.5f),
            new StepData("Esse número na tocha é o <color=#FF9AFF>resultado esperado</color>. É ele que a nossa conta precisa dar.", result != null ? result.transform : null, 4f),
            new StepData("Esse é um <color=#6FA8FF>espaço numérico</color>. É onde os blocos com número se encaixam.", slotFocus, 4f),
            new StepData("E aqui fica o espaço do <color=#B06CFF>operador</color>: ele decide se a conta soma, subtrai, multiplica ou divide.", marker != null ? marker.transform : null, 4f),
            new StepData("Eu cuido das faíscas de operador. Perto de uma tocha, segure {finn_wheel} para abrir a roda, aponte para o operador e solte para acender.", spark != null ? spark.transform : null, 4f),
            new StepData("O Al carrega os blocos: chegue perto e aperte {interact} para pegar, e de novo para encaixar num espaço.", blockFocus, 4f),
            new StepData("O Al anda com {move}. Eu sigo o mouse: segure {finn_follow} e eu vou até lá!", null, 0f, "O Al anda com o analógico de um controle, e eu me movo com o do outro: é só inclinar e eu vou!"),
            new StepData("Agora é com a gente: encaixe o bloco, escolha o operador e abra caminho até a Fagulha!", null, 0f)
        };

        var serialized = new SerializedObject(player);
        serialized.FindProperty("defaultPortrait").objectReferenceValue = portrait;

        SerializedProperty list = serialized.FindProperty("steps");
        list.arraySize = steps.Count;

        for (int i = 0; i < steps.Count; i++)
        {
            SerializedProperty item = list.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("text").stringValue = steps[i].Text;
            item.FindPropertyRelative("gamepadText").stringValue = steps[i].GamepadText;
            item.FindPropertyRelative("focusTarget").objectReferenceValue = steps[i].Target;
            item.FindPropertyRelative("zoomSize").floatValue = steps[i].Zoom;
        }

        serialized.FindProperty("dialogue").objectReferenceValue = root.GetComponent<TutorialDialogueUI>();
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(root.scene);
        Selection.activeGameObject = tourObject;

        var missing = new List<string>();
        if (result == null) missing.Add("ExpectedResultDisplay (tocha de resultado)");
        if (slots.Count == 0) missing.Add("NumericSlot");
        if (marker == null) missing.Add("OperatorSlotMarker");
        if (spark == null) missing.Add("OperatorSpark");
        if (blocks.Count == 0) missing.Add("DadoBlock");
        if (fagulha == null) missing.Add("FagulhaDeLuz");
        if (thorn == null) missing.Add("ThornBarrier");
        if (al == null) missing.Add("AlMovement");
        if (finn == null) missing.Add("FinnMovement");

        string message = "Tour montado em 'Tutorial > Tour_Prologo'. Salve a cena (Ctrl+S).\n\nAs falas e os alvos de camera podem ser editados no Inspector.";

        if (portrait == null)
        {
            message += "\n\nNao achei o sprite 'Finn_idle_0'. Arraste um retrato em 'Default Portrait'.";
        }

        if (missing.Count > 0)
        {
            message += "\n\nNao encontrei na cena (falas sem alvo de camera): " + string.Join(", ", missing) + ".";
        }

        EditorUtility.DisplayDialog("Tutorial", message, "OK");
    }

    [MenuItem("Alongside You/Tutorial/Adicionar avisos de botao (pegar / encaixar)")]
    private static void AddPickupPrompts()
    {
        int added = 0;

        foreach (DadoBlock block in FindAll<DadoBlock>())
        {
            if (HasPrompt(block.gameObject, ButtonPromptWorld.ShowWhen.NearPlayerNotCarrying))
            {
                continue;
            }

            AddPrompt(block.gameObject, ButtonPromptWorld.ShowWhen.NearPlayerNotCarrying, new Vector2(0f, 1f));
            added++;
        }

        foreach (NumericSlot slot in FindAll<NumericSlot>())
        {
            if (HasPrompt(slot.gameObject, ButtonPromptWorld.ShowWhen.NearPlayerCarrying))
            {
                continue;
            }

            AddPrompt(slot.gameObject, ButtonPromptWorld.ShowWhen.NearPlayerCarrying, new Vector2(0f, 1.3f));
            added++;
        }

        FinishPrompts(added);
    }

    [MenuItem("Alongside You/Tutorial/Adicionar aviso de girar nos blocos (fase 2)")]
    private static void AddRotatePrompts()
    {
        int added = 0;

        foreach (DadoBlock block in FindAll<DadoBlock>())
        {
            if (HasPrompt(block.gameObject, ButtonPromptWorld.ShowWhen.BlockIsHeld))
            {
                continue;
            }

            AddPrompt(block.gameObject, ButtonPromptWorld.ShowWhen.BlockIsHeld, new Vector2(0f, 1.5f));
            added++;
        }

        FinishPrompts(added);
    }

    // --- Ajudantes ---------------------------------------------------------

    private struct StepData
    {
        public string Text;
        public Transform Target;
        public float Zoom;
        public string GamepadText;

        public StepData(string text, Transform target, float zoom, string gamepadText = "")
        {
            Text = text;
            Target = target;
            Zoom = zoom;
            GamepadText = gamepadText;
        }
    }

    private static T FindFirst<T>() where T : Object
    {
        return Object.FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
    }

    private static List<T> FindAll<T>() where T : Object
    {
        return Object.FindObjectsByType<T>(FindObjectsSortMode.None).ToList();
    }

    private static Sprite LoadFinnPortrait()
    {
        return AssetDatabase.LoadAllAssetsAtPath(FinnSheetPath)
            .OfType<Sprite>()
            .FirstOrDefault(s => s.name == "Finn_idle_0");
    }

    private static bool HasPrompt(GameObject target, ButtonPromptWorld.ShowWhen mode)
    {
        foreach (ButtonPromptWorld prompt in target.GetComponents<ButtonPromptWorld>())
        {
            var so = new SerializedObject(prompt);

            if (so.FindProperty("showWhen").enumValueIndex == (int)mode)
            {
                return true;
            }
        }

        return false;
    }

    private static void AddPrompt(GameObject target, ButtonPromptWorld.ShowWhen mode, Vector2 offset)
    {
        var prompt = Undo.AddComponent<ButtonPromptWorld>(target);
        var so = new SerializedObject(prompt);
        so.FindProperty("showWhen").enumValueIndex = (int)mode;
        so.FindProperty("offset").vector2Value = offset;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void FinishPrompts(int added)
    {
        if (added > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        EditorUtility.DisplayDialog(
            "Tutorial",
            added > 0
                ? added + " avisos adicionados. Salve a cena (Ctrl+S)."
                : "Nada novo para adicionar (os avisos ja existem).",
            "OK");
    }
}
