using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Painel diegético (parte do mundo, não HUD) que mostra o resultado
// da equação assim que todos os slots numéricos e tochas de operador
// estiverem preenchidos. Sem PuzzleGenerator ainda, então cada fase
// organiza manualmente seus NumericSlot/OperatorSlotMarker como
// filhos deste objeto, com o campo "Position" definindo a ordem.
public class CalculationStationDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro resultLabel;
    [SerializeField] private string incompleteText = "–";
    [SerializeField] private PuzzleConfig puzzleConfig;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;

    [Header("Confirmacao")]
    [Tooltip("Confirma sozinho assim que todos os slots e tochas estao preenchidos, uma vez por arranjo. Desligado, so a alavanca confirma.")]
    [SerializeField] private bool autoConfirm = true;

    [Header("Tentativas")]
    [Tooltip("Liga o limite de erros. Ao errar Max Attempts arranjos diferentes, o jogador perde e a fase reinicia. Desligado, pode tentar a vontade.")]
    [SerializeField] private bool limitAttempts;
    [SerializeField] private int maxAttempts = 3;
    [Tooltip("Segundos mostrando o ultimo erro (vermelho) antes de reiniciar a fase.")]
    [SerializeField] private float defeatDelay = 1.5f;

    private const double ComparisonTolerance = 0.0001;

    private NumericSlot[] _slots;
    private OperatorSlotMarker[] _torches;
    private Color _defaultLabelColor;
    private string _lastConfirmedKey;

    // Disparado quando o Al confirma uma tentativa na alavanca,
    // com true se o resultado bateu com o alvo da fase. Sistemas
    // futuros (abrir passagem, derreter gelo, etc.) assinam isso.
    public event System.Action<bool> OnEquationValidated;

    // Disparado a cada erro contado, com (erros ate agora, maximo). So com
    // Limit Attempts ligado.
    public event System.Action<int, int> OnFailedAttemptsChanged;

    // Disparado quando os erros chegam ao maximo, antes de reiniciar a fase.
    public event System.Action OnAttemptsExhausted;

    // Expostos para os obstáculos (ResultThresholdGate, ParityGate,
    // SignGate) lerem o resultado ao vivo, sem precisar reavaliar a
    // equação por conta própria.
    public bool HasValidResult { get; private set; }
    public int FailedAttempts { get; private set; }
    public int MaxAttempts => maxAttempts;
    public int AttemptsRemaining => Mathf.Max(0, maxAttempts - FailedAttempts);
    public bool IsSolved { get; private set; }
    public double CurrentResultValue { get; private set; }
    public PuzzleConfig Config => puzzleConfig;

    private void Awake()
    {
        _slots = GetComponentsInChildren<NumericSlot>()
            .OrderBy(slot => slot.Position)
            .ToArray();

        _torches = GetComponentsInChildren<OperatorSlotMarker>()
            .OrderBy(torch => torch.Position)
            .ToArray();

        if (resultLabel != null)
        {
            _defaultLabelColor = resultLabel.color;
        }
    }

    private void Update()
    {
        if (!AllFilled())
        {
            HasValidResult = false;
            _lastConfirmedKey = null; // incompleto: o proximo arranjo completo conta como novo
            ShowIncomplete();
            return;
        }

        double result = EvaluateCurrentEquation();
        HasValidResult = true;
        CurrentResultValue = result;
        ShowResult(result);

        if (autoConfirm)
        {
            ConfirmIfNewArrangement();
        }
    }

    // Confirma uma unica vez para cada combinacao completa de blocos e operadores.
    // Mexer em qualquer coisa muda a chave e a proxima combinacao completa e avaliada.
    private void ConfirmIfNewArrangement()
    {
        string key = BuildArrangementKey();

        if (key == _lastConfirmedKey)
        {
            return;
        }

        // ConfirmAttempt grava a chave do arranjo (e decide se conta como erro).
        ConfirmAttempt();
    }

    private string BuildArrangementKey()
    {
        string values = string.Join(",", _slots.Select(slot => slot.CurrentValue));
        string operators = new string(_torches.Select(torch => torch.CurrentSymbol).ToArray());
        return values + "|" + operators;
    }

    public void ConfirmAttempt()
    {
        if (!AllFilled())
        {
            return; // nada para confirmar ainda
        }

        // Cada combinacao conta uma vez: apertar a alavanca de novo no mesmo
        // arranjo nao conta como outra tentativa.
        string key = BuildArrangementKey();
        bool isNewArrangement = key != _lastConfirmedKey;
        _lastConfirmedKey = key;

        double result = EvaluateCurrentEquation();
        bool isCorrect = puzzleConfig != null
            && System.Math.Abs(result - puzzleConfig.targetResult) < ComparisonTolerance;

        OnEquationValidated?.Invoke(isCorrect);
        ShowValidationFeedback(isCorrect);

        if (isCorrect)
        {
            IsSolved = true; // depois de resolver, errar nao custa mais nada
        }
        else if (isNewArrangement)
        {
            RegisterFailedAttempt();
        }
    }

    private void RegisterFailedAttempt()
    {
        if (!limitAttempts || IsSolved || FailedAttempts >= maxAttempts)
        {
            return;
        }

        FailedAttempts++;
        OnFailedAttemptsChanged?.Invoke(FailedAttempts, maxAttempts);

        if (FailedAttempts >= maxAttempts)
        {
            OnAttemptsExhausted?.Invoke();
            StartCoroutine(DefeatRoutine());
        }
    }

    // Derrota por tentativas: GameOver, uma pausa para o jogador ver o erro,
    // e a fase recomeca do zero (com o fade, se houver PhaseProgressionManager).
    private IEnumerator DefeatRoutine()
    {
        GameManager.Instance?.TryTransitionTo(GameState.GameOver);

        yield return new WaitForSecondsRealtime(defeatDelay);

        if (PhaseProgressionManager.Instance != null && PhaseProgressionManager.Instance.CurrentPhaseIndex >= 0)
        {
            PhaseProgressionManager.Instance.RestartCurrentPhase();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameManager.Instance?.TryTransitionTo(GameState.Playing);
        }
    }

    private void ShowValidationFeedback(bool isCorrect)
    {
        if (resultLabel != null)
        {
            resultLabel.color = isCorrect ? correctColor : incorrectColor;
        }
    }

    private bool AllFilled()
    {
        foreach (NumericSlot slot in _slots)
        {
            if (!slot.IsFilled)
            {
                return false;
            }
        }

        foreach (OperatorSlotMarker torch in _torches)
        {
            if (!torch.IsLit)
            {
                return false;
            }
        }

        return true;
    }

    private double EvaluateCurrentEquation()
    {
        double[] values = _slots.Select(slot => (double)slot.CurrentValue).ToArray();
        char[] operators = _torches.Select(torch => torch.CurrentSymbol).ToArray();

        return EquationEvaluator.Evaluate(values, operators);
    }

    private void ShowResult(double result)
    {
        if (resultLabel == null)
        {
            return;
        }

        // Mostra sem casas decimais quando o resultado é um número
        // inteiro; senão, com até 2 casas decimais.
        bool isWholeNumber = result == System.Math.Floor(result);
        resultLabel.text = isWholeNumber ? result.ToString("0") : result.ToString("0.##");
    }

    private void ShowIncomplete()
    {
        if (resultLabel != null)
        {
            resultLabel.text = incompleteText;
            resultLabel.color = _defaultLabelColor;
        }
    }
}