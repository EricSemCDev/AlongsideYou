using UnityEngine;
using UnityEngine.SceneManagement;

// Menu principal navegável: Jogar, Configurações, Créditos.
// Configurações/Créditos são painéis placeholder por enquanto — a
// funcionalidade real (áudio, texto final) vem em issues futuras.
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject rootCanvas;
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private string gameplaySceneName = "SampleScene";

    private void Update()
    {
        bool shouldShowMenu = GameManager.Instance != null
            && GameManager.Instance.CurrentState == GameState.Menu;

        if (rootCanvas != null && rootCanvas.activeSelf != shouldShowMenu)
        {
            rootCanvas.SetActive(shouldShowMenu);

            if (shouldShowMenu)
            {
                ShowMainButtons();
            }
        }
    }

    public void OnJogarClicked()
    {
        SceneManager.LoadScene(gameplaySceneName);
        GameManager.Instance?.TryTransitionTo(GameState.Playing);
    }

    public void OnConfiguracoesClicked()
    {
        mainButtonsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnCreditosClicked()
    {
        mainButtonsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OnVoltarClicked()
    {
        ShowMainButtons();
    }

    public void OnSairClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowMainButtons()
    {
        mainButtonsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }
}