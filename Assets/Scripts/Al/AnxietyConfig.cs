using UnityEngine;

[CreateAssetMenu(fileName = "AnxietyConfig", menuName = "Alongside You/Anxiety Config")]
public class AnxietyConfig : ScriptableObject
{
    [Header("Duração de cada estágio (segundos até avançar para o próximo)")]
    public float stage1Duration = 20f;
    public float stage2Duration = 20f;
    public float stage3Duration = 20f;
    public float stage4Duration = 20f;
    // Estágio 5 é o final: sem avanço automático, sem duração configurável.
}