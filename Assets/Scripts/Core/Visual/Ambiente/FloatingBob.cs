using UnityEngine;

// Faz um objeto flutuar suavemente para cima e para baixo (senoide).
// Funciona em qualquer coisa: Fagulha de Luz, itens, operadores, etc.
// Se o sprite estiver em um filho, mova so o filho (campo Visual) e o
// collider do objeto raiz fica parado. Se nao houver filho, move o proprio
// objeto.
public class FloatingBob : MonoBehaviour
{
    [Tooltip("O que se move. Se vazio: o SpriteRenderer filho, ou este objeto.")]
    [SerializeField] private Transform visual;

    [Header("Motion")]
    [Tooltip("Distancia maxima para cima e para baixo (em unidades).")]
    [SerializeField] private float amplitude = 0.15f;
    [Tooltip("Ciclos completos (sobe + desce) por segundo.")]
    [SerializeField] private float frequency = 0.6f;
    [Tooltip("Cada objeto comeca em um ponto diferente, para nao subirem juntos.")]
    [SerializeField] private bool randomizeStart = true;

    [Header("Pixel Art")]
    [Tooltip("Arredonda o movimento para a grade de pixels da arte (evita tremer).")]
    [SerializeField] private bool snapToPixels = true;
    [SerializeField] private float pixelsPerUnit = 16f;

    private Vector3 _baseLocalPosition;
    private float _phase;

    private void Awake()
    {
        if (visual == null)
        {
            SpriteRenderer childRenderer = GetComponentInChildren<SpriteRenderer>();
            visual = childRenderer != null ? childRenderer.transform : transform;
        }

        _baseLocalPosition = visual.localPosition;
        _phase = randomizeStart ? Random.value * Mathf.PI * 2f : 0f;
    }

    private void OnDisable()
    {
        if (visual != null)
        {
            visual.localPosition = _baseLocalPosition;
        }
    }

    private void Update()
    {
        float offset = Mathf.Sin((Time.time * frequency * Mathf.PI * 2f) + _phase) * amplitude;

        if (snapToPixels && pixelsPerUnit > 0f)
        {
            offset = Mathf.Round(offset * pixelsPerUnit) / pixelsPerUnit;
        }

        visual.localPosition = _baseLocalPosition + new Vector3(0f, offset, 0f);
    }
}
