using UnityEngine;

[CreateAssetMenu(fileName = "AnxietyConfig", menuName = "Alongside You/Anxiety Config")]
public class AnxietyConfig : ScriptableObject
{
    [Header("Tempo em cada zona até mudar de estágio (segundos)")]
    public float timeInDarknessToEscalate = 5f;
    public float timeInLightToRecover = 3f;

    [Header("Multiplicador de velocidade do Al por estágio (1 = normal)")]
    public float stage1SpeedMultiplier = 1f;
    public float stage2SpeedMultiplier = 0.85f;
    public float stage3SpeedMultiplier = 0.65f;
    public float stage4SpeedMultiplier = 0.4f;
    public float stage5SpeedMultiplier = 0f; // sem efeito real: Estágio 5 trava o movimento por completo
}