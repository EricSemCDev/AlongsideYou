using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalculationStationDisplay : MonoBehaviour
{
    [SerializeField] private PuzzleConfig puzzleConfig;

    [Header("Confirmacao")]
    [SerializeField] private bool autoConfirm = true;

    [Header("Tentativas")]
    [SerializeField] private bool limitAttempts;
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private float defeatDelay = 1.5f;

    private const double ComparisonTolerance = 0.0001;

    private NumericSlot[] _slots;
    private OperatorSlotMarker[] _torches;
    private string _lastConfirmedKey;
    public event System.Action<bool> OnEquationValidated;
    public event System.Action<int, int> OnFailedAttemptsChanged;
    public event System.Action OnAttemptsExhausted;

    public bool HasValidResult { get; private set; }
    public int FailedAttempts { get; private set; }

    public void UseConfig(PuzzleConfig config)
    {
        puzzleConfig = config;
    }

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
    }

    private void Update()
    {
        if (!AllFilled())
        {
            HasValidResult = false;
            _lastConfirmedKey = null;
            return;
        }

        double result = EvaluateCurrentEquation();
        HasValidResult = true;
        CurrentResultValue = result;

        if (autoConfirm)
        {
            ConfirmIfNewArrangement();
        }
    }

    private void ConfirmIfNewArrangement()
    {
        string key = BuildArrangementKey();

        if (key == _lastConfirmedKey)
        {
            return;
        }

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

    private IEnumerator DefeatRoutine()
    {
        GameManager.Instance?.TryTransitionTo(GameState.GameOver);

        yield return new WaitForSecondsRealtime(defeatDelay);

        if (PhaseProgressionManager.Instance != null && PhaseProgressionManager.Instance.CanRestartCurrentPhase)
        {
            PhaseProgressionManager.Instance.RestartCurrentPhase();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameManager.Instance?.TryTransitionTo(GameState.Playing);
        }
    }

    private bool AllFilled()
    {
        if (_slots.Length == 0)
        {
            return false;
        }

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
}