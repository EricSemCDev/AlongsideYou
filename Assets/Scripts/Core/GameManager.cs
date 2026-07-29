using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameStateMachine stateMachine;

    public GameState CurrentState => stateMachine.CurrentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        stateMachine = new GameStateMachine();
    }

    public bool TryTransitionTo(GameState novoEstado)
    {
        bool sucesso = stateMachine.TryTransitionTo(novoEstado);

        if (!sucesso)
        {
            Debug.LogWarning(
                $"Transição inválida: {stateMachine.CurrentState} -> {novoEstado}"
            );
        }

        return sucesso;
    }
}