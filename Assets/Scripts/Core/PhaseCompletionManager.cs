using UnityEngine;
using UnityEngine.SceneManagement;

// Controla o tempo da fase atual e o que acontece ao completá-la:
// transiciona o GameState, congela o tempo, e tenta avançar pra
// próxima cena (se existir configurada nas Build Settings).
public class PhaseCompletionManager : MonoBehaviour
{
    public static PhaseCompletionManager Instance { get; private set; }

    public float CurrentElapsedTime { get; private set; }
    public bool IsCompleted { get; private set; }

    // Disparado no momento da conclusão, com o tempo final da fase.
    public event System.Action<float> OnPhaseCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (IsCompleted)
        {
            return;
        }

        if (GameManager.Instance == null
            || !InputAccessPolicy.CanAcceptInput(GameManager.Instance.CurrentState))
        {
            return; // pausado/menu — o tempo não corre
        }

        CurrentElapsedTime += Time.deltaTime;
    }

    public void CompletePhase()
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;

        GameManager.Instance?.TryTransitionTo(GameState.Complete);
        OnPhaseCompleted?.Invoke(CurrentElapsedTime);

        TryLoadNextScene();
    }

    private void TryLoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("Fase concluída — nenhuma próxima fase configurada nas Build Settings ainda.");
        }
    }
}