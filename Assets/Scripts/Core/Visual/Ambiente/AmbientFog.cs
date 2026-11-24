using System.Collections.Generic;
using UnityEngine;

// Neblina em camadas no vazio do fundo, sem nenhum sprite: manchas grandes e
// bem transparentes, com borda "em degraus" (visual de pixel art), flutuando
// devagar. Cada camada pode seguir a camera so em parte (Camera Follow), o que
// da parallax: camada longe anda menos que a de perto.
// Fica atras do chao (Order in Layer negativo): aparece no vazio e entre as
// ilhas. Coloque em um objeto vazio da fase (ex: "Neblina"). Tudo ajustavel no
// Inspector, inclusive em Play Mode.
public class AmbientFog : MonoBehaviour
{
    [System.Serializable]
    public class FogLayerSettings
    {
        public string name = "Camada";
        [Tooltip("Quantas manchas ao mesmo tempo.")]
        public int count = 8;
        [Tooltip("Tamanho das manchas, em unidades.")]
        public Vector2 sizeRange = new Vector2(3f, 5.5f);
        [Tooltip("Quanto tempo cada mancha vive (segundos). Maior = mais devagar aparecer/sumir.")]
        public float lifetime = 30f;
        [Tooltip("Velocidade da deriva (unidades por segundo).")]
        public Vector2 velocity = new Vector2(0.1f, 0f);
        public Color color = new Color(0.5f, 0.45f, 0.9f, 1f);
        [Range(0f, 1f)] public float alpha = 0.08f;
        [Tooltip("0 = parada no mundo, 1 = colada na camera. Valores entre dao parallax.")]
        [Range(0f, 1f)] public float cameraFollow = 0.85f;
        [Tooltip("Negativo = atras do chao (que esta em 0). Menor = mais ao fundo.")]
        public int sortingOrder = -55;
    }

    [SerializeField] private Vector2 areaSize = new Vector2(60f, 36f);
    [SerializeField] private string sortingLayerName = "Default";

    [SerializeField] private List<FogLayerSettings> layers = new List<FogLayerSettings>
    {
        new FogLayerSettings
        {
            name = "Longe",
            count = 10,
            sizeRange = new Vector2(5f, 8f),
            lifetime = 40f,
            velocity = new Vector2(0.05f, 0f),
            color = new Color(0.35f, 0.4f, 0.9f, 1f),
            alpha = 0.07f,
            cameraFollow = 0.7f,
            sortingOrder = -70
        },
        new FogLayerSettings
        {
            name = "Perto",
            count = 8,
            sizeRange = new Vector2(3f, 5.5f),
            lifetime = 30f,
            velocity = new Vector2(0.12f, 0.02f),
            color = new Color(0.5f, 0.35f, 0.9f, 1f),
            alpha = 0.09f,
            cameraFollow = 0.9f,
            sortingOrder = -55
        }
    };

    [Header("Pausa")]
    [Tooltip("Ligado: continua flutuando com o jogo pausado (ambiente vivo). Desligado: congela junto com o jogo.")]
    [SerializeField] private bool keepMovingWhenPaused = false;

    private readonly List<ParticleSystem> _systems = new List<ParticleSystem>();
    private Material _material;
    private Texture2D _texture;
    private Camera _camera;
    private Vector3 _cameraStart;

    private void Awake()
    {
        _camera = Camera.main;
        _cameraStart = _camera != null ? _camera.transform.position : Vector3.zero;
        _cameraStart.z = 0f;

        _texture = CreateSteppedCircle();
        _material = new Material(FinnFlameTrail.GetAdditiveMaterial())
        {
            name = "AmbientFog",
            mainTexture = _texture
        };

        foreach (FogLayerSettings layer in layers)
        {
            _systems.Add(BuildSystem(layer));
        }

        ApplySettings();

        foreach (ParticleSystem system in _systems)
        {
            system.Play();
        }
    }

    private void OnValidate()
    {
        if (_systems.Count > 0)
        {
            ApplySettings();
        }
    }

    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
        }

        if (_texture != null)
        {
            Destroy(_texture);
        }
    }

    // Cada camada segue a camera so em parte, a partir da posicao inicial dela.
    private void LateUpdate()
    {
        if (_camera == null)
        {
            _camera = Camera.main;

            if (_camera == null)
            {
                return;
            }
        }

        Vector3 delta = _camera.transform.position - _cameraStart;
        delta.z = 0f;

        for (int i = 0; i < _systems.Count && i < layers.Count; i++)
        {
            _systems[i].transform.position = _cameraStart + (delta * layers[i].cameraFollow);
        }
    }

    private ParticleSystem BuildSystem(FogLayerSettings layer)
    {
        var go = new GameObject("Fog_" + layer.name);
        go.transform.SetParent(transform, false);

        ParticleSystem system = go.AddComponent<ParticleSystem>();
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = system.main;
        main.playOnAwake = false;
        main.loop = true;
        main.duration = 10f;
        main.prewarm = true; // ja comeca com a tela cheia de neblina
        main.useUnscaledTime = keepMovingWhenPaused;
        main.startSpeed = 0f;
        // Local: as manchas andam junto com o emissor, e e o emissor que segue a
        // camera so em parte (parallax).
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var shape = system.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        return system;
    }

    private void ApplySettings()
    {
        for (int i = 0; i < _systems.Count && i < layers.Count; i++)
        {
            FogLayerSettings layer = layers[i];
            ParticleSystem system = _systems[i];

            var main = system.main;
            main.startLifetime = layer.lifetime;
            main.startSize = new ParticleSystem.MinMaxCurve(layer.sizeRange.x, layer.sizeRange.y);
            main.maxParticles = Mathf.Max(1, layer.count + 4);

            var emission = system.emission;
            emission.enabled = true;
            emission.rateOverTime = layer.count / Mathf.Max(layer.lifetime, 0.1f);

            var shape = system.shape;
            shape.scale = new Vector3(areaSize.x, areaSize.y, 0.1f);

            var velocity = system.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.x = new ParticleSystem.MinMaxCurve(layer.velocity.x);
            velocity.y = new ParticleSystem.MinMaxCurve(layer.velocity.y);
            velocity.z = new ParticleSystem.MinMaxCurve(0f);

            var color = system.colorOverLifetime;
            color.enabled = true;
            color.color = new ParticleSystem.MinMaxGradient(CreateFadeGradient(layer));

            var renderer = system.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = _material;
            renderer.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
            renderer.sortingOrder = layer.sortingOrder;
        }
    }

    // Aparece e some devagar: transparente -> alpha da camada -> transparente.
    private static Gradient CreateFadeGradient(FogLayerSettings layer)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(layer.color, 0f), new GradientColorKey(layer.color, 1f) },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(layer.alpha, 0.3f),
                new GradientAlphaKey(layer.alpha, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        return gradient;
    }

    // Circulo "em degraus": miolo mais denso e dois aneis mais fracos, com
    // filtro Point. Ampliado, vira blocos grandes (pixel art), nao degrade liso.
    private static Texture2D CreateSteppedCircle()
    {
        const int size = 32;

        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        var pixels = new Color[size * size];
        float center = (size - 1) / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                float alpha = d < 0.45f ? 1f : d < 0.75f ? 0.55f : d < 1f ? 0.22f : 0f;
                pixels[(y * size) + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
