using UnityEngine;
using UnityEngine.Rendering.Universal;

// Encolhe a luz pessoal do Al conforme o estágio de ansiedade sobe,
// reforçando visualmente a necessidade de voltar perto de uma fonte
// de luz de segurança. Só reage ao evento — não sabe nada sobre
// timer ou regras de escalada/recuperação.
public class AlAnxietyLightEffect : MonoBehaviour
{
    [SerializeField] private AnxietySystem anxietySystem;
    [SerializeField] private Light2D personalLight;
    [SerializeField] private float stage1Radius = 1.5f;
    [SerializeField] private float stage5Radius = 0.1f; // quase zero, nunca exatamente 0

    private void Awake()
    {
        if (anxietySystem == null)
        {
            anxietySystem = GetComponent<AnxietySystem>();
        }
    }

    private void OnEnable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged += UpdateRadius;
            UpdateRadius(anxietySystem.CurrentStage);
        }
    }

    private void OnDisable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged -= UpdateRadius;
        }
    }

    private void UpdateRadius(int stage)
    {
        if (personalLight == null)
        {
            return;
        }

        float t = Mathf.InverseLerp(1, 5, stage);
        float radius = Mathf.Lerp(stage1Radius, stage5Radius, t);

        personalLight.pointLightOuterRadius = radius;
        personalLight.pointLightInnerRadius = radius * 0.5f;
    }
}