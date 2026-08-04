using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly GameStateMachine stateMachine = new();

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
    }

    public bool TryTransitionTo(GameState newState)
    {
        bool success = stateMachine.TryTransitionTo(newState);

        if (!success)
        {
            Debug.LogWarning(
                $"Invalid transition: {stateMachine.CurrentState} -> {newState}"
            );
        }

        return success;
    }
}