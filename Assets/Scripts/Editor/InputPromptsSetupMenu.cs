using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

// Prepara os icones de botao (teclado / mouse / controle) para o TextMeshPro:
// configura a folha de pixel art, fatia cada icone com o nome certo e cria o
// Sprite Asset usado nas falas e nos avisos (<sprite name="kb_space_f">).
// Menu: Alongside You > Tutorial > Gerar Sprite Asset dos icones de botao.
public static class InputPromptsSetupMenu
{
    private const string Folder = "Assets/Art/UI/InputPrompts";
    private const string TexturePath = Folder + "/input_prompts.png";
    private const string NamesPath = Folder + "/input_prompts_names.txt";
    private const string SpriteAssetPath = Folder + "/input_prompts.asset";
    private const int Cell = 16;

    // Posicao de cada icone dentro do texto (em pixels do icone, a 100% de tamanho).
    // O TextMeshPro cria com X = -8 e Y = 8, o que deixa o icone torto e colado nas letras.
    //  - IconBearingX: afastamento horizontal a partir do ponto de escrita.
    //  - IconBearingY: altura do TOPO do icone acima da linha do texto (maior = icone mais alto).
    //  - IconAdvance: espaco que o icone ocupa na linha (largura 16 + folga nas laterais).
    private const float IconBearingX = 1f;
    //    Pode ser mexido pelos menus "Subir / Descer icones", sem editar codigo.
    private const string BearingYKey = "AlongsideYou.InputPrompts.BearingY";
    private const float DefaultBearingY = 16f;
    private static float IconBearingY => EditorPrefs.GetFloat(BearingYKey, DefaultBearingY);
    private const float IconAdvance = 18f;

    [MenuItem("Alongside You/Tutorial/Subir icones no texto (+1)")]
    private static void RaiseIcons()
    {
        NudgeIcons(1f);
    }

    [MenuItem("Alongside You/Tutorial/Descer icones no texto (-1)")]
    private static void LowerIcons()
    {
        NudgeIcons(-1f);
    }

    private static void NudgeIcons(float delta)
    {
        var spriteAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(SpriteAssetPath);

        if (spriteAsset == null)
        {
            EditorUtility.DisplayDialog("Icones de botao", "Gere o Sprite Asset primeiro (menu ao lado).", "OK");
            return;
        }

        EditorPrefs.SetFloat(BearingYKey, IconBearingY + delta);
        ApplyMetrics(spriteAsset);
        Debug.Log("InputPrompts: altura dos icones = " + IconBearingY + " (maior = mais alto).");
    }

    [MenuItem("Alongside You/Tutorial/Ajustar posicao dos icones no texto")]
    private static void AdjustMetricsMenu()
    {
        var spriteAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(SpriteAssetPath);

        if (spriteAsset == null)
        {
            EditorUtility.DisplayDialog("Icones de botao", "Gere o Sprite Asset primeiro (menu ao lado).", "OK");
            return;
        }

        ApplyMetrics(spriteAsset);
        EditorUtility.DisplayDialog("Icones de botao", "Posicao dos icones ajustada.", "OK");
    }

    private static void ApplyMetrics(TMP_SpriteAsset spriteAsset)
    {
        var serialized = new SerializedObject(spriteAsset);
        SerializedProperty glyphs = serialized.FindProperty("m_SpriteGlyphTable");

        for (int i = 0; i < glyphs.arraySize; i++)
        {
            SerializedProperty metrics = glyphs.GetArrayElementAtIndex(i).FindPropertyRelative("m_Metrics");
            metrics.FindPropertyRelative("m_HorizontalBearingX").floatValue = IconBearingX;
            metrics.FindPropertyRelative("m_HorizontalBearingY").floatValue = IconBearingY;
            metrics.FindPropertyRelative("m_HorizontalAdvance").floatValue = IconAdvance;
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
        spriteAsset.UpdateLookupTables();
        EditorUtility.SetDirty(spriteAsset);
        AssetDatabase.SaveAssets();
    }

    // Mesmo nome = mesmo id, assim regerar a folha nao quebra referencias.
    private static GUID StableId(string spriteName)
    {
        using (var md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(spriteName));
            var hex = new StringBuilder();

            foreach (byte b in hash)
            {
                hex.Append(b.ToString("x2"));
            }

            return new GUID(hex.ToString());
        }
    }

    [MenuItem("Alongside You/Tutorial/Gerar Sprite Asset dos icones de botao")]
    private static void Generate()
    {
        var names = AssetDatabase.LoadAssetAtPath<TextAsset>(NamesPath);
        var importer = AssetImporter.GetAtPath(TexturePath) as TextureImporter;

        if (names == null || importer == null)
        {
            EditorUtility.DisplayDialog("Icones de botao", "Nao encontrei " + TexturePath + " ou " + NamesPath + ".", "OK");
            return;
        }

        // A folha tem 16 colunas de 16 px; precisamos da altura para virar as linhas de cabeca para baixo.
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = Cell;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 512;
        importer.SaveAndReimport();

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
        int height = texture.height;

        var rects = new List<SpriteRect>();

        foreach (string rawLine in names.text.Split('\n'))
        {
            string line = rawLine.Trim();

            if (line.Length == 0)
            {
                continue;
            }

            string[] parts = line.Split(',');

            if (parts.Length != 3 || !int.TryParse(parts[1], out int column) || !int.TryParse(parts[2], out int row))
            {
                Debug.LogWarning("InputPrompts: linha ignorada: " + line);
                continue;
            }

            rects.Add(new SpriteRect
            {
                name = parts[0],
                spriteID = StableId(parts[0]),
                rect = new Rect(column * Cell, height - ((row + 1) * Cell), Cell, Cell),
                alignment = SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f)
            });
        }

        // Fatiar pela API oficial do editor de sprites (mais confiavel que mexer no importer).
        var factories = new SpriteDataProviderFactories();
        factories.Init();
        ISpriteEditorDataProvider provider = factories.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();
        provider.SetSpriteRects(rects.ToArray());

        var nameFileIds = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();

        if (nameFileIds != null)
        {
            var pairs = new List<SpriteNameFileIdPair>();

            foreach (SpriteRect spriteRect in rects)
            {
                pairs.Add(new SpriteNameFileIdPair(spriteRect.name, spriteRect.spriteID));
            }

            nameFileIds.SetNameFileIdPairs(pairs);
        }

        provider.Apply();
        importer.SaveAndReimport();
        // O proprio TextMeshPro monta o Sprite Asset a partir da folha fatiada.
        if (AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(SpriteAssetPath) != null)
        {
            AssetDatabase.DeleteAsset(SpriteAssetPath);
        }

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
        bool created = EditorApplication.ExecuteMenuItem("Assets/Create/TextMeshPro/Sprite Asset");
        AssetDatabase.Refresh();

        var spriteAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(SpriteAssetPath);

        if (spriteAsset == null)
        {
            EditorUtility.DisplayDialog(
                "Icones de botao",
                "A folha foi fatiada (" + rects.Count + " icones), mas nao consegui criar o Sprite Asset sozinho.\n\n"
                + "Faca manualmente: selecione input_prompts.png e use Assets > Create > TextMeshPro > Sprite Asset.",
                "OK");
            return;
        }

        ApplyMetrics(spriteAsset);

        Selection.activeObject = spriteAsset;
        Debug.Log("InputPrompts: Sprite Asset criado com " + rects.Count + " icones (menu executado: " + created + ").");
        EditorUtility.DisplayDialog("Icones de botao", rects.Count + " icones prontos em " + SpriteAssetPath + ".", "OK");
    }
}
