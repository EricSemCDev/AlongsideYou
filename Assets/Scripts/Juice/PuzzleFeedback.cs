using UnityEngine;
using UnityEngine.SceneManagement;

// Liga a estacao de calculo aos efeitos (GameJuice): acertar, errar e acabar as
// tentativas. Se coloca SOZINHO em toda CalculationStationDisplay de cada fase
// (nada para montar em cena).
[DisallowMultipleComponent]
public class PuzzleFeedback : MonoBehaviour
{
    private CalculationStationDisplay _station;

    // Roda uma vez ao abrir o jogo e em toda cena carregada depois.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        SceneManager.sceneLoaded += (scene, mode) => AttachAll();
        AttachAll();
    }

    private static void AttachAll()
    {
        foreach (CalculationStationDisplay station in FindObjectsByType<CalculationStationDisplay>(FindObjectsSortMode.None))
        {
            if (station.GetComponent<PuzzleFeedback>() == null)
            {
                station.gameObject.AddComponent<PuzzleFeedback>();
            }
        }

        foreach (ThornBarrier barrier in FindObjectsByType<ThornBarrier>(FindObjectsSortMode.None))
        {
            if (barrier.GetComponent<BarrierFeedback>() == null)
            {
                barrier.gameObject.AddComponent<BarrierFeedback>();
            }
        }
    }

    private void OnEnable()
    {
        _station = GetComponent<CalculationStationDisplay>();

        if (_station != null)
        {
            _station.OnEquationValidated += HandleValidated;
            _station.OnAttemptsExhausted += HandleExhausted;
        }
    }

    private void OnDisable()
    {
        if (_station != null)
        {
            _station.OnEquationValidated -= HandleValidated;
            _station.OnAttemptsExhausted -= HandleExhausted;
        }
    }

    private void HandleValidated(bool isCorrect)
    {
        // Os efeitos saem do numero do resultado (se a estacao tiver um), senao da propria estacao.
        Transform label = _station.ResultLabel != null ? _station.ResultLabel.transform : null;
        Vector3 position = label != null ? label.position : transform.position;

        if (isCorrect)
        {
            GameJuice.ResultCorrect(position, label);
        }
        else
        {
            GameJuice.ResultWrong(position, label);
        }
    }

    private void HandleExhausted()
    {
        GameJuice.AttemptsExhausted();
    }
}
