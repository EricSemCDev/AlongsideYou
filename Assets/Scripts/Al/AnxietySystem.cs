using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Estágio de ansiedade do Al, agora dirigido por proximidade de luz:
// sobe conforme fica na escuridão, desce conforme fica perto de uma
// fonte de luz "de segurança" (Finn, tochas). A luz pessoal do próprio
// Al é ignorada nessa checagem — é só acessória visual.
public class AnxietySystem : MonoBehaviour
{
    [SerializeField] private AnxietyConfig config;
    [SerializeField] private List<Light2D> safetyLights = new();

    private float _elapsedInCurrentZone;
    private bool _wasInLightLastFrame;

    public int CurrentStage { get; private set; } = 1;

    // Disparado sempre que o estágio muda. Sistemas futuros (visual,
    // velocidade, derrota) assinam isso — nenhuma reação concreta mora
    // aqui dentro.
    public event System.Action<int> OnStageChanged;

    private void Awake()
    {
        if (safetyLights.Count == 0)
        {
            AutoFindSafetyLights();
        }
    }

    // Encontra sozinho todas as luzes Point da cena, exceto qualquer
    // uma anexada ao próprio Al (a luz pessoal dele não conta como
    // "segurança" — só ajuda o jogador a não perdê-lo de vista).
    private void AutoFindSafetyLights()
    {
        Light2D[] allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);

        foreach (Light2D light in allLights)
        {
            if (light.lightType != Light2D.LightType.Point)
            {
                continue;
            }

            if (light.transform == transform || light.transform.IsChildOf(transform))
            {
                continue;
            }

            safetyLights.Add(light);
        }
    }

    private void OnEnable()
    {
        OnStageChanged += LogStageChanged; // DEBUG TEMPORÁRIO — remover depois de confirmar
    }

    private void OnDisable()
    {
        OnStageChanged -= LogStageChanged; // DEBUG TEMPORÁRIO
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
            return; // pausado/menu — não avança nem recupera
        }

        bool isInLight = IsNearAnySafetyLight();

        if (isInLight != _wasInLightLastFrame)
        {
            _elapsedInCurrentZone = 0f; // trocou de zona, recomeça a contagem
        }

        _wasInLightLastFrame = isInLight;
        _elapsedInCurrentZone += Time.deltaTime;

        if (isInLight)
        {
            HandleRecovery();
        }
        else
        {
            HandleEscalation();
        }
    }

    private bool IsNearAnySafetyLight()
    {
        foreach (Light2D light in safetyLights)
        {
            if (light == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, light.transform.position);

            if (distance <= light.pointLightOuterRadius)
            {
                return true;
            }
        }

        return false;
    }

    private void HandleEscalation()
    {
        if (CurrentStage >= 5 || config == null)
        {
            return;
        }

        if (_elapsedInCurrentZone >= config.timeInDarknessToEscalate)
        {
            _elapsedInCurrentZone = 0f;
            SetStage(CurrentStage + 1);
        }
    }

    private void HandleRecovery()
    {
        if (CurrentStage <= 1 || config == null)
        {
            return;
        }

        if (_elapsedInCurrentZone >= config.timeInLightToRecover)
        {
            _elapsedInCurrentZone = 0f;
            SetStage(CurrentStage - 1);
        }
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