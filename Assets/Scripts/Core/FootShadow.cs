using UnityEngine;

// Sombra oval sob os pes de um personagem. Cria o sprite por codigo (elipse
// em pixel art, sem PNG), fica logo atras do personagem na ordem de desenho
// e acompanha o Y-sorting. Use no Al e no Finn (no Finn, aumente
// verticalOffset para ele parecer flutuar acima do chao).
public class FootShadow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [Tooltip("Opcional. Se vazio, usa o DepthSorter do mesmo objeto (ou a base do sprite).")]
    [SerializeField] private DepthSorter depthSorter;

    [Header("Shape (in sprite pixels)")]
    [SerializeField] private int widthPixels = 14;
    [SerializeField] private int heightPixels = 6;
    [SerializeField] private float pixelsPerUnit = 16f;

    [Header("Position")]
    [Tooltip("Ligue para objetos que flutuam (ex: Fagulha): a sombra fica na posicao do objeto raiz, em vez de seguir a base do sprite. Ajuste o Vertical Offset para ficar embaixo.")]
    [SerializeField] private bool lockToRootPosition;

    [Header("Look")]
    [SerializeField] private Color color = new Color(0f, 0f, 0f, 0.45f);
    [Tooltip("Mais negativo = sombra mais abaixo dos pes (use no Finn para parecer flutuando).")]
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float horizontalOffset = 0f;

    private SpriteRenderer _shadowRenderer;
    private Transform _shadowTransform;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (depthSorter == null)
        {
            depthSorter = GetComponent<DepthSorter>();
        }

        BuildShadow();
    }

    private void OnValidate()
    {
        // Permite ajustar cor, tamanho e offset em Play Mode.
        if (_shadowRenderer != null)
        {
            RebuildSprite();
        }
    }

    private void BuildShadow()
    {
        var go = new GameObject("FootShadow");
        _shadowTransform = go.transform;
        _shadowTransform.SetParent(transform, false);

        _shadowRenderer = go.AddComponent<SpriteRenderer>();

        // Mesmo material do personagem: a sombra tambem passa pelas luzes 2D.
        if (targetRenderer != null)
        {
            _shadowRenderer.sharedMaterial = targetRenderer.sharedMaterial;
        }

        RebuildSprite();
    }

    private void RebuildSprite()
    {
        int w = Mathf.Max(2, widthPixels);
        int h = Mathf.Max(2, heightPixels);

        var texture = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        var pixels = new Color[w * h];
        float cx = (w - 1) / 2f;
        float cy = (h - 1) / 2f;
        float rx = w / 2f;
        float ry = h / 2f;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = (x - cx) / rx;
                float dy = (y - cy) / ry;
                float d = (dx * dx) + (dy * dy);

                Color c = color;

                if (d > 1f)
                {
                    c.a = 0f; // fora da elipse
                }
                else if (d > 0.55f)
                {
                    c.a *= 0.5f; // borda mais clara, em um unico degrau (pixel art)
                }

                pixels[(y * w) + x] = c;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        _shadowRenderer.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, w, h),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );
    }

    private void LateUpdate()
    {
        if (_shadowRenderer == null || targetRenderer == null)
        {
            return;
        }

        // Base do personagem: ponto do Y-sorting se existir, senao o fundo do sprite.
        Vector3 feet;

        if (lockToRootPosition)
        {
            feet = transform.position;
        }
        else if (depthSorter != null)
        {
            feet = depthSorter.SortPoint;
        }
        else
        {
            feet = new Vector3(targetRenderer.bounds.center.x, targetRenderer.bounds.min.y, transform.position.z);
        }

        Vector3 position = feet + new Vector3(horizontalOffset, verticalOffset, 0f);
        _shadowTransform.position = position;

        _shadowRenderer.sortingLayerID = targetRenderer.sortingLayerID;
        _shadowRenderer.sortingOrder = targetRenderer.sortingOrder - 1;
    }
}
