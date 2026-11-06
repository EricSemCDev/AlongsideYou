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

    // Fase secreta em andamento (fica fora da sequencia linear, entao nao tem indice).
    private string _secretSceneName;

    public bool IsSecretPhase => !string.IsNullOrEmpty(_secretSceneName);
    public bool CanRestartCurrentPhase => CurrentPhaseIndex >= 0 || IsSecretPhase;

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

        if (!LoadSceneWithTransition(phaseSequence.phaseSceneNames[index], GameState.Playing))
        {
            return; // ja trocando de cena: ignora, sem mexer no indice
        }

        CurrentPhaseIndex = index;
        _secretSceneName = null;
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

        if (!LoadSceneWithTransition(config.secretSceneName, GameState.Playing))
        {
            return;
        }

        CurrentPhaseIndex = -1; // fora da sequência linear
        _secretSceneName = config.secretSceneName;
    }

    // Centraliza o carregamento de cena (com fade). O estado do jogo so muda quando a
    // cena nova termina de carregar: durante o fade a fase antiga continua no estado em
    // que estava (Complete, Paused...), entao ninguem anda nem pausa nela. Se o
    // SceneTransitionManager nao existir (esquecido de configurar), cai para o
    // carregamento direto, sem fade, em vez de travar o jogo.
    // Devolve false se ja ha uma troca de cena em andamento (o pedido e ignorado).
    private bool LoadSceneWithTransition(string sceneName, GameState stateAfterLoad)
    {
        if (SceneTransitionManager.Instance != null)
        {
            return SceneTransitionManager.Instance.LoadScene(
                sceneName,
                () => GameManager.Instance?.TryTransitionTo(stateAfterLoad));
        }

        SceneManager.LoadScene(sceneName);
        GameManager.Instance?.TryTransitionTo(stateAfterLoad);
        return true;
    }
    // Chamado pelo PhaseCompletionManager ao concluir a fase atual:
    // registra o tempo, checa se algum bloco foi completado dentro
    // do limite (desbloqueando fase secreta), e avança.
    public void CompleteCurrentPhase(float elapsedTime)
    {
        if (IsSecretPhase)
        {
            GoToMainMenu(); // fase secreta nao tem proxima: volta ao menu
            return;
        }

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
        if (IsSecretPhase)
        {
            GoToMainMenu(); // sem isso o indice -1 mandaria o jogador de volta ao Prologo
            return;
        }

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
        if (IsSecretPhase)
        {
            LoadSceneWithTransition(_secretSceneName, GameState.Playing);
            return;
        }

        if (CurrentPhaseIndex < 0)
        {
            return;
        }

        StartPhase(CurrentPhaseIndex);
    }

    public void GoToMainMenu()
    {
        if (!LoadSceneWithTransition(mainMenuSceneName, GameState.Menu))
        {
            return;
        }

        CurrentPhaseIndex = -1;
        _secretSceneName = null;
    }
}