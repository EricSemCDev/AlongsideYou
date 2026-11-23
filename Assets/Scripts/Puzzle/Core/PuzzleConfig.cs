using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleConfig", menuName = "Alongside You/Puzzle Config")]
public class PuzzleConfig : ScriptableObject
{
    [Header("Blocos (N, F)")]
    public int blockCount = 2;
    public int facesPerBlock = 4;

    [Header("Equação (S, OS, CS)")]
    public int slotCount = 2;
    public int operatorSlots = 1;
    public List<bool> compositeSlots = new();

    [Header("Operadores (OC)")]
    public int operatorCount = 1;

    [Header("Dificuldade (D)")]
    [Range(1f, 10f)] public float difficulty = 1f;

    [Header("Resultado-alvo")]
    public int targetResult;

    private void OnValidate()
    {
        Validate();
    }

    public void Validate()
    {
        ResizeCompositeSlots();

        if (slotCount < 2)
        {
            Debug.LogWarning($"[{name}] slotCount ({slotCount}) está abaixo do mínimo do Forge (2).", this);
        }

        if (operatorSlots < 1)
        {
            Debug.LogWarning($"[{name}] operatorSlots ({operatorSlots}) está abaixo do mínimo do Forge (1).", this);
        }

        int compositeCount = CountComposite();
        int minimumBlocks = slotCount + compositeCount;

        if (blockCount < minimumBlocks)
        {
            Debug.LogWarning(
                $"[{name}] blockCount ({blockCount}) é menor que o necessário " +
                $"({minimumBlocks} = slotCount + slots compostos). A fase não é jogável assim.",
                this
            );
        }

        if (operatorCount < operatorSlots)
        {
            Debug.LogWarning(
                $"[{name}] operatorCount ({operatorCount}) é menor que operatorSlots " +
                $"({operatorSlots}). O pool precisa ser igual ou maior que o usado na equação.",
                this
            );
        }
    }

    private void ResizeCompositeSlots()
    {
        while (compositeSlots.Count < slotCount)
        {
            compositeSlots.Add(false);
        }

        while (compositeSlots.Count > slotCount)
        {
            compositeSlots.RemoveAt(compositeSlots.Count - 1);
        }
    }

    private int CountComposite()
    {
        int count = 0;

        foreach (bool isComposite in compositeSlots)
        {
            if (isComposite)
            {
                count++;
            }
        }

        return count;
    }
}