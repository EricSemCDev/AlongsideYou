using UnityEngine;

// Detecta contato do Al com a Fagulha de Luz e conclui a fase.
public class AlFagulhaCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out FagulhaDeLuz fagulha))
        {
            GameJuice.Victory(fagulha.transform.position);
            PhaseCompletionManager.Instance?.CompletePhase();
            Destroy(fagulha.gameObject);
        }
    }
}