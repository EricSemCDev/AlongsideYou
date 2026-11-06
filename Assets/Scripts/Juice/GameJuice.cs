using System.Collections;
using TMPro;
using UnityEngine;

// "Suco" do jogo: pequenos efeitos que dao peso as acoes (faiscas, tremor,
// pulinho, som). Tudo e chamado por uma linha de onde o evento acontece, e e
// criado por codigo (sem prefab nem cena para montar).
//
// - Acertou a conta: explosao de faiscas douradas, numero pulsando (sem tremor).
// - Errou: tremor curto e faiscas vermelhas.
// - Bloco encaixou: pulinho do bloco e nuvenzinha de poeira.
// - Tocha de operador acendeu: clarao de faiscas.
// - Barreira abriu: tremor medio e faiscas magenta.
// - Pegou a Fagulha: festa de faiscas.
// Sons: ver FeedbackSounds. Tremor: ver CameraShake (pode ser desligado).
public static class GameJuice
{
    private static readonly Color Gold = new Color(1f, 0.82f, 0.3f, 1f);
    private static readonly Color Warm = new Color(1f, 0.55f, 0.15f, 1f);
    private static readonly Color Red = new Color(1f, 0.25f, 0.2f, 1f);
    private static readonly Color Magenta = new Color(0.9f, 0.3f, 0.85f, 1f);
    private static readonly Color Dust = new Color(0.75f, 0.65f, 0.55f, 0.55f);

    private static JuiceRunner _runner;

    // --- Eventos do jogo -----------------------------------------------------

    public static void ResultCorrect(Vector3 position, Transform label)
    {
        Burst(position, Gold, 26, 1.6f, 0.9f);
        Burst(position, Warm, 12, 0.9f, 0.7f);
        Punch(label, 1.35f, 0.28f);
        FeedbackAudio.Play(FeedbackSounds.Cue.ResultCorrect);
    }

    public static void ResultWrong(Vector3 position, Transform label)
    {
        Burst(position, Red, 14, 1.1f, 0.5f);
        Punch(label, 1.25f, 0.2f);
        CameraShake.Shake(0.1f, 0.22f);
        FeedbackAudio.Play(FeedbackSounds.Cue.ResultWrong);
    }

    public static void AttemptsExhausted()
    {
        CameraShake.Shake(0.25f, 0.5f);
        FeedbackAudio.Play(FeedbackSounds.Cue.AttemptsExhausted);
    }

    public static void BlockSnapped(Transform block)
    {
        if (block == null)
        {
            return;
        }

        Burst(block.position + (Vector3.down * 0.2f), Dust, 7, 0.7f, 0.35f);
        Punch(block, 1.22f, 0.16f);
        FeedbackAudio.Play(FeedbackSounds.Cue.BlockSnap);
    }

    public static void TorchLit(Vector3 position)
    {
        Burst(position + (Vector3.up * 0.3f), Gold, 16, 1.2f, 0.6f);
        FeedbackAudio.Play(FeedbackSounds.Cue.TorchLit);
    }

    public static void BarrierOpened(Vector3 position)
    {
        Burst(position, Magenta, 30, 1.8f, 0.9f);
        CameraShake.Shake(0.12f, 0.3f);
        FeedbackAudio.Play(FeedbackSounds.Cue.BarrierOpen);
    }

    public static void Victory(Vector3 position)
    {
        Burst(position, Gold, 50, 2.6f, 1.2f);
        Burst(position, Color.white, 20, 1.8f, 0.9f);
        FeedbackAudio.Play(FeedbackSounds.Cue.Victory);
    }

    // --- Ferramentas ---------------------------------------------------------

    // Cresce e volta ao tamanho (como um "pulinho"). Usa tempo nao escalado.
    public static void Punch(Transform target, float peakScale, float duration)
    {
        if (target != null)
        {
            Runner.StartCoroutine(PunchRoutine(target, peakScale, duration));
        }
    }

    // Explosao unica de particulas, criada por codigo e apagada sozinha.
    public static void Burst(Vector3 position, Color color, int count, float speed, float lifetime)
    {
        var go = new GameObject("JuiceBurst");
        go.transform.position = position;

        var system = go.AddComponent<ParticleSystem>();
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = system.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.5f;
        main.useUnscaledTime = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.6f, lifetime);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.4f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.14f);
        main.startColor = color;
        main.gravityModifier = 0.15f;
        main.maxParticles = count + 4;

        var emission = system.emission;
        emission.rateOverTime = 0f;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)count));

        var shape = system.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.12f;

        var sizeOverLifetime = system.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var colorOverLifetime = system.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var fade = new Gradient();
        fade.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fade);

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = FinnFlameTrail.GetAdditiveMaterial();
        renderer.sortingLayerID = SortingLayer.NameToID("Effects");
        renderer.sortingOrder = 60;

        system.Play();
        Object.Destroy(go, lifetime + 1f);
    }

    private static IEnumerator PunchRoutine(Transform target, float peakScale, float duration)
    {
        Vector3 baseScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < duration && target != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(elapsed / duration);
            target.localScale = baseScale * (1f + ((peakScale - 1f) * Mathf.Sin(u * Mathf.PI)));
            yield return null;
        }

        if (target != null)
        {
            target.localScale = baseScale;
        }
    }

    private static JuiceRunner Runner
    {
        get
        {
            if (_runner == null)
            {
                var go = new GameObject("GameJuice");
                Object.DontDestroyOnLoad(go);
                _runner = go.AddComponent<JuiceRunner>();
            }

            return _runner;
        }
    }

    internal static AudioSource Audio => Runner.Audio;
}
