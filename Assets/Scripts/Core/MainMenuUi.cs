using UnityEngine;
using UnityEngine.InputSystem;
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

    private void Update()
    {
        HandleCancel();

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

    // B / Bola no controle (ou Esc) voltam de Configuracoes / Creditos para o menu.
    private void HandleCancel()
    {
        if (mainButtonsPanel == null || mainButtonsPanel.activeSelf)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Menu)
        {
            return;
        }

        bool cancel = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        foreach (Gamepad gamepad in Gamepad.all)
        {
            cancel |= gamepad.buttonEast.wasPressedThisFrame;
        }

        if (cancel)
        {
            ShowMainButtons();
        }
    }

    public void OnJogarClicked()
    {
        PhaseProgressionManager.Instance?.StartFirstPhase();
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