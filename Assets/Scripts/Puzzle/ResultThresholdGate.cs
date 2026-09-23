using UnityEngine;

public enum ThresholdComparison
{
    GreaterOrEqual,
    Less
}

// Condição de limiar: satisfeita quando o resultado ao vivo é
// >= ou < que o targetResult da fase.
public class ResultThresholdGate : MonoBehaviour, IGateCondition
{
    [SerializeField] private CalculationStationDisplay station;
    [SerializeField] private ThresholdComparison comparison = ThresholdComparison.GreaterOrEqual;

    public bool ConditionMet { get; private set; }

    private void Update()
    {
        if (station == null || !station.HasValidResult || station.Config == null)
        {
            ConditionMet = false;
            return;
        }

        double target = station.Config.targetResult;
        double current = station.CurrentResultValue;

        ConditionMet = comparison == ThresholdComparison.GreaterOrEqual
            ? current >= target
            : current < target;
    }
}