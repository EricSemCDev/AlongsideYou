using UnityEngine;

// Efeito de quando a barreira de espinhos abre (faiscas magenta e tremor medio).
// Colocado sozinho pelo PuzzleFeedback em cada ThornBarrier.
[DisallowMultipleComponent]
public class BarrierFeedback : MonoBehaviour
{
    private ThornBarrier _barrier;

    private void OnEnable()
    {
        _barrier = GetComponent<ThornBarrier>();

        if (_barrier != null)
        {
            _barrier.OnOpened += HandleOpened;
        }
    }

    private void OnDisable()
    {
        if (_barrier != null)
        {
            _barrier.OnOpened -= HandleOpened;
        }
    }

    private void HandleOpened()
    {
        GameJuice.BarrierOpened(transform.position);
    }
}
