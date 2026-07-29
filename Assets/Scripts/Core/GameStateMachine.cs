using System.Collections.Generic;

public class GameStateMachine
{
    private static readonly HashSet<(GameState De, GameState Para)> TransicoesValidas = new()
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

    public GameStateMachine(GameState estadoInicial = GameState.Menu)
    {
        CurrentState = estadoInicial;
    }

    public bool TryTransitionTo(GameState novoEstado)
    {
            if (!TransicoesValidas.Contains((CurrentState, novoEstado)))
            {
                return false;
            }

        CurrentState = novoEstado;
        return true;
    }
}