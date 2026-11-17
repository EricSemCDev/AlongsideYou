using UnityEngine;

// Placa de pressão que detecta se um personagem específico (Al e/ou
// Finn) está em cima dela. Só um Al e um Finn existem no jogo, então
// um bool simples de "ocupado" é suficiente, sem precisar contar.
[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool acceptsAl = true;
    [SerializeField] private bool acceptsFinn = true;

    public bool IsOccupied { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Matches(other))
        {
            IsOccupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Matches(other))
        {
            IsOccupied = false;
        }
    }

    private bool Matches(Collider2D other)
    {
        if (acceptsAl && other.GetComponent<AlMovement>() != null)
        {
            return true;
        }

        if (acceptsFinn && other.GetComponent<FinnMovement>() != null)
        {
            return true;
        }

        return false;
    }
}