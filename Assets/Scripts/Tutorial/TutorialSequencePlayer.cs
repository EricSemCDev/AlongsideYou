using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Toca uma sequencia de falas do tutorial: para cada fala a camera enquadra um
// objeto (zoom suave), a caixa de dialogo digita o texto e o jogador aperta
// para avancar. Durante a conversa o mundo segue animado, mas os controles, a
// ansiedade do Al e o cronometro da fase ficam parados ate o final.
//
// Para avancar: Espaco, Enter, clique esquerdo ou botao A/Sul do controle.
//
// Uso: crie um objeto na fase com este componente, arraste a caixa de dialogo
// (TutorialDialogueUI) e preencha as falas. Dispara sozinho no inicio da fase
// (Play On Start) ou por codigo/evento/zona (TutorialTriggerZone) chamando Play().
public class TutorialSequencePlayer : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("A caixa de dialogo. Vazio = procura uma na cena.")]
    [SerializeField] private TutorialDialogueUI dialogue;

    [Header("Falas")]
    [SerializeField] private string defaultSpeaker = "Finn";
    [SerializeField] private Sprite defaultPortrait;
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    [Header("Disparo")]
    [SerializeField] private bool playOnStart = true;
    [Tooltip("Espera apos o fade de entrada da fase antes de comecar.")]
    [SerializeField] private float startDelay = 0.6f;
    [Tooltip("Ligado: so toca uma vez por sessao do jogo (nao repete ao reiniciar a fase).")]
    [SerializeField] private bool onlyOncePerSession = true;
    [Tooltip("Identificador para o 'uma vez por sessao'. Vazio = nome do objeto.")]
    [SerializeField] private string sequenceId = "";

    [Header("Camera")]
    [Tooltip("Duracao minima de cada movimento de camera (segundos). Maior = mais lento.")]
    [SerializeField] private float cameraMoveSeconds = 0.7f;
    [Tooltip("O texto comeca a digitar quando a camera ja percorreu esta fracao do movimento.")]
    [Range(0.2f, 1f)] [SerializeField] private float textStartsAtMove = 0.55f;
    [Tooltip("Multiplica o zoom de todas as falas. Menor = mais perto (0.5 = o dobro de zoom). 1 = valores das falas.")]
    [Range(0.2f, 1.5f)] [SerializeField] private float zoomMultiplier = 0.75f;

    [Header("Entrada")]
    [Tooltip("Evita que o mesmo toque que avancou uma fala pule a proxima.")]
    [SerializeField] private float inputLockAfterLine = 0.25f;

    [Header("Eventos")]
    [SerializeField] private UnityEvent onStarted;
    [SerializeField] private UnityEvent onFinished;

    private static readonly HashSet<string> PlayedThisSession = new HashSet<string>();
    private static int _activePlayers;

    private Coroutine _routine;
    private TutorialCameraFocus _camera;

    public static bool IsAnyPlaying => _activePlayers > 0;
    public bool IsPlaying => _routine != null;

    private string Id => string.IsNullOrEmpty(sequenceId) ? gameObject.name : sequenceId;

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
            Release();
        }
    }

    // Pode ser chamado por codigo, UnityEvent ou TutorialTriggerZone.
    public void Play()
    {
        if (_routine != null || steps.Count == 0)
        {
            return;
        }

        if (onlyOncePerSession && PlayedThisSession.Contains(Id))
        {
            return;
        }

        _routine = StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (dialogue == null)
        {
            dialogue = FindFirstObjectByType<TutorialDialogueUI>();
        }

        if (dialogue == null)
        {
            Debug.LogWarning("TutorialSequencePlayer: nenhuma TutorialDialogueUI na cena.", this);
            _routine = null;
            yield break;
        }

        // Espera o fade de entrada da fase e o jogo estar rodando.
        while (IsSceneBusy())
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(startDelay);

        // Espera o cartao com o nome da fase terminar de passar.
        while (IsSceneBusy() || PhaseTitleCard.IsAnyPlaying)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.4f);

        PlayedThisSession.Add(Id);
        Freeze();
        onStarted?.Invoke();

        _camera = TutorialCameraFocus.GetOrCreate();

        for (int i = 0; i < steps.Count; i++)
        {
            TutorialStep step = steps[i];

            ApplyCamera(step);
            step.onStepStart?.Invoke();

            // Deixa a camera chegar (quase) la antes do texto aparecer.
            while (_camera != null && _camera.IsMoving && _camera.Progress < textStartsAtMove)
            {
                yield return null;
            }

            string speaker = string.IsNullOrEmpty(step.speaker) ? defaultSpeaker : step.speaker;
            Sprite portrait = step.portrait != null ? step.portrait : defaultPortrait;
            dialogue.SetSpeaker(speaker, portrait);

            // A caixa so sobe na primeira fala, ja com nome e retrato preenchidos.
            if (!dialogue.IsOpen)
            {
                yield return dialogue.Open();
            }

            dialogue.BeginLine(step.text, step.gamepadText);

            float lockUntil = Time.unscaledTime + inputLockAfterLine;

            // Digitando: apertar completa a fala.
            while (dialogue.IsTyping)
            {
                if (Time.unscaledTime >= lockUntil && AdvancePressed())
                {
                    dialogue.CompleteNow();
                }

                yield return null;
            }

            dialogue.SetContinueVisible(true);
            yield return null;

            // Esperando o jogador querer continuar.
            while (true)
            {
                if (Time.unscaledTime >= lockUntil && AdvancePressed())
                {
                    break;
                }

                yield return null;
            }

            dialogue.SetContinueVisible(false);
        }

        // Fim: caixa desce e a camera volta para a visao geral.
        if (_camera != null)
        {
            _camera.ReturnHome(cameraMoveSeconds);
        }

        yield return dialogue.Close();

        while (_camera != null && _camera.IsMoving)
        {
            yield return null;
        }

        Release();
        _routine = null;
        onFinished?.Invoke();
    }

    private void ApplyCamera(TutorialStep step)
    {
        if (_camera == null)
        {
            return;
        }

        if (step.focusTarget != null)
        {
            Vector3 position = step.focusTarget.position + (Vector3)step.focusOffset;
            _camera.FocusOn(position, step.zoomSize * zoomMultiplier, cameraMoveSeconds);
        }
        else
        {
            _camera.ReturnHome(cameraMoveSeconds);
        }
    }

    // --- Travar / liberar o jogo --------------------------------------------
    // O tempo NAO para (animacoes, particulas, luzes e o mundo continuam vivos).
    // O que para: os controles (GameplayInputSuspended), a ansiedade do Al e o
    // cronometro da fase, que checam TutorialSequencePlayer.IsAnyPlaying.

    private bool _frozen;

    private void Freeze()
    {
        if (_frozen)
        {
            return;
        }

        _frozen = true;
        _activePlayers++;

        if (InputManager.Instance != null)
        {
            InputManager.Instance.GameplayInputSuspended = true;
        }
    }

    private void Release()
    {
        if (!_frozen)
        {
            return;
        }

        _frozen = false;
        _activePlayers = Mathf.Max(0, _activePlayers - 1);

        if (InputManager.Instance != null)
        {
            InputManager.Instance.GameplayInputSuspended = _activePlayers > 0;
        }
    }

    private static bool IsSceneBusy()
    {
        if (SceneTransitionManager.Instance != null && SceneTransitionManager.Instance.IsTransitioning)
        {
            return true;
        }

        // Sem GameManager (rodando a cena direto no editor) conta como jogando.
        return GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing;
    }

    // --- Entrada -----------------------------------------------------------

    private static bool AdvancePressed()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused)
        {
            return false;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame))
        {
            return true;
        }

        Mouse mouse = Mouse.current;

        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }
}
