using System.Collections;
using TMPro;
using UnityEngine;

// Cartao com o nome da fase: desce do topo da tela ate o centro, fica um tempo
// e some num fade. Toca sozinho quando a fase comeca, DEPOIS do fade de entrada
// da cena (espera o SceneTransitionManager terminar). Usa tempo nao escalado.
//
// Montagem: um Canvas (Screen Space Overlay) com um filho "Card" centralizado
// (ancora no centro) contendo o CanvasGroup e os textos. Arraste aqui.
public class PhaseTitleCard : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("O objeto que se move (centralizado: ancora no centro da tela).")]
    [SerializeField] private RectTransform card;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text titleLabel;
    [Tooltip("Opcional: segunda linha (ex: o nome da fase).")]
    [SerializeField] private TMP_Text subtitleLabel;

    [Header("Texto")]
    [Tooltip("Se vazio e 'Auto Number Title' ligado, mostra 'Fase N'.")]
    [SerializeField] private string title = "";
    [SerializeField] private string subtitle = "";
    [SerializeField] private bool autoNumberTitle = true;
    [SerializeField] private string autoNumberFormat = "Fase {0}";

    [Header("Tempo (segundos)")]
    [SerializeField] private float startDelay = 0.3f;
    [SerializeField] private float slideDuration = 0.7f;
    [SerializeField] private float holdDuration = 1.6f;
    [SerializeField] private float fadeDuration = 0.8f;

    [Header("Movimento")]
    [Tooltip("Altura final do cartao, em unidades do canvas, a partir do centro da tela. 0 = no meio, positivo = mais para cima, negativo = mais para baixo.")]
    [SerializeField] private float restOffsetY = 0f;
    [Tooltip("Arredonda a posicao para a grade de pixels do canvas (nao borra o pixel art).")]
    [SerializeField] private bool snapToPixels = true;

    private static int _activeCards;
    private bool _active;

    // Verdadeiro enquanto algum cartao de titulo esta aparecendo (o tutorial espera).
    public static bool IsAnyPlaying => _activeCards > 0;

    private void OnDisable()
    {
        SetActive(false);
    }

    private void SetActive(bool active)
    {
        if (_active == active)
        {
            return;
        }

        _active = active;
        _activeCards = Mathf.Max(0, _activeCards + (active ? 1 : -1));
    }

    private void Awake()
    {
        if (group == null && card != null)
        {
            group = card.GetComponent<CanvasGroup>();
        }

        // Comeca escondido: fora da tela e transparente.
        if (group != null)
        {
            group.alpha = 0f;
        }

        if (card != null)
        {
            card.anchoredPosition = new Vector2(0f, StartY());
        }

        ApplyText();
    }

    private void Start()
    {
        StartCoroutine(Play());
    }

    // Pode ser chamado de fora para repetir a animacao.
    public void Replay()
    {
        StopAllCoroutines();
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        if (card == null || group == null)
        {
            yield break;
        }

        SetActive(true);

        // Espera o fade de entrada da cena terminar.
        while (SceneTransitionManager.Instance != null && SceneTransitionManager.Instance.IsTransitioning)
        {
            yield return null;
        }

        card.anchoredPosition = new Vector2(0f, StartY());
        group.alpha = 0f;

        yield return new WaitForSecondsRealtime(startDelay);

        // Desce do topo ate o centro (comeca rapido e desacelera) e aparece.
        float startY = StartY();
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f); // ease-out cubico

            SetY(Mathf.Lerp(startY, restOffsetY, eased));
            group.alpha = Mathf.Clamp01(t * 2f); // aparece na primeira metade da descida
            yield return null;
        }

        SetY(restOffsetY);
        group.alpha = 1f;

        yield return new WaitForSecondsRealtime(holdDuration);

        // Some num fade.
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        group.alpha = 0f;
        SetActive(false);
    }

    private void SetY(float y)
    {
        card.anchoredPosition = new Vector2(0f, snapToPixels ? Mathf.Round(y) : y);
    }

    // Altura do ponto de partida: acima da borda de cima da tela.
    private float StartY()
    {
        Canvas canvas = card.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas != null ? canvas.rootCanvas.transform as RectTransform : null;
        float canvasHeight = canvasRect != null ? canvasRect.rect.height : 270f;

        return (canvasHeight / 2f) + (card.rect.height / 2f);
    }

    private void ApplyText()
    {
        if (titleLabel != null)
        {
            string text = title;

            if (string.IsNullOrEmpty(text) && autoNumberTitle)
            {
                int index = PhaseProgressionManager.Instance != null ? PhaseProgressionManager.Instance.CurrentPhaseIndex : -1;
                text = index >= 0 ? string.Format(autoNumberFormat, index + 1) : "";
            }

            titleLabel.text = text;
        }

        if (subtitleLabel != null)
        {
            subtitleLabel.text = subtitle;
        }
    }
}
