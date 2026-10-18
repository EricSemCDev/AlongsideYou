using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Centraliza troca de cena com fade suave pra preto, em vez de corte
// seco. Todo carregamento de cena do jogo deveria passar por aqui.
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [Tooltip("Segundos de tela preta depois de carregar a cena, para o engasgo do carregamento (Awake/Start, particulas) passar antes do fade-in.")]
    [SerializeField] private float postLoadHold = 0.15f;
    [Tooltip("Maximo que um frame pode avancar o fade. Impede que um engasgo faca o fade pular de uma vez.")]
    [SerializeField] private float maxFrameTime = 0.04f;

    private bool _isTransitioning;

    public bool IsTransitioning => _isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // A copia duplicada se destroi no fim do frame; desativa ja, senao o Canvas
            // dela (salvo com alpha 1) aparece como um piscar de preto.
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    public void LoadScene(string sceneName)
    {
        // Ignora pedidos enquanto ja esta trocando de cena (duplo clique, dois
        // eventos de fim de fase...), senao duas corrotinas brigam pelo fade.
        if (_isTransitioning)
        {
            return;
        }

        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        _isTransitioning = true;

        yield return Fade(0f, 1f); // escurece

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        if (loadOperation == null)
        {
            Debug.LogWarning($"SceneTransitionManager: cena '{sceneName}' nao encontrada (confira o Build Settings).");
            yield return Fade(1f, 0f);
            _isTransitioning = false;
            yield break;
        }

        while (!loadOperation.isDone)
        {
            yield return null; // espera o carregamento pesado terminar de verdade
        }

        // O Awake/Start da cena nova (e as particulas com prewarm) rodam agora e
        // causam um engasgo. Segura o preto ate estabilizar, para o fade-in comecar
        // com frames normais.
        yield return null;
        yield return null;
        yield return new WaitForSecondsRealtime(postLoadHold);

        yield return Fade(1f, 0f); // clareia de volta

        _isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeCanvasGroup == null)
        {
            yield break;
        }

        fadeCanvasGroup.blocksRaycasts = true; // impede clique durante a transição

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Mathf.Min(Time.unscaledDeltaTime, maxFrameTime);
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;

        if (to <= 0f)
        {
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}