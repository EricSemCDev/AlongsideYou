using TMPro;
using UnityEngine;

// Exibição temporária do tempo de fase na tela (Screen Space UI),
// até a "Tela de conclusão" de verdade existir (issue futura).
public class PhaseTimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private Color completedColor = Color.green;

    private void Update()
    {
        if (timeLabel == null || PhaseCompletionManager.Instance == null)
        {
            return;
        }

        float time = PhaseCompletionManager.Instance.CurrentElapsedTime;
        timeLabel.text = $"Tempo: {time:0.0}s";

        if (PhaseCompletionManager.Instance.IsCompleted)
        {
            timeLabel.color = completedColor;
        }
    }
}