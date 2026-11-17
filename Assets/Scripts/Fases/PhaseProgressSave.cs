using UnityEngine;

// Persistencia simples via PlayerPrefs: guarda as estrelas (0 a 3) de cada fase.
// Sobrevive a fechar/abrir o jogo.
public static class PhaseProgressSave
{
    private const string StarsKeyPrefix = "PhaseStars_";

    // Guarda so se for melhor que a nota anterior.
    public static void RecordPhaseStars(int phaseIndex, int stars)
    {
        if (stars <= GetStars(phaseIndex))
        {
            return;
        }

        PlayerPrefs.SetInt(StarsKeyPrefix + phaseIndex, stars);
        PlayerPrefs.Save();
    }

    public static int GetStars(int phaseIndex)
    {
        return PlayerPrefs.GetInt(StarsKeyPrefix + phaseIndex, 0);
    }
}
