using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly GameStateMachine stateMachine = new();

    public GameState CurrentState => stateMachine.CurrentState;

    // Disparado depois de cada mudanca de estado valida, com o novo estado.
    public event System.Action<GameState> OnStateChanged;

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

    public bool TryTransitionTo(GameState newState)
    {
        bool success = stateMachine.TryTransitionTo(newState);

        if (success)
        {
            // Pausado congela o tempo do jogo (Animators, particulas, fisica).
            // O que usa tempo nao escalado (fade, botoes) continua funcionando.
            Time.timeScale = newState == GameState.Paused ? 0f : 1f;
            OnStateChanged?.Invoke(newState);
        }

        if (!success)
        {
            Debug.LogWarning(
                $"Invalid transition: {stateMachine.CurrentState} -> {newState}"
            );
        }

        return success;
    }
}