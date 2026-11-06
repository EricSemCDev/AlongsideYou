using UnityEngine;

// Tremor de camera curto, para dar peso a momentos do jogo (erro na conta,
// barreira abrindo...). Chame CameraShake.Shake(amplitude, duracao) de qualquer
// lugar: o componente se coloca sozinho na camera principal.
//
// Convive com o foco de camera do tutorial: o deslocamento do tremor e
// desfeito no comeco de cada quadro e refeito no fim, entao nunca "gruda" a
// camera fora do lugar. Os pulos sao arredondados para 1 pixel da arte (1/16),
// o que combina com pixel art.
[DefaultExecutionOrder(-1000)]
public class CameraShake : MonoBehaviour
{
    private const string PrefKey = "AlongsideYou.Juice.ShakeEnabled";
    private const float PixelsPerUnit = 16f;

    private static CameraShake _instance;

    // Opcao de acessibilidade ("reduzir movimento"). Pode ser ligada num menu de configuracoes.
    public static bool Enabled
    {
        get => PlayerPrefs.GetInt(PrefKey, 1) == 1;
        set => PlayerPrefs.SetInt(PrefKey, value ? 1 : 0);
    }

    // amplitude em unidades do mundo (0.1 = ~1,6 pixel), duracao em segundos.
    public static void Shake(float amplitude, float duration)
    {
        if (!Enabled || amplitude <= 0f || duration <= 0f)
        {
            return;
        }

        Camera camera = Camera.main;

        if (camera == null)
        {
            return;
        }

        if (_instance == null || _instance.gameObject != camera.gameObject)
        {
            _instance = camera.GetComponent<CameraShake>();

            if (_instance == null)
            {
                _instance = camera.gameObject.AddComponent<CameraShake>();
            }
        }

        _instance.Add(amplitude, duration);
    }

    private float _amplitude;
    private float _duration;
    private float _elapsed;
    private Vector3 _applied;

    private float Remaining => _duration <= 0f ? 0f : _amplitude * Mathf.Pow(1f - Mathf.Clamp01(_elapsed / _duration), 2f);

    private void Add(float amplitude, float duration)
    {
        // Um tremor novo so substitui o atual se for mais forte do que o que ainda resta dele.
        if (amplitude >= Remaining)
        {
            _amplitude = amplitude;
            _duration = duration;
            _elapsed = 0f;
        }
    }

    // Antes de qualquer outro script: tira o deslocamento do quadro anterior.
    private void Update()
    {
        transform.position -= _applied;
        _applied = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (_elapsed >= _duration)
        {
            return;
        }

        _elapsed += Time.unscaledDeltaTime;

        Vector2 offset = Random.insideUnitCircle * Remaining;
        offset.x = Mathf.Round(offset.x * PixelsPerUnit) / PixelsPerUnit;
        offset.y = Mathf.Round(offset.y * PixelsPerUnit) / PixelsPerUnit;

        _applied = new Vector3(offset.x, offset.y, 0f);
        transform.position += _applied;
    }

    private void OnDisable()
    {
        transform.position -= _applied;
        _applied = Vector3.zero;
    }
}
