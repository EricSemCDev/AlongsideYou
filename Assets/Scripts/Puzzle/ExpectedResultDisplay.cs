using TMPro;
using UnityEngine;

// Mostra o resultado esperado (o alvo da fase) em um texto do mundo, por
// exemplo na tocha "ResultadoEsperado". Le o valor de
// CalculationStationDisplay.Config.targetResult, que o PuzzleGenerator
// sorteia no Awake. Atualiza sozinho se o valor mudar.
public class ExpectedResultDisplay : MonoBehaviour
{
    [SerializeField] private CalculationStationDisplay station;
    [SerializeField] private TextMeshPro label;

    [Header("Display")]
    [Tooltip("Desmarque nas fases avancadas em que o alvo nao deve aparecer (so as tochas como feedback).")]
    [SerializeField] private bool showTarget = true;
    [Tooltip("Texto mostrado quando o alvo esta escondido.")]
    [SerializeField] private string hiddenText = "?";

    private bool _hasShownValue;
    private int _lastShownValue;
    private bool _lastShownVisible;

    private void Awake()
    {
        if (label == null)
        {
            label = GetComponentInChildren<TextMeshPro>();
        }

        if (station == null)
        {
            station = FindFirstObjectByType<CalculationStationDisplay>();
        }
    }

    // Start (e nao Awake): o PuzzleGenerator sorteia o alvo no Awake dele, e
    // todos os Awake ja rodaram quando o Start comeca.
    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (label == null || station == null || station.Config == null)
        {
            return;
        }

        int target = station.Config.targetResult;

        if (_hasShownValue && target == _lastShownValue && showTarget == _lastShownVisible)
        {
            return;
        }

        _hasShownValue = true;
        _lastShownValue = target;
        _lastShownVisible = showTarget;

        label.text = showTarget ? target.ToString() : hiddenText;
    }
}
