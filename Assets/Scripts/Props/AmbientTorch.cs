using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Tocha de ambiente: queima por um tempo, avisa diminuindo a luz, apaga, e
// so volta a acender se o Finn ficar dentro do trigger ate o fim do tempo
// de reacender. Se ele sair antes, o progresso zera. O estado e lido pelo
// raio da luz (sem barra nem HUD): a luz so cresce enquanto ele acende.
//
// O AnxietySystem considera uma luz "segura" pelo raio dela. Como o raio vai
// a ~0 quando a tocha apaga, o Al deixa de ser aliviado sem nenhuma mudanca
// no AnxietySystem.
//
// Requer um Collider2D marcado como Trigger neste objeto (o Finn precisa ter
// Rigidbody2D, o que ele ja tem).
public class AmbientTorch : MonoBehaviour
{
    [SerializeField] private Light2D torchLight;

    [Header("Burn")]
    [SerializeField] private bool startLit = true;
    [Tooltip("Desmarque para a tocha nunca apagar (ex: fases de tutorial). Ela continua podendo comecar apagada.")]
    [SerializeField] private bool canBurnOut = true;
    [Tooltip("Tempo aceso (segundos), sorteado entre min e max, para as tochas nao apagarem juntas.")]
    [SerializeField] private float minBurnTime = 25f;
    [SerializeField] private float maxBurnTime = 40f;
    [Tooltip("Nos ultimos segundos antes de apagar, o raio da luz vai diminuindo.")]
    [SerializeField] private float warningDuration = 5f;

    [Header("Relight")]
    [Tooltip("Quanto tempo o Finn precisa ficar dentro do trigger para reacender.")]
    [SerializeField] private float relightDuration = 3f;

    [Header("Light")]
    [Tooltip("Raio quando apagada. Perto de 0 para o Al nao ser aliviado.")]
    [SerializeField] private float extinguishedRadius = 0f;
    [Tooltip("Velocidade (unidades/s) com que o raio acompanha o estado (reset suave ao sair do trigger).")]
    [SerializeField] private float radiusChangeSpeed = 8f;

    [Header("Animator (optional)")]
    [SerializeField] private Animator animator;
    [Tooltip("Parametro Bool: ligado = acesa, desligado = apagada.")]
    [SerializeField] private string litParameter = "Lit";

    private readonly HashSet<Collider2D> _finnColliders = new();

    private float _litOuterRadius;
    private float _innerRatio;
    private float _currentRadius;
    private float _burnTimeRemaining;
    private float _relightProgress;
    private int _litParameterId;

    public bool IsLit { get; private set; }

    // Disparado quando a tocha acende (true) ou apaga (false).
    public event Action<bool> OnLitChanged;

    private void Awake()
    {
        if (torchLight == null)
        {
            torchLight = GetComponentInChildren<Light2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _litParameterId = Animator.StringToHash(litParameter);

        if (torchLight != null)
        {
            // O raio que esta no prefab e o raio "aceso".
            _litOuterRadius = torchLight.pointLightOuterRadius;
            _innerRatio = _litOuterRadius > 0f ? torchLight.pointLightInnerRadius / _litOuterRadius : 0f;
        }

        IsLit = startLit;
        _burnTimeRemaining = RollBurnTime();
        _relightProgress = 0f;
        _currentRadius = IsLit ? _litOuterRadius : extinguishedRadius;

        ApplyRadius();
        ApplyAnimator();
    }

    private void OnDisable()
    {
        _finnColliders.Clear();
    }

    private void Update()
    {
        // Pausado, no menu ou fim de fase: o tempo da tocha nao corre.
        if (GameManager.Instance == null
            || !InputAccessPolicy.CanAcceptInput(GameManager.Instance.CurrentState))
        {
            return;
        }

        float targetRadius;

        if (IsLit && !canBurnOut)
        {
            // Tocha eterna: nao gasta tempo de queima, mantem o raio aceso.
            targetRadius = _litOuterRadius;
        }
        else if (IsLit)
        {
            _burnTimeRemaining -= Time.deltaTime;

            if (_burnTimeRemaining <= 0f)
            {
                SetLit(false);
                targetRadius = extinguishedRadius;
            }
            else if (_burnTimeRemaining < warningDuration)
            {
                // Aviso: o raio encolhe ate apagar.
                float t = Mathf.Clamp01(_burnTimeRemaining / Mathf.Max(warningDuration, 0.01f));
                targetRadius = Mathf.Lerp(extinguishedRadius, _litOuterRadius, t);
            }
            else
            {
                targetRadius = _litOuterRadius;
            }
        }
        else
        {
            if (_finnColliders.Count > 0)
            {
                _relightProgress += Time.deltaTime / Mathf.Max(relightDuration, 0.01f);
            }
            else
            {
                _relightProgress = 0f; // saiu do trigger: nao acende, recomeca do zero
            }

            if (_relightProgress >= 1f)
            {
                SetLit(true);
                targetRadius = _litOuterRadius;
            }
            else
            {
                targetRadius = Mathf.Lerp(extinguishedRadius, _litOuterRadius, _relightProgress);
            }
        }

        _currentRadius = Mathf.MoveTowards(_currentRadius, targetRadius, radiusChangeSpeed * Time.deltaTime);
        ApplyRadius();
    }

    private void SetLit(bool lit)
    {
        if (IsLit == lit)
        {
            return;
        }

        IsLit = lit;
        _relightProgress = 0f;

        if (lit)
        {
            _burnTimeRemaining = RollBurnTime();
        }

        ApplyAnimator();
        OnLitChanged?.Invoke(lit);
    }

    private float RollBurnTime()
    {
        return UnityEngine.Random.Range(Mathf.Min(minBurnTime, maxBurnTime), Mathf.Max(minBurnTime, maxBurnTime));
    }

    private void ApplyRadius()
    {
        if (torchLight == null)
        {
            return;
        }

        torchLight.pointLightOuterRadius = _currentRadius;
        torchLight.pointLightInnerRadius = _currentRadius * _innerRatio;
    }

    private void ApplyAnimator()
    {
        if (animator != null)
        {
            animator.SetBool(_litParameterId, IsLit);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<FinnMovement>() != null)
        {
            _finnColliders.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _finnColliders.Remove(other);
    }
}
