using UnityEngine;
using UnityEngine.Rendering.Universal;

// Faz a luz tremular como fogo, mexendo SO na intensidade (nunca no raio:
// o AlAnxietyLightEffect e o AnxietySystem dependem do raio). Reutilizavel
// em qualquer Light2D: Al, tochas, etc.
// Opcionalmente a tremulacao fica mais forte e mais rapida conforme a
// ansiedade do Al sobe (estagio 1 = sutil, estagio 5 = nervosa).
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light2D targetLight;

    [Header("Flicker")]
    [SerializeField] private bool flickerEnabled = true;
    [Tooltip("Variacao maxima de intensidade, como fracao da intensidade base (0.1 = +-10%).")]
    [SerializeField] private float amount = 0.12f;
    [SerializeField] private float speed = 6f;

    [Header("Anxiety")]
    [Tooltip("Se ligado, amount e speed vao dos valores acima (estagio 1) ate os valores abaixo (estagio 5).")]
    [SerializeField] private bool linkToAnxiety;
    [SerializeField] private AnxietySystem anxietySystem;
    [SerializeField] private float amountAtMaxAnxiety = 0.35f;
    [SerializeField] private float speedAtMaxAnxiety = 14f;
    [Tooltip("Quao rapido amount/speed acompanham a mudanca de estagio (evita saltos bruscos).")]
    [SerializeField] private float anxietyBlendSpeed = 2f;

    private float _baseIntensity;
    private float _phase;
    private float _seed;
    private float _currentAmount;
    private float _currentSpeed;
    private float _anxietyT; // 0 = estagio 1, 1 = estagio 5

    private void Awake()
    {
        if (targetLight == null)
        {
            targetLight = GetComponentInChildren<Light2D>();
        }

        if (anxietySystem == null)
        {
            anxietySystem = GetComponentInParent<AnxietySystem>();
        }

        if (targetLight != null)
        {
            _baseIntensity = targetLight.intensity;
        }

        // Cada luz comeca em um ponto diferente do ruido, para duas luzes
        // com o mesmo script nao tremularem em sincronia.
        _seed = Random.value * 100f;
        _currentAmount = amount;
        _currentSpeed = speed;
    }

    private void OnEnable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged += HandleStageChanged;
            HandleStageChanged(anxietySystem.CurrentStage);
        }
    }

    private void OnDisable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged -= HandleStageChanged;
        }

        if (targetLight != null)
        {
            targetLight.intensity = _baseIntensity;
        }
    }

    private void HandleStageChanged(int stage)
    {
        _anxietyT = Mathf.InverseLerp(1, 5, stage);
    }

    private void Update()
    {
        if (targetLight == null)
        {
            return;
        }

        if (!flickerEnabled)
        {
            targetLight.intensity = _baseIntensity;
            return;
        }

        float targetAmount = linkToAnxiety ? Mathf.Lerp(amount, amountAtMaxAnxiety, _anxietyT) : amount;
        float targetSpeed = linkToAnxiety ? Mathf.Lerp(speed, speedAtMaxAnxiety, _anxietyT) : speed;

        float blend = 1f - Mathf.Exp(-anxietyBlendSpeed * Time.deltaTime);
        _currentAmount = Mathf.Lerp(_currentAmount, targetAmount, blend);
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, blend);

        // Fase acumulada (e nao Time.time * speed): mudar a velocidade no
        // meio do caminho nao faz o ruido "pular".
        _phase += Time.deltaTime * _currentSpeed;

        float noise = Mathf.PerlinNoise(_phase, _seed) - 0.5f;
        float intensity = _baseIntensity * (1f + (noise * 2f * _currentAmount));
        targetLight.intensity = Mathf.Max(0f, intensity);
    }
}
