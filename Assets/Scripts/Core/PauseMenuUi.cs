using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Menu de pausa: Continuar, Reiniciar Fase, Voltar ao Menu.
public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance { get; private set; }

    [SerializeField] private GameObject rootCanvas;
    [SerializeField] private string menuSceneName = "MainMenu";

    // Persiste entre cenas (o menu de pausa fica na MainMenu e vale para todas as
    // fases). Se voltar ao menu e a cena trouxer outra copia, a nova se destroi.
    // Os dois objetos (este e o canvas) precisam ser raizes na Hierarchy.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyRoot(rootCanvas);
            DestroyRoot(gameObject);
            return;
        }

        Instance = this;
        MakePersistent(gameObject);
        MakePersistent(rootCanvas);
    }

    private static void MakePersistent(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (target.transform.parent != null)
        {
            Debug.LogWarning($"PauseMenuUI: '{target.name}' precisa ser um objeto raiz na Hierarchy para persistir entre cenas.", target);
            return;
        }

        DontDestroyOnLoad(target);
    }

    private static void DestroyRoot(GameObject target)
    {
        if (target != null && target.transform.parent == null)
        {
            Destroy(target);
        }
    }

    private void Update()
    {
        HandlePauseInput();
        HandleCancelInput();

        bool shouldShow = GameManager.Instance != null
            && GameManager.Instance.CurrentState == GameState.Paused;

        if (rootCanvas != null && rootCanvas.activeSelf != shouldShow)
        {
            rootCanvas.SetActive(shouldShow);
        }
    }

    // Esc / Start alterna entre jogando e pausado. Fora desses dois estados
    // (menu, fim de fase, derrota) nao faz nada.
    private void HandlePauseInput()
    {
        if (InputManager.Instance == null || !InputManager.Instance.PausePressed || GameManager.Instance == null)
        {
            return;
        }

        GameState state = GameManager.Instance.CurrentState;

        if (state == GameState.Playing)
        {
            GameManager.Instance.TryTransitionTo(GameState.Paused);
        }
        else if (state == GameState.Paused)
        {
            GameManager.Instance.TryTransitionTo(GameState.Playing);
        }
    }

    // B / Bola no controle tambem continua o jogo (alem do Start / Esc).
    private void HandleCancelInput()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Paused)
        {
            return;
        }

        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad.buttonEast.wasPressedThisFrame)
            {
                GameManager.Instance.TryTransitionTo(GameState.Playing);
                return;
            }
        }
    }

    public void OnContinuarClicked()
    {
        GameManager.Instance?.TryTransitionTo(GameState.Playing);
    }

    public void OnReiniciarFaseClicked()
    {
        PhaseProgressionManager.Instance?.RestartCurrentPhase();
    }

    public void OnVoltarAoMenuClicked()
    {
        PhaseProgressionManager.Instance?.GoToMainMenu();
    }
}