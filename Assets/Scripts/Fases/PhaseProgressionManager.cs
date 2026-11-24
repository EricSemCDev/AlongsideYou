using UnityEngine;
using UnityEngine.SceneManagement;

// Gerente persistente da progressao entre fases: qual fase estou, qual e a proxima e o
// que fazer quando acabam as fases. Separado do GameManager (que so cuida da maquina de
// estados Menu/Playing/...), mantendo cada gerente com uma responsabilidade so.
public class PhaseProgressionManager : MonoBehaviour
{
    public static PhaseProgressionManager Instance { get; private set; }

    [SerializeField] private PhaseSequence phaseSequence;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // -1 significa "nenhuma fase em andamento" (ex: ainda no menu)
    public int CurrentPhaseIndex { get; private set; } = -1;

    public bool CanRestartCurrentPhase => CurrentPhaseIndex >= 0;

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

    // Chamado pelo PhaseCompletionManager ao concluir a fase atual: guarda as estrelas. Quem avanca
    // para a proxima fase e a tela de resultado (PhaseResultScreen), pelos botoes.
    public void CompleteCurrentPhase(int stars)
    {
        if (CurrentPhaseIndex < 0)
        {
            return; // nenhuma fase em andamento
        }

        Debug.Log($"Fase {CurrentPhaseIndex}: {stars} estrela(s)");
        PhaseProgressSave.RecordPhaseStars(CurrentPhaseIndex, stars);
    }

    public void GoToNextPhase()
    {
        int nextIndex = CurrentPhaseIndex + 1;

        if (phaseSequence == null || nextIndex >= phaseSequence.phaseSceneNames.Count)
        {
            Debug.Log("PhaseProgressionManager: não há próxima fase configurada. Voltando ao menu.");
            MainMenuUI.OpenCreditsOnLoad = phaseSequence != null; // acabou a ultima fase: creditos
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
        if (!LoadSceneWithTransition(mainMenuSceneName, GameState.Menu))
        {
            return;
        }

        CurrentPhaseIndex = -1;
    }
}