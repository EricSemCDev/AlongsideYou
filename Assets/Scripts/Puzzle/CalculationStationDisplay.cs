using System.Linq;
using TMPro;
using UnityEngine;

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

    private const double ComparisonTolerance = 0.0001;

    private NumericSlot[] _slots;
    private OperatorSlotMarker[] _torches;
    private Color _defaultLabelColor;

    // Disparado quando o Al confirma uma tentativa na alavanca,
    // com true se o resultado bateu com o alvo da fase. Sistemas
    // futuros (abrir passagem, derreter gelo, etc.) assinam isso.
    public event System.Action<bool> OnEquationValidated;

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
            ShowIncomplete();
            return;
        }

        double result = EvaluateCurrentEquation();
        ShowResult(result);
    }

    public void ConfirmAttempt()
    {
        if (!AllFilled())
        {
            return; // nada para confirmar ainda
        }

        double result = EvaluateCurrentEquation();
        bool isCorrect = puzzleConfig != null
            && System.Math.Abs(result - puzzleConfig.targetResult) < ComparisonTolerance;

        OnEquationValidated?.Invoke(isCorrect);
        ShowValidationFeedback(isCorrect);
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