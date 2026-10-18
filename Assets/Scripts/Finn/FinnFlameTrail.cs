using UnityEngine;
using UnityEngine.Rendering.Universal;

// Rastro de chama do Finn: um ParticleSystem (criado por codigo) que solta
// chamas atras dele conforme a velocidade. As particulas ficam no espaco do
// mundo, entao ficam para tras de verdade, e usam material aditivo sem luz,
// entao brilham mesmo no escuro. Opcionalmente faz a luz do Finn tremular
// como fogo (so a INTENSIDADE - o raio nao muda, porque o AnxietySystem usa
// o raio para saber se o Al esta em zona segura).
// Todos os valores podem ser mexidos no Inspector, inclusive em Play Mode.
[RequireComponent(typeof(FinnMovement))]
public class FinnFlameTrail : MonoBehaviour
{
    [Header("Emission")]
    [SerializeField] private float speedThreshold = 0.3f;
    [SerializeField] private float speedForMaxEmission = 6f;
    [SerializeField] private float maxEmissionRate = 40f;
    [SerializeField] private int maxParticles = 200;

    [Header("Flame")]
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private float startSize = 0.25f;
    [SerializeField] private float spawnRadius = 0.1f;
    [SerializeField] private float riseSpeed = 0.8f;
    [SerializeField] private float turbulence = 0.5f;
    [SerializeField] private Gradient colorOverLifetime = CreateDefaultGradient();

    [Header("Rendering")]
    [Tooltip("Opcional. Se vazio, usa um material aditivo criado por codigo.")]
    [SerializeField] private Material flameMaterial;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Header("Light Flicker")]
    [SerializeField] private bool flickerLight = true;
    [SerializeField] private Light2D glowLight;
    [Tooltip("Variacao maxima de intensidade para cima e para baixo.")]
    [SerializeField] private float flickerAmount = 0.08f;
    [SerializeField] private float flickerSpeed = 8f;

    private static Material _sharedAdditive;

    private FinnMovement _finnMovement;
    private ParticleSystem _particles;
    private ParticleSystemRenderer _particlesRenderer;
    private float _baseLightIntensity;

    private void Awake()
    {
        _finnMovement = GetComponent<FinnMovement>();

        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (glowLight == null)
        {
            glowLight = GetComponentInChildren<Light2D>();
        }

        if (glowLight != null)
        {
            _baseLightIntensity = glowLight.intensity;
        }

        BuildParticleSystem();
        ApplySettings();
        _particles.Play();
    }

    private void OnDisable()
    {
        if (glowLight != null)
        {
            glowLight.intensity = _baseLightIntensity;
        }
    }

    private void OnValidate()
    {
        // Permite mexer nos valores em Play Mode e ver o resultado na hora.
        if (_particles != null)
        {
            ApplySettings();
        }
    }

    private void Update()
    {
        float speed = _finnMovement.CurrentSpeed;

        var emission = _particles.emission;
        emission.rateOverTime = speed < speedThreshold
            ? 0f
            : maxEmissionRate * Mathf.Clamp01(speed / speedForMaxEmission);

        UpdateLightFlicker();
    }

    private void LateUpdate()
    {
        UpdateSorting();
    }

    private void UpdateLightFlicker()
    {
        if (glowLight == null)
        {
            return;
        }

        if (!flickerLight)
        {
            glowLight.intensity = _baseLightIntensity;
            return;
        }

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.37f) - 0.5f;
        glowLight.intensity = _baseLightIntensity + (noise * 2f * flickerAmount);
    }

    // Cria o objeto e o ParticleSystem uma unica vez. Configuracoes que nao
    // podem mudar com o sistema tocando (duracao, loop) ficam aqui.
    private void BuildParticleSystem()
    {
        var go = new GameObject("FinnFlameParticles");
        go.transform.SetParent(transform, false);

        _particles = go.AddComponent<ParticleSystem>();
        _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = _particles.main;
        main.playOnAwake = false;
        main.loop = true;
        main.duration = 1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = 0f;

        var emission = _particles.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;

        var shape = _particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;

        _particlesRenderer = go.GetComponent<ParticleSystemRenderer>();
        _particlesRenderer.renderMode = ParticleSystemRenderMode.Billboard;
    }

    // Valores ajustaveis pelo Inspector. Pode rodar de novo a qualquer hora.
    private void ApplySettings()
    {
        var main = _particles.main;
        main.startLifetime = lifetime;
        main.startSize = new ParticleSystem.MinMaxCurve(startSize * 0.7f, startSize);
        main.maxParticles = maxParticles;

        var shape = _particles.shape;
        shape.radius = spawnRadius;

        var velocity = _particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(0f);
        velocity.y = new ParticleSystem.MinMaxCurve(riseSpeed);
        velocity.z = new ParticleSystem.MinMaxCurve(0f);

        var noise = _particles.noise;
        noise.enabled = turbulence > 0f;
        noise.strength = turbulence;
        noise.frequency = 1.5f;
        noise.scrollSpeed = 1f;

        var color = _particles.colorOverLifetime;
        color.enabled = true;
        color.color = new ParticleSystem.MinMaxGradient(colorOverLifetime);

        var size = _particles.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        _particlesRenderer.sharedMaterial = flameMaterial != null ? flameMaterial : GetAdditiveMaterial();

        UpdateSorting();
    }

    // Acompanha a ordem do Finn (que muda com o Y-sorting), ficando logo atras dele.
    private void UpdateSorting()
    {
        if (targetSpriteRenderer == null || _particlesRenderer == null)
        {
            return;
        }

        _particlesRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;
        _particlesRenderer.sortingOrder = targetSpriteRenderer.sortingOrder - 1;
    }

    // Material aditivo compartilhado (tambem usado pelo FinnTrailEffect).
    // Sem interacao com Light2D: por isso brilha no escuro.
    public static Material GetAdditiveMaterial()
    {
        if (_sharedAdditive != null)
        {
            return _sharedAdditive;
        }

        Shader shader = Shader.Find("Legacy Shaders/Particles/Additive");

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        _sharedAdditive = new Material(shader)
        {
            name = "FinnFlameAdditive",
            mainTexture = CreateCircleTexture()
        };

        return _sharedAdditive;
    }

    // Circulo com borda suave, gerado por codigo (sem asset externo).
    private static Texture2D CreateCircleTexture()
    {
        const int size = 32;

        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        var pixels = new Color[size * size];
        float center = (size - 1) / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                float alpha = Mathf.SmoothStep(0f, 1f, 1f - distance);
                pixels[(y * size) + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private static Gradient CreateDefaultGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.95f, 0.6f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0.1f), 0.4f),
                new GradientColorKey(new Color(0.7f, 0.1f, 0f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0.6f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        return gradient;
    }
}
