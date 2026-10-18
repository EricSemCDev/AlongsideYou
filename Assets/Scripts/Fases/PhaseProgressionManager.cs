using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Gerente persistente da progressão entre fases: qual fase estou,
// qual é a próxima, o que fazer quando acabam as fases, e checagem
// de desbloqueio de fases secretas. Separado do GameManager (que só
// cuida da máquina de estados Menu/Playing/...), mantendo cada
// gerente com uma responsabilidade só.
public class PhaseProgressionManager : MonoBehaviour
{
    public static PhaseProgressionManager Instance { get; private set; }

    [SerializeField] private PhaseSequence phaseSequence;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private List<SecretPhaseUnlockConfig> secretUnlockConfigs = new();

    // -1 significa "nenhuma fase em andamento" (ex: ainda no menu)
    public int CurrentPhaseIndex { get; private set; } = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartFirstPhase()
    {
        StartPhase(0);
    }

    public void StartPhase(int index)
    {
        if (phaseSequence == null
            || index < 0
            || index >= phaseSequence.phaseSceneNames.Count)
        {
            Debug.LogWarning($"PhaseProgressionManager: índice de fase inválido ({index}).");
            return;
        }

        CurrentPhaseIndex = index;
        LoadSceneWithTransition(phaseSequence.phaseSceneNames[index]);
        GameManager.Instance?.TryTransitionTo(GameState.Playing);
    }

    // Fases secretas não fazem parte da sequência linear — não usam
    // índice, usam o nome da cena diretamente.
    public void StartSecretPhase(SecretPhaseUnlockConfig config)
    {
        if (config == null || !PhaseProgressSave.IsSecretPhaseUnlocked(config.secretPhaseId))
        {
            Debug.LogWarning("PhaseProgressionManager: fase secreta ainda não desbloqueada.");
            return;
        }

        CurrentPhaseIndex = -1; // fora da sequência linear
        LoadSceneWithTransition(config.secretSceneName);
        GameManager.Instance?.TryTransitionTo(GameState.Playing);
    }

    // Centraliza o carregamento de cena — se o SceneTransitionManager
    // não existir ainda (ex: esquecido de configurar), cai para o
    // carregamento direto, sem fade, em vez de travar o jogo.
    private void LoadSceneWithTransition(string sceneName)
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    // Chamado pelo PhaseCompletionManager ao concluir a fase atual:
    // registra o tempo, checa se algum bloco foi completado dentro
    // do limite (desbloqueando fase secreta), e avança.
    public void CompleteCurrentPhase(float elapsedTime)
    {
        if (CurrentPhaseIndex >= 0)
        {
            PhaseProgressSave.RecordPhaseCompletion(CurrentPhaseIndex, elapsedTime);
            CheckSecretUnlocks(CurrentPhaseIndex);
        }

        GoToNextPhase();
    }

    private void CheckSecretUnlocks(int completedPhaseIndex)
    {
        foreach (SecretPhaseUnlockConfig config in secretUnlockConfigs)
        {
            if (config == null)
            {
                continue;
            }

            if (completedPhaseIndex < config.blockStartPhaseIndex
                || completedPhaseIndex > config.blockEndPhaseIndex)
            {
                continue; // essa fase não pertence a esse bloco
            }

            if (IsBlockCompletedWithinLimit(config))
            {
                PhaseProgressSave.UnlockSecretPhase(config.secretPhaseId);
                Debug.Log($"Fase secreta desbloqueada: {config.secretSceneName}");
            }
        }
    }

    private bool IsBlockCompletedWithinLimit(SecretPhaseUnlockConfig config)
    {
        for (int i = config.blockStartPhaseIndex; i <= config.blockEndPhaseIndex; i++)
        {
            if (!PhaseProgressSave.HasCompletedPhase(i))
            {
                return false;
            }

            if (PhaseProgressSave.GetBestTime(i) > config.timeLimitPerPhase)
            {
                return false;
            }
        }

        return true;
    }

    public void GoToNextPhase()
    {
        int nextIndex = CurrentPhaseIndex + 1;

        if (phaseSequence == null || nextIndex >= phaseSequence.phaseSceneNames.Count)
        {
            Debug.Log("PhaseProgressionManager: não há próxima fase configurada. Voltando ao menu.");
            GoToMainMenu();
            return;
        }

        StartPhase(nextIndex);
    }

    public void RestartCurrentPhase()
    {
        if (CurrentPhaseIndex < 0)
        {
            return;
        }

        StartPhase(CurrentPhaseIndex);
    }

    public void GoToMainMenu()
    {
        CurrentPhaseIndex = -1;
        LoadSceneWithTransition(mainMenuSceneName);
        GameManager.Instance?.TryTransitionTo(GameState.Menu);
    }
}