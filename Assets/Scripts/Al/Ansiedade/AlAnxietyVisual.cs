using UnityEngine;

// Escurece o Al progressivamente conforme o estágio de ansiedade sobe:
// branco no Estágio 1, preto no Estágio 5. Só reage ao evento do
// AnxietySystem, sem conhecer nada sobre timer ou regras de alívio.
public class AlAnxietyVisual : MonoBehaviour
{
    [SerializeField] private AnxietySystem anxietySystem;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    private void Awake()
    {
        if (anxietySystem == null)
        {
            anxietySystem = GetComponent<AnxietySystem>();
        }

        if (targetSpriteRenderer == null)
        {
            // Fallback: procura em si mesmo e nos filhos, caso o sprite
            // visível esteja em um GameObject filho (comum quando o
            // objeto raiz só tem colisão/física/scripts).
            targetSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged += UpdateColor;
            UpdateColor(anxietySystem.CurrentStage); // aplica a cor certa desde o início
        }
    }

    private void OnDisable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged -= UpdateColor;
        }
    }

    private void UpdateColor(int stage)
    {
        if (targetSpriteRenderer == null)
        {
            return;
        }

        // Estágio 1 = branco (1,1,1), Estágio 5 = preto (0,0,0),
        // interpolando linearmente entre os dois.
        float t = Mathf.InverseLerp(1, 5, stage);
        float shade = Mathf.Lerp(1f, 0f, t);

        Color current = targetSpriteRenderer.color;
        targetSpriteRenderer.color = new Color(shade, shade, shade, current.a);
    }
}