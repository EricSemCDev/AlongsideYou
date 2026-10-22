using UnityEngine;

// Fade de oclusao: quando o Al ou o Finn esta ATRAS deste objeto (mais
// acima na tela) e sobreposto ao sprite dele, o objeto fica translucido
// para o jogador nao sumir atras dele. Volta ao normal ao sair de tras.
// Funciona em qualquer objeto com SpriteRenderer (arvores, pedras...).
public class OcclusionFader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Fade")]
    [Range(0f, 1f)] [SerializeField] private float fadedAlpha = 0.4f;
    [Tooltip("Unidades de alpha por segundo.")]
    [SerializeField] private float fadeSpeed = 4f;

    [Header("Detection")]
    [Tooltip("Aumenta (ou diminui, se negativo) a area de sobreposicao do sprite.")]
    [SerializeField] private Vector2 padding = Vector2.zero;

    private DepthSorter _ownSorter;
    private Transform _al;
    private Transform _finn;
    private DepthSorter _alSorter;
    private DepthSorter _finnSorter;
    private SpriteRenderer _alRenderer;
    private SpriteRenderer _finnRenderer;
    private float _currentAlpha = 1f;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        _ownSorter = GetComponent<DepthSorter>();
    }

    // Start (e nao Awake) porque precisa que os jogadores ja tenham acordado.
    private void Start()
    {
        AlMovement al = FindFirstObjectByType<AlMovement>();

        if (al != null)
        {
            _al = al.transform;
            _alSorter = al.GetComponent<DepthSorter>();
            _alRenderer = al.GetComponentInChildren<SpriteRenderer>();
        }

        FinnMovement finn = FindFirstObjectByType<FinnMovement>();

        if (finn != null)
        {
            _finn = finn.transform;
            _finnSorter = finn.GetComponent<DepthSorter>();
            _finnRenderer = finn.GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (targetRenderer == null)
        {
            return;
        }

        bool occluding = IsOccluding(_al, _alSorter, _alRenderer)
            || IsOccluding(_finn, _finnSorter, _finnRenderer);

        float targetAlpha = occluding ? fadedAlpha : 1f;
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        Color color = targetRenderer.color;

        if (!Mathf.Approximately(color.a, _currentAlpha))
        {
            color.a = _currentAlpha;
            targetRenderer.color = color;
        }
    }

    // O jogador so "esta atras" se estiver mais acima que a base deste
    // objeto E seu sprite se sobrepoe ao sprite deste objeto.
    private bool IsOccluding(Transform player, DepthSorter playerSorter, SpriteRenderer playerRenderer)
    {
        if (player == null || playerRenderer == null)
        {
            return false;
        }

        float playerSortY = playerSorter != null ? playerSorter.SortY : player.position.y;
        float ownSortY = _ownSorter != null ? _ownSorter.SortY : transform.position.y;

        if (playerSortY <= ownSortY)
        {
            return false; // esta na frente, nada a fazer
        }

        Bounds own = targetRenderer.bounds;
        Bounds other = playerRenderer.bounds;

        bool overlapX = other.min.x < own.max.x + padding.x && other.max.x > own.min.x - padding.x;
        bool overlapY = other.min.y < own.max.y + padding.y && other.max.y > own.min.y - padding.y;

        return overlapX && overlapY;
    }

    private void OnDisable()
    {
        // Nao deixa o objeto preso translucido se for desativado no meio do fade.
        if (targetRenderer != null)
        {
            Color color = targetRenderer.color;
            color.a = 1f;
            targetRenderer.color = color;
        }

        _currentAlpha = 1f;
    }
}
