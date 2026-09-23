using UnityEngine;

[CreateAssetMenu(fileName = "AnxietyConfig", menuName = "Alongside You/Anxiety Config")]
public class AnxietyConfig : ScriptableObject
{
    [Header("Duração de cada estágio (segundos até avançar para o próximo)")]
    public float stage1Duration = 20f;
    public float stage2Duration = 20f;
    public float stage3Duration = 20f;
    public float stage4Duration = 20f;

    [Header("Multiplicador de velocidade do Al por estágio (1 = normal)")]
    public float stage1SpeedMultiplier = 1f;
    public float stage2SpeedMultiplier = 0.85f;
    public float stage3SpeedMultiplier = 0.65f;
    public float stage4SpeedMultiplier = 0.4f;
    public float stage5SpeedMultiplier = 0f;
}