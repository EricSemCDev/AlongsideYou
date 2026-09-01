using UnityEngine;

// Marca um local de encaixe numérico na Estação de Cálculo (issue futura).
// Por enquanto serve só para o PuzzleConfig contar quantos slots existem
// na fase e quantos são compostos (2 dígitos).
public class NumericSlot : MonoBehaviour
{
    [SerializeField] private bool isComposite;

    public bool IsComposite => isComposite;
}