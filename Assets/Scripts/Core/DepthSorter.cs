using UnityEngine;

// Y-sorting: quem esta mais embaixo na tela fica na frente. Calcula o
// sortingOrder do SpriteRenderer a partir da altura (Y) de um "ponto de
// ordenacao" - normalmente a base/os pes do objeto, e nao o pivot (que nos
// sprites do jogo fica no centro). Use no Al, no Finn e em qualquer objeto
// que possa ficar na frente ou atras de alguem (arvores, pedras...).
public class DepthSorter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Sort Point")]
    [Tooltip("Posicao (local) da base do objeto. Selecione o objeto para ver o gizmo na Scene.")]
    [SerializeField] private Vector2 sortPointOffset;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "Players";
    [Tooltip("Somado ao valor calculado. Use para desempatar objetos na mesma altura.")]
    [SerializeField] private int baseOrder;
    [Tooltip("Quantos 'degraus' de sortingOrder por unidade de Y.")]
    [SerializeField] private float precision = 100f;

    [Header("Performance")]
    [Tooltip("Marque em objetos que nunca se mexem (arvores): calcula uma vez so.")]
    [SerializeField] private bool isStatic;

    // Ponto de ordenacao em coordenadas de mundo.
    public Vector3 SortPoint => transform.TransformPoint(sortPointOffset);

    public float SortY => SortPoint.y;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        ApplySorting();
    }

    private void LateUpdate()
    {
        if (!isStatic)
        {
            ApplySorting();
        }
    }

    private void ApplySorting()
    {
        if (targetRenderer == null)
        {
            return;
        }

        targetRenderer.sortingLayerID = SortingLayer.NameToID(sortingLayerName);

        int order = baseOrder - Mathf.RoundToInt(SortY * precision);
        targetRenderer.sortingOrder = Mathf.Clamp(order, short.MinValue, short.MaxValue);
    }

    // Chamado pela Unity ao adicionar o componente: ja posiciona o ponto
    // na base do sprite, que costuma ser o certo.
    private void Reset()
    {
        targetRenderer = GetComponentInChildren<SpriteRenderer>();
        SnapOffsetToSpriteBottom();
    }

    [ContextMenu("Snap Offset To Sprite Bottom")]
    private void SnapOffsetToSpriteBottom()
    {
        if (targetRenderer == null)
        {
            return;
        }

        Bounds bounds = targetRenderer.bounds;
        Vector3 bottom = new Vector3(bounds.center.x, bounds.min.y, transform.position.z);
        Vector3 local = transform.InverseTransformPoint(bottom);
        sortPointOffset = new Vector2(local.x, local.y);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 point = SortPoint;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(point, 0.06f);
        Gizmos.DrawLine(point + Vector3.left * 0.4f, point + Vector3.right * 0.4f);
    }
}
