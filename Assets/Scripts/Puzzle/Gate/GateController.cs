using UnityEngine;

// Barreira física simples: só liga/desliga o collider e o sprite.
// Não sabe nada sobre condições, proximidade ou cast — isso é
// responsabilidade do GateCastActivator.
public class GateController : MonoBehaviour
{
    [SerializeField] private Collider2D barrierCollider;
    [SerializeField] private SpriteRenderer barrierVisual;

    public bool IsOpen { get; private set; }

    public void SetOpen(bool open)
    {
        if (IsOpen == open)
        {
            return;
        }

        IsOpen = open;

        if (barrierCollider != null)
        {
            barrierCollider.enabled = !open;
        }

        if (barrierVisual != null)
        {
            barrierVisual.enabled = !open;
        }
    }
}