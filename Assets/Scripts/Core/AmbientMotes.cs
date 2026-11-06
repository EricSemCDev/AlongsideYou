using UnityEngine;

// Pontinhos de luz flutuando devagar no vazio do fundo (poeira / vaga-lumes).
// Cria o ParticleSystem por codigo e o mantem centrado na camera. As particulas
// ficam no espaco do mundo, entao a camera se move "por entre" elas, o que da
// profundidade ao fundo. Fica ATRAS do chao (Order in Layer negativo), entao so
// aparece no vazio e entre as ilhas.
// Coloque em um objeto vazio da fase (ex: "FundoPoeira"). Tudo ajustavel no
// Inspector, inclusive em Play Mode.
public class AmbientMotes : MonoBehaviour
{
    [Header("Area (em unidades, em volta da camera)")]
    [SerializeField] private Vector2 areaSize = new Vector2(44f, 26f);

    [Header("Motes")]
    [SerializeField] private int maxParticles = 160;
    [SerializeField] private float lifetime = 14f;
    [Tooltip("Tamanho em unidades. Com PPU 16, 0.0625 = 1 pixel.")]
    [SerializeField] private Vector2 sizeRange = new Vector2(0.0625f, 0.125f);
    [SerializeField] private float riseSpeed = 0.12f;
    [SerializeField] private float drift = 0.35f;
    [SerializeField] private Gradient colorOverLifetime = CreateDefaultGradient();

    [Header("Look")]
    [Tooltip("Quadradinhos duros (pixel art). Desligado: circulos suaves.")]
    [SerializeField] private bool squareMotes = true;

    [Header("Rendering")]
    [SerializeField] private string sortingLayerName = "Default";
    [Tooltip("Negativo = atras do chao (que esta em 0).")]
    [SerializeField] private int sortingOrder = -50;

    [Header("Pausa")]
    [Tooltip("Ligado: continua flutuando com o jogo pausado (ambiente vivo). Desligado: congela junto com o jogo.")]
    [SerializeField] private bool keepMovingWhenPaused = false;

    private ParticleSystem _particles;
    private ParticleSystemRenderer _renderer;
    private Material _material;
    private Texture2D _squareTexture;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        Build();
        ApplySettings();
        _particles.Play();
    }

    private void OnValidate()
    {
        if (_particles != null)
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

        if (_squareTexture != null)
        {
            Destroy(_squareTexture);
        }
    }

    // Mantem a area centrada na camera, para sempre haver motes na tela.
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

        Vector3 cameraPosition = _camera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0f);
    }

    private void Build()
    {
        var go = new GameObject("Motes");
        go.transform.SetParent(transform, false);

        _particles = go.AddComponent<ParticleSystem>();
        _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = _particles.main;
        main.playOnAwake = false;
        main.loop = true;
        main.duration = 10f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed = 0f;
        main.prewarm = true; // ja comeca com a tela cheia, sem esperar encher
        main.useUnscaledTime = keepMovingWhenPaused;

        var shape = _particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;

        _renderer = go.GetComponent<ParticleSystemRenderer>();
        _renderer.renderMode = ParticleSystemRenderMode.Billboard;
    }

    private void ApplySettings()
    {
        var main = _particles.main;
        main.startLifetime = lifetime;
        main.startSize = new ParticleSystem.MinMaxCurve(sizeRange.x, sizeRange.y);
        main.maxParticles = maxParticles;

        // Taxa para manter a tela aproximadamente com maxParticles.
        var emission = _particles.emission;
        emission.enabled = true;
        emission.rateOverTime = maxParticles / Mathf.Max(lifetime, 0.1f);

        var shape = _particles.shape;
        shape.scale = new Vector3(areaSize.x, areaSize.y, 0.1f);

        var velocity = _particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(0f);
        velocity.y = new ParticleSystem.MinMaxCurve(riseSpeed);
        velocity.z = new ParticleSystem.MinMaxCurve(0f);

        var noise = _particles.noise;
        noise.enabled = drift > 0f;
        noise.strength = drift;
        noise.frequency = 0.35f;
        noise.scrollSpeed = 0.2f;

        var color = _particles.colorOverLifetime;
        color.enabled = true;
        color.color = new ParticleSystem.MinMaxGradient(colorOverLifetime);

        _renderer.sharedMaterial = GetMaterial();
        _renderer.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
        _renderer.sortingOrder = sortingOrder;
    }

    // Material aditivo do FinnFlameTrail (circulo suave) ou uma copia com um
    // pixel branco duro (quadradinhos).
    private Material GetMaterial()
    {
        Material shared = FinnFlameTrail.GetAdditiveMaterial();

        if (!squareMotes)
        {
            return shared;
        }

        if (_material == null)
        {
            _squareTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            _squareTexture.SetPixel(0, 0, Color.white);
            _squareTexture.Apply();

            _material = new Material(shared) { name = "AmbientMotesSquare", mainTexture = _squareTexture };
        }

        return _material;
    }

    private static Gradient CreateDefaultGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.55f, 0.65f, 1f), 0f),
                new GradientColorKey(new Color(0.7f, 0.6f, 1f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.35f, 0.25f),
                new GradientAlphaKey(0.35f, 0.75f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        return gradient;
    }
}
