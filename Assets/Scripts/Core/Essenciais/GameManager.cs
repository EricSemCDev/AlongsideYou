using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        (GameState.Complete, GameState.Menu),
    };

    public GameState CurrentState { get; private set; } = GameState.Menu;

    // "O jogo esta rodando?" Tambem e false se nao existe GameManager (ex: cena rodando direto no editor).
    public static bool IsPlaying => Instance != null && Instance.CurrentState == GameState.Playing;

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
        if (!ValidTransitions.Contains((CurrentState, newState)))
        {
            Debug.LogWarning($"Troca de estado invalida: {CurrentState} -> {newState}");
            return false;
        }

        CurrentState = newState;
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        return true;
    }
}
