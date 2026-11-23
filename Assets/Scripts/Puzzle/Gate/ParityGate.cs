using UnityEngine;

public enum ParityType
{
    Even,
    Odd
}

// Condição de paridade: satisfeita quando o resultado ao vivo é
// par/ímpar. Não definida para resultados com casas decimais.
public class ParityGate : MonoBehaviour, IGateCondition
{
    [SerializeField] private CalculationStationDisplay station;
    [SerializeField] private ParityType parity = ParityType.Even;

    public bool ConditionMet { get; private set; }

    private void Update()
    {
        if (station == null || !station.HasValidResult)
        {
            ConditionMet = false;
            return;
        }

        double value = station.CurrentResultValue;
        bool isWholeNumber = value == System.Math.Floor(value);

        if (!isWholeNumber)
        {
            ConditionMet = false;
            return;
        }

        int intValue = (int)value;
        bool isEven = intValue % 2 == 0;

        ConditionMet = parity == ParityType.Even ? isEven : !isEven;
    }
}