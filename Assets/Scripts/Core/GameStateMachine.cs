using System.Collections.Generic;

public class GameStateMachine
{
    private static readonly HashSet<(GameState From, GameState To)> ValidTransitions = new()
    {
        (GameState.Menu, GameState.Playing),
        (GameState.GameOver, GameState.Playing),
        (GameState.Complete, GameState.Playing),
        (GameState.Playing, GameState.Paused),
        (GameState.Paused, GameState.Playing),
        (GameState.Playing, GameState.Complete),
        (GameState.Playing, GameState.GameOver),
        (GameState.Paused, GameState.Menu),
    };

    public GameState CurrentState { get; private set; }

    public GameStateMachine(GameState initialState = GameState.Menu)
    {
        CurrentState = initialState;
    }

    public bool TryTransitionTo(GameState newState)
    {
        if (!ValidTransitions.Contains((CurrentState, newState)))
        {
            return false;
        }

        CurrentState = newState;
        return true;
    }
}