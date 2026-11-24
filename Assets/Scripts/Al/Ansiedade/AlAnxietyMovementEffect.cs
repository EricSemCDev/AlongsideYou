using UnityEngine;

// Ajusta a velocidade do Al conforme o estágio de ansiedade sobe, e
// trava o movimento por completo no Estágio 5. Só reage ao evento do
// AnxietySystem — não conhece timer nem regras de alívio.
// Interact (pegar/largar/confirmar) continua funcionando mesmo com o
// movimento travado: é responsabilidade do AlBlockCarrier, script
// separado, evitando um soft-lock enquanto o resgate do Finn não existe.
public class AlAnxietyMovementEffect : MonoBehaviour
{
    [SerializeField] private AnxietySystem anxietySystem;
    [SerializeField] private AlMovement alMovement;
    [SerializeField] private AnxietyConfig config;

    private void Awake()
    {
        if (anxietySystem == null)
        {
            anxietySystem = GetComponent<AnxietySystem>();
        }

        if (alMovement == null)
        {
            alMovement = GetComponent<AlMovement>();
        }
    }

    private void OnEnable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged += ApplyStageEffect;
            ApplyStageEffect(anxietySystem.CurrentStage);
        }
    }

    private void OnDisable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged -= ApplyStageEffect;
        }
    }

    private void ApplyStageEffect(int stage)
    {
        if (alMovement == null || config == null)
        {
            return;
        }

        alMovement.MovementDisabled = stage >= 5;
        alMovement.SpeedMultiplier = GetSpeedMultiplier(stage);
    }

    private float GetSpeedMultiplier(int stage)
    {
        switch (stage)
        {
            case 1:
                return config.stage1SpeedMultiplier;
            case 2:
                return config.stage2SpeedMultiplier;
            case 3:
                return config.stage3SpeedMultiplier;
            case 4:
                return config.stage4SpeedMultiplier;
            case 5:
                return config.stage5SpeedMultiplier;
            default:
                return 1f;
        }
    }
}