using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Tela de resultado da fase: as estrelas enchem uma a uma (cada uma com um pulinho) e, no
// fim, aparecem os botoes Proxima fase e Refazer. Montada uma vez no prefab PhaseManager.
public class PhaseResultScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image[] starImages; // as 3 estrelas, em ordem
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;
    [SerializeField] private GameObject buttonsRoot; // contem Proxima e Refazer
    [SerializeField] private Button nextButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private float delayBetweenStars = 0.45f;

    private void Start()
    {
        nextButton.onClick.AddListener(() => PhaseProgressionManager.Instance?.GoToNextPhase());
        retryButton.onClick.AddListener(() => PhaseProgressionManager.Instance?.RestartCurrentPhase());
        panel.SetActive(false);
    }

    public void Show(int stars)
    {
        panel.SetActive(true);
        StartCoroutine(Play(stars));
    }

    private IEnumerator Play(int stars)
    {
        // Invisivel mas ativo: continua ocupando espaco no layout, entao as estrelas nao se mexem.
        CanvasGroup buttons = buttonsRoot.GetComponent<CanvasGroup>();
        if (buttons == null)
        {
            buttons = buttonsRoot.AddComponent<CanvasGroup>();
        }

        buttons.alpha = 0f;
        buttons.interactable = false;
        buttons.blocksRaycasts = false;

        foreach (Image star in starImages)
        {
            star.sprite = emptyStar;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        for (int i = 0; i < stars && i < starImages.Length; i++)
        {
            starImages[i].sprite = fullStar;
            GameJuice.Punch(starImages[i].transform, 1.6f, 0.3f);
            FeedbackAudio.Play(FeedbackSounds.Cue.StarEarned);
            yield return new WaitForSecondsRealtime(delayBetweenStars);
        }

        for (float t = 0f; t < 0.4f; t += Time.unscaledDeltaTime)
        {
            buttons.alpha = t / 0.4f;
            yield return null;
        }

        buttons.alpha = 1f;
        buttons.interactable = true;
        buttons.blocksRaycasts = true;
        EventSystem.current?.SetSelectedGameObject(nextButton.gameObject);
    }
}