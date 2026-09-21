using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Porta o algoritmo do Forge (repositório Equation-Generator) para
// dentro da Unity. Roda automaticamente ao carregar a fase, gerando
// uma equação nova a cada sessão — evita memorização (decisão 10/03).
public class PuzzleGenerator : MonoBehaviour
{
    [SerializeField] private PuzzleConfig config;

    private readonly System.Random _random = new();

    private void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        if (config == null)
        {
            Debug.LogWarning("PuzzleGenerator: nenhum PuzzleConfig atribuído. Nada foi gerado.");
            return;
        }

        List<char> pool = GenerateOperatorPool(config);
        List<char> selectedOperators = SelectOperators(config, pool);
        (List<List<int>> blockFaces, int result) = GenerateBlocks(config, selectedOperators);

        ApplyToScene(blockFaces, pool, result);
    }

    // --- Faixas por dificuldade (fiel ao Forge) ---

    private (int min, int max) GetSlotRange(float difficulty)
    {
        if (difficulty <= 2) return (1, 4);
        if (difficulty <= 5) return (5, 9);
        return (0, 9);
    }

    private bool ShouldGenerateComposite(float difficulty)
    {
        if (difficulty < 6) return false;
        float chance = 0.20f + (difficulty - 6) * 0.15f;
        return _random.NextDouble() < chance;
    }

    private int GetRmax(float difficulty)
    {
        if (difficulty <= 2) return 10;
        if (difficulty <= 5) return 50;
        if (difficulty <= 7) return 100;
        return 500;
    }

    private bool CanBeNegative(float difficulty) => difficulty >= 8;

    // --- Operadores ---

    private List<char> GenerateOperatorPool(PuzzleConfig cfg)
    {
        List<char> easyOps = new() { '+', '-' };
        List<char> hardOps = new() { '*', '/' };
        List<char> pool = new();

        float hardChance;
        if (cfg.difficulty <= 3) hardChance = 0.20f;
        else if (cfg.difficulty <= 7) hardChance = 0.50f;
        else hardChance = 0.80f;

        while (pool.Count < cfg.operatorCount)
        {
            char op = _random.NextDouble() < hardChance
                ? hardOps[_random.Next(0, hardOps.Count)]
                : easyOps[_random.Next(0, easyOps.Count)];

            int currentCount = pool.Count(o => o == op);

            if (currentCount < cfg.operatorSlots)
            {
                pool.Add(op);
            }
        }

        return pool;
    }

    private List<char> SelectOperators(PuzzleConfig cfg, List<char> pool)
    {
        List<char> selected = new();

        while (true)
        {
            selected.Clear();

            for (int i = 0; i < cfg.operatorSlots; i++)
            {
                int index = _random.Next(0, pool.Count);
                selected.Add(pool[index]);
            }

            bool invalid = false;

            for (int i = 0; i < selected.Count - 1; i++)
            {
                if (selected[i] == '/' && selected[i + 1] == '/')
                {
                    invalid = true;
                    break;
                }
            }

            if (!invalid)
            {
                return selected;
            }
        }
    }

    // --- Blocos e equação ---

    private (List<List<int>> blockFaces, int result) GenerateBlocks(PuzzleConfig cfg, List<char> operators)
    {
        int rMax = GetRmax(cfg.difficulty);
        bool canBeNegative = CanBeNegative(cfg.difficulty);

        while (true)
        {
            List<int> slotValues = new();
            List<int> slotDigits = new(); // dígitos individuais para plantar nos blocos

            for (int i = 0; i < cfg.slotCount; i++)
            {
                if (cfg.compositeSlots[i])
                {
                    int d1 = _random.Next(1, 10); // primeiro dígito (1-9, evita 0 na frente)
                    int d2 = _random.Next(0, 10);
                    int composite = d1 * 10 + d2;
                    slotValues.Add(composite);
                    slotDigits.Add(d1);
                    slotDigits.Add(d2);
                }
                else
                {
                    (int min, int max) = GetSlotRange(cfg.difficulty);
                    int val = _random.Next(min, max + 1);
                    slotValues.Add(val);
                    slotDigits.Add(val);
                }
            }

            // Avalia a equação da esquerda pra direita, sem prioridade
            float resultado = slotValues[0];
            bool invalid = false;

            for (int i = 0; i < operators.Count; i++)
            {
                switch (operators[i])
                {
                    case '+':
                        resultado += slotValues[i + 1];
                        break;
                    case '-':
                        resultado -= slotValues[i + 1];
                        break;
                    case '*':
                        resultado *= slotValues[i + 1];
                        break;
                    case '/':
                        if (slotValues[i + 1] == 0)
                        {
                            invalid = true;
                        }
                        else
                        {
                            resultado = (float)Math.Round(resultado / slotValues[i + 1]);
                        }
                        break;
                }

                if (invalid)
                {
                    break;
                }
            }

            if (invalid)
            {
                continue;
            }

            int r = (int)resultado;

            if (!canBeNegative && r < 0)
            {
                continue;
            }

            if (r > rMax)
            {
                continue;
            }

            // Planta 1 dígito por bloco
            List<List<int>> blockFaces = new();

            for (int i = 0; i < cfg.blockCount; i++)
            {
                blockFaces.Add(new List<int>());
            }

            for (int i = 0; i < slotDigits.Count; i++)
            {
                blockFaces[i].Add(slotDigits[i]);
            }

            // Preenche faces restantes sem repetir, depois embaralha
            foreach (List<int> faces in blockFaces)
            {
                while (faces.Count < cfg.facesPerBlock)
                {
                    int randomFace = _random.Next(0, 10);

                    if (!faces.Contains(randomFace))
                    {
                        faces.Add(randomFace);
                    }
                }

                for (int i = faces.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(0, i + 1);
                    (faces[i], faces[j]) = (faces[j], faces[i]);
                }
            }

            return (blockFaces, r);
        }
    }

    // --- Aplicação na cena ---

    private void ApplyToScene(List<List<int>> blockFaces, List<char> operatorPool, int result)
    {
        DadoBlock[] sceneBlocks = FindObjectsByType<DadoBlock>(FindObjectsSortMode.None);
        OperatorSpark[] sceneSparks = FindObjectsByType<OperatorSpark>(FindObjectsSortMode.None);

        if (sceneBlocks.Length != blockFaces.Count)
        {
            Debug.LogWarning(
                $"PuzzleGenerator: esperava {blockFaces.Count} blocos na cena, encontrou {sceneBlocks.Length}."
            );
        }

        if (sceneSparks.Length != operatorPool.Count)
        {
            Debug.LogWarning(
                $"PuzzleGenerator: esperava {operatorPool.Count} faíscas-operador na cena, encontrou {sceneSparks.Length}."
            );
        }

        int blockCountToApply = Mathf.Min(sceneBlocks.Length, blockFaces.Count);

        for (int i = 0; i < blockCountToApply; i++)
        {
            sceneBlocks[i].SetFaces(blockFaces[i]);
        }

        int sparkCountToApply = Mathf.Min(sceneSparks.Length, operatorPool.Count);

        for (int i = 0; i < sparkCountToApply; i++)
        {
            sceneSparks[i].SetSymbol(operatorPool[i]);
        }

        config.targetResult = result;
    }
}