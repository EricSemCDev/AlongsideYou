using System.Collections;
using UnityEngine;

// Move e da zoom na camera de forma suave durante o tutorial, usando tempo nao
// escalado (funciona com o jogo congelado). Lembra o enquadramento original e
// volta para ele quando o tutorial termina.
// A camera da fase e fixa, entao e so mover posicao e tamanho ortografico.
public class TutorialCameraFocus : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _homePosition;
    private float _homeSize;
    private bool _hasHome;
    private Coroutine _move;
    private float _progress = 1f;

    // 0..1 do movimento atual (1 = parado / chegou).
    public float Progress => _progress;
    public bool IsMoving => _move != null;

    public static TutorialCameraFocus GetOrCreate()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            return null;
        }

        TutorialCameraFocus focus = cam.GetComponent<TutorialCameraFocus>();

        if (focus == null)
        {
            focus = cam.gameObject.AddComponent<TutorialCameraFocus>();
        }

        return focus;
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnDisable()
    {
        _move = null;
        _progress = 1f;
    }

    // Enquadra um ponto do mundo. zoomSize <= 0 mantem o zoom atual.
    public void FocusOn(Vector3 worldPosition, float zoomSize, float minDuration)
    {
        if (!_hasHome)
        {
            _homePosition = transform.position;
            _homeSize = _camera.orthographicSize;
            _hasHome = true;
        }

        Vector3 target = new Vector3(worldPosition.x, worldPosition.y, _homePosition.z);
        float size = zoomSize > 0f ? zoomSize : _camera.orthographicSize;
        StartMove(target, size, minDuration);
    }

    public void ReturnHome(float minDuration)
    {
        if (!_hasHome)
        {
            return;
        }

        StartMove(_homePosition, _homeSize, minDuration);
        _hasHome = false;
    }

    private void StartMove(Vector3 targetPosition, float targetSize, float minDuration)
    {
        if (_move != null)
        {
            StopCoroutine(_move);
        }

        _move = StartCoroutine(MoveRoutine(targetPosition, targetSize, minDuration));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, float targetSize, float minDuration)
    {
        Vector3 startPosition = transform.position;
        float startSize = _camera.orthographicSize;

        // Quanto mais longe, mais devagar e demorado (nunca brusco).
        float distance = Vector2.Distance(startPosition, targetPosition) + (Mathf.Abs(targetSize - startSize) * 0.6f);
        float duration = distance < 0.05f ? 0f : Mathf.Clamp(minDuration + (distance * 0.03f), minDuration, minDuration * 1.5f);

        float elapsed = 0f;
        _progress = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _progress = t;

            // Suavizacao "smootherstep": comeca e termina bem devagar.
            float eased = t * t * t * (t * ((t * 6f) - 15f) + 10f);

            transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, eased);
            _camera.orthographicSize = Mathf.LerpUnclamped(startSize, targetSize, eased);
            yield return null;
        }

        transform.position = targetPosition;
        _camera.orthographicSize = targetSize;
        _progress = 1f;
        _move = null;
    }
}
