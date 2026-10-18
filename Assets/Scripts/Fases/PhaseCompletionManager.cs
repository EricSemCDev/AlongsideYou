using UnityEngine;

// Controla o tempo da fase atual e o que acontece ao completá-la:
// transiciona o GameState, congela o tempo, e avança pra próxima
// fase via PhaseProgressionManager.
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
            || !InputAccessPolicy.CanAcceptInput(GameManager.Instance.CurrentState)
            || TutorialSequencePlayer.IsAnyPlaying)
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

        int phaseIndex = PhaseProgressionManager.Instance != null
            ? PhaseProgressionManager.Instance.CurrentPhaseIndex
            : -1;

        GameManager.Instance?.TryTransitionTo(GameState.Complete);
        OnPhaseCompleted?.Invoke(CurrentElapsedTime);

        PhaseProgressionManager.Instance?.CompleteCurrentPhase(CurrentElapsedTime);
    }
}