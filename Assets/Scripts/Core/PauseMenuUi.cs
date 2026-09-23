using UnityEngine;
using UnityEngine.SceneManagement;

// Menu de pausa: Continuar, Reiniciar Fase, Voltar ao Menu.
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject rootCanvas;
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Update()
    {
        bool shouldShow = GameManager.Instance != null
            && GameManager.Instance.CurrentState == GameState.Paused;

        if (rootCanvas != null && rootCanvas.activeSelf != shouldShow)
        {
            rootCanvas.SetActive(shouldShow);
        }
    }

    public void OnContinuarClicked()
    {
        GameManager.Instance?.TryTransitionTo(GameState.Playing);
    }

    public void OnReiniciarFaseClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance?.TryTransitionTo(GameState.Playing);
    }

    public void OnVoltarAoMenuClicked()
    {
        SceneManager.LoadScene(menuSceneName);
        GameManager.Instance?.TryTransitionTo(GameState.Menu);
    }
}