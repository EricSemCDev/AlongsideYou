using UnityEngine;

// Dispara uma sequencia de tutorial quando o Al (ou o Finn) entra na area.
// Coloque em um objeto com um Collider2D marcado como "Is Trigger" e arraste o
// TutorialSequencePlayer. Com "Only Once" ligado, dispara uma vez e se desliga.
[RequireComponent(typeof(Collider2D))]
public class TutorialTriggerZone : MonoBehaviour
{
    [SerializeField] private TutorialSequencePlayer sequence;
    [SerializeField] private bool triggeredByAl = true;
    [SerializeField] private bool triggeredByFinn = false;
    [SerializeField] private bool onlyOnce = true;

    private bool _used;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (sequence == null || (onlyOnce && _used))
        {
            return;
        }

        bool isAl = triggeredByAl && other.GetComponentInParent<AlMovement>() != null;
        bool isFinn = triggeredByFinn && other.GetComponentInParent<FinnMovement>() != null;

        if (!isAl && !isFinn)
        {
            return;
        }

        _used = true;
        sequence.Play();
    }
}
