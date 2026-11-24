using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnxietySystem : MonoBehaviour
{
    [SerializeField] private AnxietyConfig config;
    [SerializeField] private List<Light2D> safetyLights = new();

    private float _elapsedInCurrentZone;
    private bool _wasInLightLastFrame;

    public int CurrentStage { get; private set; } = 1;

    public event System.Action<int> OnStageChanged;

    private void Update()
    {
        if (!GameManager.IsPlaying || TutorialSequencePlayer.IsAnyPlaying)
        {
            return;
        }

        bool isInLight = IsNearAnySafetyLight();

        if (isInLight != _wasInLightLastFrame)
        {
            _elapsedInCurrentZone = 0f;
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