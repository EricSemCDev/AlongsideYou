using UnityEditor;
using UnityEngine;

// Atalhos do "suco" (feedback). Menu: Alongside You > Juice.
public static class JuiceMenu
{
    private const string SoundsPath = "Assets/Resources/FeedbackSounds.asset";

    [MenuItem("Alongside You/Juice/Criar FeedbackSounds (sons de feedback)")]
    private static void CreateFeedbackSounds()
    {
        var existing = AssetDatabase.LoadAssetAtPath<FeedbackSounds>(SoundsPath);

        if (existing != null)
        {
            Selection.activeObject = existing;
            EditorGUIUtility.PingObject(existing);
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        var sounds = ScriptableObject.CreateInstance<FeedbackSounds>();
        AssetDatabase.CreateAsset(sounds, SoundsPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = sounds;
        EditorGUIUtility.PingObject(sounds);
    }

    [MenuItem("Alongside You/Juice/Testar tremor de camera (em Play Mode)")]
    private static void TestShake()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Tremor", "Entre em Play Mode para testar.", "OK");
            return;
        }

        CameraShake.Shake(0.15f, 0.3f);
    }
}
