using System.Linq;
using System.Text.RegularExpressions;
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

    // Liga sozinho os clipes de Assets/Audio/SFX aos campos do FeedbackSounds, pelo NOME do
    // arquivo: o campo "uiHover" recebe "sfx_ui_hover", "resultCorrect" recebe
    // "sfx_result_correct", e assim por diante. Cria o asset se ainda nao existir e ja
    // ajusta a importacao dos clipes (mono, descomprime ao carregar, Vorbis).
    [MenuItem("Alongside You/Juice/Ligar sons de Assets-Audio-SFX ao FeedbackSounds")]
    private static void WireSounds()
    {
        CreateFeedbackSounds();
        var sounds = AssetDatabase.LoadAssetAtPath<FeedbackSounds>(SoundsPath);

        var clips = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio/SFX" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToList();

        foreach (string path in clips)
        {
            if (AssetImporter.GetAtPath(path) is AudioImporter importer)
            {
                var settings = importer.defaultSampleSettings;
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                settings.quality = 0.7f;
                importer.forceToMono = true;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }
        }

        var serialized = new SerializedObject(sounds);
        var wired = new System.Collections.Generic.List<string>();
        var missing = new System.Collections.Generic.List<string>();

        SerializedProperty property = serialized.GetIterator();
        property.NextVisible(true);

        while (property.NextVisible(false))
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || property.name == "m_Script")
            {
                continue;
            }

            string expected = "sfx_" + Regex.Replace(property.name, "([A-Z])", "_$1").ToLowerInvariant();
            string match = clips.FirstOrDefault(c => System.IO.Path.GetFileNameWithoutExtension(c).ToLowerInvariant() == expected);

            if (match != null)
            {
                property.objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(match);
                wired.Add(expected);
            }
            else if (property.objectReferenceValue == null)
            {
                missing.Add(expected);
            }
        }

        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(sounds);
        AssetDatabase.SaveAssets();

        Debug.Log("FeedbackSounds ligados (" + wired.Count + "): " + string.Join(", ", wired)
            + "\nAinda sem arquivo (" + missing.Count + "): " + string.Join(", ", missing));
        Selection.activeObject = sounds;
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
