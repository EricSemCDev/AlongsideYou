using UnityEngine;

// Controla o tempo da fase atual e o que acontece ao completá-la:
// transiciona o GameState, congela o tempo, e avança pra próxima
// fase via PhaseProgressionManager.
public class PhaseCompletionManager : MonoBehaviour
{
    public static PhaseCompletionManager Instance { get; private set; }

    public float CurrentElapsedTime { get; private set; }
    public bool IsCompleted { get; private set; }

    [Header("Estrelas: tempo maximo (em segundos) para cada nota")]
    [SerializeField] private float threeStarsTime = 30f;
    [SerializeField] private float twoStarsTime = 60f;
    [SerializeField] private float oneStarTime = 90f;

    [Header("Tela de resultado")]
    [SerializeField] private PhaseResultScreen resultScreen;
    [Tooltip("Desligue no Prologo: sem estrelas, vai direto para a proxima fase.")]
    [SerializeField] private bool showResultScreen = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (IsCompleted)
        {
            return;
        }

        if (!GameManager.IsPlaying || TutorialSequencePlayer.IsAnyPlaying)
        {
            return; // pausado/menu — o tempo não corre
        }

        CurrentElapsedTime += Time.deltaTime;
    }

    // Quanto mais rapido, mais estrelas. Passou do tempo de 1 estrela: zero.
    private int CalculateStars(float time)
    {
        if (time <= threeStarsTime) return 3;
        if (time <= twoStarsTime) return 2;
        if (time <= oneStarTime) return 1;
        return 0;
    }

    public void CompletePhase()
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;

        int stars = CalculateStars(CurrentElapsedTime);

        GameManager.Instance?.TryTransitionTo(GameState.Complete);
        PhaseProgressionManager.Instance?.CompleteCurrentPhase(stars);

        // Sem tela de resultado (Prologo, ou ainda nao montada): segue direto para a proxima fase.
        if (showResultScreen && resultScreen != null)
        {
            resultScreen.Show(stars);
        }
        else
        {
            PhaseProgressionManager.Instance?.GoToNextPhase();
        }
    }
}