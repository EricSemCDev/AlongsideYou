using UnityEngine;
using UnityEngine.SceneManagement;

// Ao atingir o Estágio 5, dispara a derrota: transiciona pra GameOver
// e recarrega a fase inteira do zero, sem checkpoints. Só reage ao
// evento — não sabe nada sobre timer ou luz.
public class AnxietyDefeatTrigger : MonoBehaviour
{
    [SerializeField] private AnxietySystem anxietySystem;

    private void Awake()
    {
        if (anxietySystem == null)
        {
            anxietySystem = GetComponent<AnxietySystem>();
        }
    }

    private void OnEnable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged += HandleStageChanged;
        }
    }

    private void OnDisable()
    {
        if (anxietySystem != null)
        {
            anxietySystem.OnStageChanged -= HandleStageChanged;
        }
    }

    private void HandleStageChanged(int stage)
    {
        if (stage >= 5)
        {
            TriggerDefeat();
        }
    }

    private void TriggerDefeat()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryTransitionTo(GameState.GameOver);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryTransitionTo(GameState.Playing);
        }
    }
}