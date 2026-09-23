using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Faz um texto sumir gradualmente conforme se afasta de TODAS as
// fontes de luz próximas (Finn, Al, etc.) — o mais próximo "vence".
// Substitui a necessidade de shader Lit no TextMeshPro. Lê o raio
// direto do componente Light2D real, evitando duplicar/dessincronizar
// esse número num campo separado.
public class ProximityVisibility : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private List<Light2D> lightSources = new();

    private void Awake()
    {
        if (label == null)
        {
            label = GetComponent<TextMeshPro>();
        }

        if (lightSources.Count == 0)
        {
            AutoFindPointLights();
        }
    }

    // Sem nada configurado manualmente, encontra sozinho todas as luzes
    // do tipo Point na cena (ignora a Global Light 2D, que não tem
    // raio de alcance real).
    private void AutoFindPointLights()
    {
        Light2D[] allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);

        foreach (Light2D light in allLights)
        {
            if (light.lightType == Light2D.LightType.Point)
            {
                lightSources.Add(light);
            }
        }
    }

    private void Update()
    {
        if (label == null)
        {
            return;
        }

        float bestVisibility = 0f;

        foreach (Light2D light in lightSources)
        {
            if (light == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, light.transform.position);
            float visibility = CalculateVisibility(distance, light.pointLightInnerRadius, light.pointLightOuterRadius);

            if (visibility > bestVisibility)
            {
                bestVisibility = visibility;
            }
        }

        Color color = label.color;
        color.a = bestVisibility;
        label.color = color;
    }

    private float CalculateVisibility(float distance, float innerRadius, float outerRadius)
    {
        float range = outerRadius - innerRadius;

        if (range <= 0f)
        {
            return distance <= outerRadius ? 1f : 0f;
        }

        return Mathf.Clamp01((outerRadius - distance) / range);
    }
}