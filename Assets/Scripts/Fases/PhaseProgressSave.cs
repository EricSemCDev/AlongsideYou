using UnityEngine;

// Persistência simples via PlayerPrefs — guarda o melhor tempo de
// cada fase e quais fases secretas já foram desbloqueadas. Sobrevive
// fechar/abrir o jogo (decisão revisada: o GDD original excluía save,
// isso muda essa decisão — registrar na tabela de Decisões).
public static class PhaseProgressSave
{
    private const string CompletedTimeKeyPrefix = "PhaseCompletedTime_";
    private const string SecretUnlockedKeyPrefix = "SecretPhaseUnlocked_";

    public static void RecordPhaseCompletion(int phaseIndex, float elapsedTime)
    {
        string key = CompletedTimeKeyPrefix + phaseIndex;

        if (!PlayerPrefs.HasKey(key) || elapsedTime < PlayerPrefs.GetFloat(key))
        {
            PlayerPrefs.SetFloat(key, elapsedTime);
        }

        PlayerPrefs.Save();
    }

    public static bool HasCompletedPhase(int phaseIndex)
    {
        return PlayerPrefs.HasKey(CompletedTimeKeyPrefix + phaseIndex);
    }

    public static float GetBestTime(int phaseIndex)
    {
        return PlayerPrefs.GetFloat(CompletedTimeKeyPrefix + phaseIndex, float.MaxValue);
    }

    public static void UnlockSecretPhase(int secretPhaseId)
    {
        PlayerPrefs.SetInt(SecretUnlockedKeyPrefix + secretPhaseId, 1);
        PlayerPrefs.Save();
    }

    public static bool IsSecretPhaseUnlocked(int secretPhaseId)
    {
        return PlayerPrefs.GetInt(SecretUnlockedKeyPrefix + secretPhaseId, 0) == 1;
    }
}