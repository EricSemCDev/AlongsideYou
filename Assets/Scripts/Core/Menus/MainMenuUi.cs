using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Menu principal navegavel: Jogar, Configuracoes, Creditos.
// Jogar abre a escolha de modo (Solo ou Co-op); escolher um deles inicia o jogo.
// Configuracoes/Creditos sao paineis placeholder por enquanto - a
// funcionalidade real (audio, texto final) vem em issues futuras.
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject rootCanvas;
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Escolha de modo (aparece ao clicar em Jogar)")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button[] otherButtons; // Configuracoes, Creditos, Sair
    [SerializeField] private CanvasGroup modePanel; // contem Solo, Co-op e Voltar
    [SerializeField] private Button soloButton;
    [SerializeField] private Button coopButton;
    [SerializeField] private Button modeBackButton;
    [SerializeField] private float modeFadeTime = 0.2f;

    private bool _modeOpen;
    private Coroutine _fade;

    private void Start()
    {
        if (modePanel == null)
        {
            return;
        }

        soloButton.onClick.AddListener(() => StartGame(InputMode.Solo));
        coopButton.onClick.AddListener(() => StartGame(InputMode.TwoPlayers));
        modeBackButton.onClick.AddListener(() => ShowModePanel(false));

        ResetModePanel();
    }

    private void Update()
    {
        HandleCancel();

        // Some assim que o jogador clica em Jogar (a cena ainda esta carregando), em vez de
        // ficar visivel durante o fade.
        bool shouldShowMenu = GameManager.Instance != null
            && GameManager.Instance.CurrentState == GameState.Menu
            && !SceneTransitionManager.IsLoadingScene;

        if (rootCanvas != null && rootCanvas.activeSelf != shouldShowMenu)
        {
            rootCanvas.SetActive(shouldShowMenu);

            if (shouldShowMenu)
            {
                ShowMainButtons();
            }
        }
    }

    // B / Bola no controle (ou Esc) fecham a escolha de modo, ou voltam de Configuracoes / Creditos.
    private void HandleCancel()
    {
        if (mainButtonsPanel == null)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Menu)
        {
            return;
        }

        if (mainButtonsPanel.activeSelf && !_modeOpen)
        {
            return;
        }

        bool cancel = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        foreach (Gamepad gamepad in Gamepad.all)
        {
            cancel |= gamepad.buttonEast.wasPressedThisFrame;
        }

        if (!cancel)
        {
            return;
        }

        if (_modeOpen)
        {
            ShowModePanel(false);
        }
        else
        {
            ShowMainButtons();
        }
    }

    public void OnJogarClicked()
    {
        // Sem o painel de modo montado na cena, Jogar inicia direto (modo que o InputManager ja tem).
        if (modePanel == null)
        {
            PhaseProgressionManager.Instance?.StartFirstPhase();
            return;
        }

        ShowModePanel(true);
    }

    private void StartGame(InputMode mode)
    {
        InputManager.Instance?.SetInputMode(mode);
        PhaseProgressionManager.Instance?.StartFirstPhase();
    }

    // Abre/fecha Solo e Co-op. Enquanto abertos, Jogar e os outros botoes ficam desabilitados.
    private void ShowModePanel(bool show)
    {
        _modeOpen = show;
        SetMainButtonsInteractable(!show);

        if (show)
        {
            modePanel.gameObject.SetActive(true);
        }

        if (_fade != null)
        {
            StopCoroutine(_fade);
        }

        _fade = StartCoroutine(FadeModePanel(show));

        // Controle e teclado precisam de um botao selecionado para continuar navegando.
        EventSystem.current?.SetSelectedGameObject(show ? soloButton.gameObject : playButton.gameObject);
    }

    private IEnumerator FadeModePanel(bool show)
    {
        float from = modePanel.alpha;
        float to = show ? 1f : 0f;
        modePanel.interactable = show;

        for (float t = 0f; t < modeFadeTime; t += Time.unscaledDeltaTime)
        {
            modePanel.alpha = Mathf.Lerp(from, to, t / modeFadeTime);
            yield return null;
        }

        modePanel.alpha = to;

        if (!show)
        {
            modePanel.gameObject.SetActive(false);
        }
    }

    private void ResetModePanel()
    {
        if (modePanel == null)
        {
            return;
        }

        if (_fade != null)
        {
            StopCoroutine(_fade);
        }

        _modeOpen = false;
        modePanel.alpha = 0f;
        modePanel.gameObject.SetActive(false);
        SetMainButtonsInteractable(true);
    }

    private void SetMainButtonsInteractable(bool interactable)
    {
        playButton.interactable = interactable;

        foreach (Button button in otherButtons)
        {
            button.interactable = interactable;
        }
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
        ResetModePanel();
        mainButtonsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }
}