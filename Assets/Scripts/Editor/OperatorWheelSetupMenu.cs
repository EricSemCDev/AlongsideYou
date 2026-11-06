using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Monta o visual novo da roda de operadores no prefab OperatorMenuCanvas:
// fatias em pixel art, aro com miolo e textos na fonte do jogo (simbolo + numero
// como expoente). Reaproveita os mesmos objetos que o OperatorMenuController ja
// usa (fatias e rotulos), entao nenhuma referencia quebra.
// Menu: Alongside You > Operadores > Montar visual da roda de operadores.
//
// Para mudar o TAMANHO da roda depois: selecione o OperatorMenuCanvas e mude a
// Scale do Rect Transform da raiz (hoje 0.01). Tudo escala junto.
public static class OperatorWheelSetupMenu
{
    private const string Folder = "Assets/Art/UI/OperatorWheel";
    private const string PrefabPath = "Assets/Prefabs/Puzzle/OperatorMenuCanvas.prefab";
    private const string FontPath = "Assets/Art/Fonts/PressStart2P-Regular.asset";

    // As imagens tem 72 x 72 pixels. Cada pixel da arte vale 4,25 unidades do canvas
    // (com a escala 0.01 da raiz, a roda fica com ~3 unidades de largura no mundo).
    private const int ImageSize = 72;
    private const float UnitsPerPixel = 4.25f;

    // Posicao do texto de cada fatia (centro do bloco simbolo + numero), em pixels da
    // arte a partir do centro da roda (y para cima). Calculado para ficar DENTRO da fatia,
    // com folga das bordas, do aro e do miolo.
    private static readonly Vector2 PlusPos = new Vector2(11.5f, 15f);
    private static readonly Vector2 MinusPos = new Vector2(13.5f, -18.5f);
    private static readonly Vector2 TimesPos = new Vector2(-12f, -19f);
    private static readonly Vector2 DividePos = new Vector2(-16.5f, 10.5f);

    [MenuItem("Alongside You/Operadores/Montar visual da roda de operadores")]
    private static void Build()
    {
        string[] images = { "wheel_wedge_normal", "wheel_wedge_highlight", "wheel_wedge_empty", "wheel_frame" };

        foreach (string image in images)
        {
            ConfigureSprite(Folder + "/" + image + ".png");
        }

        Sprite normal = LoadSprite("wheel_wedge_normal");
        Sprite highlight = LoadSprite("wheel_wedge_highlight");
        Sprite empty = LoadSprite("wheel_wedge_empty");
        Sprite frame = LoadSprite("wheel_frame");
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

        if (normal == null || highlight == null || empty == null || frame == null)
        {
            EditorUtility.DisplayDialog("Roda de operadores", "Nao achei as imagens em " + Folder + ".", "OK");
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);

        try
        {
            var controller = root.GetComponent<OperatorMenuController>();

            if (controller == null)
            {
                EditorUtility.DisplayDialog("Roda de operadores", "O prefab nao tem OperatorMenuController na raiz.", "OK");
                return;
            }

            var serialized = new SerializedObject(controller);
            float size = ImageSize * UnitsPerPixel;

            var rootRect = (RectTransform)root.transform;
            rootRect.sizeDelta = new Vector2(size, size);

            SetupSlice(serialized, "slicePlus", "countLabelPlus", PlusPos, normal, font, size);
            SetupSlice(serialized, "sliceMinus", "countLabelMinus", MinusPos, normal, font, size);
            SetupSlice(serialized, "sliceTimes", "countLabelTimes", TimesPos, normal, font, size);
            SetupSlice(serialized, "sliceDivide", "countLabelDivide", DividePos, normal, font, size);

            // Aro externo e miolo: atras de tudo.
            Transform frameTransform = rootRect.Find("Frame");

            if (frameTransform == null)
            {
                var frameObject = new GameObject("Frame", typeof(RectTransform), typeof(Image));
                frameObject.transform.SetParent(rootRect, false);
                frameTransform = frameObject.transform;
            }

            var frameRect = (RectTransform)frameTransform;
            frameRect.anchorMin = frameRect.anchorMax = new Vector2(0.5f, 0.5f);
            frameRect.pivot = new Vector2(0.5f, 0.5f);
            frameRect.anchoredPosition = Vector2.zero;
            frameRect.sizeDelta = new Vector2(size, size);
            frameRect.SetSiblingIndex(0);

            var frameImage = frameRect.GetComponent<Image>();
            frameImage.sprite = frame;
            frameImage.type = Image.Type.Simple;
            frameImage.color = Color.white;
            frameImage.raycastTarget = false;

            serialized.FindProperty("wedgeNormal").objectReferenceValue = normal;
            serialized.FindProperty("wedgeHighlight").objectReferenceValue = highlight;
            serialized.FindProperty("wedgeEmpty").objectReferenceValue = empty;
            serialized.FindProperty("highlightScale").floatValue = 1.06f;
            serialized.FindProperty("dimmedColor").colorValue = new Color(0.55f, 0.47f, 0.6f, 1f);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog(
            "Roda de operadores",
            "Visual montado em " + PrefabPath + ".\n\nPara mudar o tamanho, use a Scale do Rect Transform da raiz do prefab (hoje 0.01).",
            "OK");
    }

    private static void SetupSlice(SerializedObject controller, string sliceField, string labelField, Vector2 labelPixels, Sprite sprite, TMP_FontAsset font, float size)
    {
        var slice = controller.FindProperty(sliceField).objectReferenceValue as RectTransform;
        var label = controller.FindProperty(labelField).objectReferenceValue as TextMeshProUGUI;

        if (slice == null || label == null)
        {
            Debug.LogWarning("OperatorWheel: faltou referencia em " + sliceField + " / " + labelField);
            return;
        }

        slice.anchorMin = slice.anchorMax = new Vector2(0.5f, 0.5f);
        slice.pivot = new Vector2(0.5f, 0.5f);
        slice.anchoredPosition = Vector2.zero;
        slice.sizeDelta = new Vector2(size, size);

        var image = slice.GetComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        image.color = Color.white;
        image.raycastTarget = false;

        // O rotulo sai de dentro da fatia (que e girada) e fica direto na raiz, sempre em pe.
        RectTransform labelRect = label.rectTransform;
        labelRect.SetParent(slice.parent, false);
        labelRect.localRotation = Quaternion.identity;
        labelRect.localScale = Vector3.one;
        labelRect.anchorMin = labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.sizeDelta = new Vector2(12f * UnitsPerPixel * 2f, 14f * UnitsPerPixel);
        labelRect.anchoredPosition = labelPixels * UnitsPerPixel;

        if (font != null)
        {
            label.font = font;
        }

        label.fontSize = 8f * UnitsPerPixel; // 8 px da arte = a grade da fonte (nitido)
        label.alignment = TextAlignmentOptions.Center;
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Overflow;
        label.richText = true;
        label.raycastTarget = false;
        label.color = new Color(1f, 0.93f, 0.81f, 1f);
    }

    private static void ConfigureSprite(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16f;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private static Sprite LoadSprite(string fileName)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(Folder + "/" + fileName + ".png");
    }
}
