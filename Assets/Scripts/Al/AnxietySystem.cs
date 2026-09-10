using System.Collections.Generic;
using UnityEngine;

public class AnxietySystem : MonoBehaviour
{
    [SerializeField] private AnxietyConfig config;
    [SerializeField] private List<CalculationStationDisplay> stations = new();

    private float _elapsed;

    public int CurrentStage { get; private set; } = 1;

    // Disparado sempre que o estágio muda (subindo por tempo ou
    // descendo ao acertar uma equação). Sistemas futuros (LightSystem,
    // animações, GameManager) assinam isso — nenhuma reação concreta
    // implementada ainda além de expor o estado.
    public event System.Action<int> OnStageChanged;

    private void OnEnable()
    {

        foreach (CalculationStationDisplay station in stations)
        {
            if (station != null)
            {
                station.OnEquationValidated += HandleEquationValidated;
            }
        }
    }

    private void OnDisable()
    {
        OnStageChanged -= LogStageChanged; // DEBUG TEMPORÁRIO

        foreach (CalculationStationDisplay station in stations)
        {
            if (station != null)
            {
                station.OnEquationValidated -= HandleEquationValidated;
            }
        }
    }

    private void LogStageChanged(int newStage) // DEBUG TEMPORÁRIO
    {
        Debug.Log($"Ansiedade: estágio {newStage}");
    }

    private void Update()
    {
        if (GameManager.Instance == null
            || !InputAccessPolicy.CanAcceptInput(GameManager.Instance.CurrentState))
        {
            return; // pausado/menu — o timer não avança
        }

        if (CurrentStage >= 5)
        {
            return; // estágio final, sem avanço automático
        }

        _elapsed += Time.deltaTime;

        if (_elapsed >= GetDurationForStage(CurrentStage))
        {
            _elapsed = 0f;
            SetStage(CurrentStage + 1);
        }
    }

    private float GetDurationForStage(int stage)
    {
        if (config == null)
        {
            return float.MaxValue; // sem config atribuída, nunca avança — seguro por padrão
        }

        switch (stage)
        {
            case 1:
                return config.stage1Duration;
            case 2:
                return config.stage2Duration;
            case 3:
                return config.stage3Duration;
            case 4:
                return config.stage4Duration;
            default:
                return float.MaxValue;
        }
    }

    private void HandleEquationValidated(bool isCorrect)
    {
        if (!isCorrect)
        {
            return;
        }

        _elapsed = 0f;
        SetStage(CurrentStage - 1);
    }

    private void SetStage(int newStage)
    {
        int clamped = Mathf.Clamp(newStage, 1, 5);

        if (clamped == CurrentStage)
        {
            return;
        }

        CurrentStage = clamped;
        OnStageChanged?.Invoke(CurrentStage);
    }
}