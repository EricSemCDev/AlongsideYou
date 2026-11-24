using UnityEngine;

public enum SignType
{
    Positive,
    Negative,
    Zero
}

// Condição de sinal: satisfeita quando o resultado ao vivo é
// positivo/negativo/zero.
public class SignGate : MonoBehaviour, IGateCondition
{
    [SerializeField] private CalculationStationDisplay station;
    [SerializeField] private SignType requiredSign = SignType.Positive;

    public bool ConditionMet { get; private set; }

    private void Update()
    {
        if (station == null || !station.HasValidResult)
        {
            ConditionMet = false;
            return;
        }

        double value = station.CurrentResultValue;

        ConditionMet = requiredSign switch
        {
            SignType.Positive => value > 0,
            SignType.Negative => value < 0,
            SignType.Zero => value == 0,
            _ => false,
        };
    }
}