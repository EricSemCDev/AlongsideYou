using UnityEngine;

// Define um bloco de fases (por índice na PhaseSequence) e o tempo
// limite por fase pra desbloquear a fase secreta correspondente.
[CreateAssetMenu(fileName = "SecretPhaseUnlockConfig", menuName = "Alongside You/Secret Phase Unlock Config")]
public class SecretPhaseUnlockConfig : ScriptableObject
{
    public int secretPhaseId;
    public string secretSceneName;

    [Header("Bloco de fases (índices na PhaseSequence, inclusive)")]
    public int blockStartPhaseIndex;
    public int blockEndPhaseIndex;

    [Header("Cada fase do bloco precisa ser completada abaixo disso")]
    public float timeLimitPerPhase = 60f;
}