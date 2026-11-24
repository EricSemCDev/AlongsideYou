using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [SerializeField] private PuzzleConfig config;
    [SerializeField] private int maxGenerationAttempts = 1000;
    [SerializeField] private int maxOperatorRetries = 30;

    private readonly System.Random _random = new();

    private PuzzleConfig _runtimeConfig;

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

        PuzzleConfig cfg = CreateRuntimeConfig();

        List<char> pool = null;
        List<char> selectedOperators = null;
        List<List<int>> blockFaces = null;
        int result = 0;
        bool success = false;

        for (int operatorTry = 0; operatorTry < maxOperatorRetries && !success; operatorTry++)
        {
            pool = GenerateOperatorPool(cfg);
            selectedOperators = SelectOperators(cfg, pool);
            success = TryGenerateBlocks(cfg, selectedOperators, out blockFaces, out result);
        }

        if (!success)
        {
            Debug.LogWarning(
                $"PuzzleGenerator: não encontrou solução em {maxGenerationAttempts} tentativas " +
                "com os parâmetros atuais da fase. Usando equação fixa de segurança " +
                "(mitigação do risco de balanceamento, seção 15). Considere ajustar o PuzzleConfig."
            );

            pool = GenerateFallbackOperatorPool(cfg);
            selectedOperators = GenerateFallbackOperators(cfg);
            (blockFaces, result) = GenerateFallbackBlocks(cfg);
        }

        ApplyToScene(cfg, blockFaces, pool, result);
    }

    // --- Faixas por dificuldade (fiel ao Forge) ---

    private (int min, int max) GetSlotRange(float difficulty)
    {
        if (difficulty <= 2) return (1, 4);
        if (difficulty <= 5) return (5, 9);
        return (0, 9);
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

        int maxPerOperator = Mathf.Max(cfg.operatorSlots, Mathf.CeilToInt(cfg.operatorCount / 4f));

        while (pool.Count < cfg.operatorCount)
        {
            char op = _random.NextDouble() < hardChance
                ? hardOps[_random.Next(0, hardOps.Count)]
                : easyOps[_random.Next(0, easyOps.Count)];

            int currentCount = pool.Count(o => o == op);

            if (currentCount < maxPerOperator)
            {
                pool.Add(op);
            }
        }

        return pool;
    }

    private List<char> SelectOperators(PuzzleConfig cfg, List<char> pool)
    {
        List<char> selected = new();

        for (int attempt = 0; attempt < 100; attempt++)
        {
            List<char> shuffled = new(pool);

            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }

            selected = shuffled.GetRange(0, Mathf.Min(cfg.operatorSlots, shuffled.Count));

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

        return selected;
    }

    // --- Blocos e equação ---

    private bool TryGenerateBlocks(PuzzleConfig cfg, List<char> operators, out List<List<int>> blockFaces, out int result)
    {
        int rMax = GetRmax(cfg.difficulty);
        bool canBeNegative = CanBeNegative(cfg.difficulty);

        for (int attempt = 0; attempt < maxGenerationAttempts; attempt++)
        {
            List<int> slotValues = new();
            List<int> slotDigits = new();

            for (int i = 0; i < cfg.slotCount; i++)
            {
                if (cfg.compositeSlots[i])
                {
                    int d1 = _random.Next(1, 10);
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
                    {
                        int divisor = slotValues[i + 1];

                        if (divisor == 0 || resultado == 0f || resultado % divisor != 0f)
                        {
                            invalid = true;
                        }
                        else
                        {
                            resultado /= divisor;
                        }

                        break;
                    }
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

            List<List<int>> faces = new();

            for (int i = 0; i < cfg.blockCount; i++)
            {
                faces.Add(new List<int>());
            }

            for (int i = 0; i < slotDigits.Count; i++)
            {
                faces[i].Add(slotDigits[i]);
            }

            FillAndShuffleFaces(faces, cfg.facesPerBlock);

            blockFaces = faces;
            result = r;
            return true;
        }

        blockFaces = null;
        result = 0;
        return false;
    }

    private void FillAndShuffleFaces(List<List<int>> blockFaces, int facesPerBlock)
    {
        foreach (List<int> faces in blockFaces)
        {
            while (faces.Count < facesPerBlock)
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
    }

    private List<char> GenerateFallbackOperatorPool(PuzzleConfig cfg)
    {
        List<char> safeOps = new() { '+', '-' };
        List<char> pool = new();
        int opIndex = 0;

        int maxPerOperator = Mathf.Max(cfg.operatorSlots, Mathf.CeilToInt(cfg.operatorCount / 2f));

        while (pool.Count < cfg.operatorCount)
        {
            char op = safeOps[opIndex % safeOps.Count];
            int currentCount = pool.Count(o => o == op);

            if (currentCount < maxPerOperator)
            {
                pool.Add(op);
            }

            opIndex++;
        }

        return pool;
    }

    private List<char> GenerateFallbackOperators(PuzzleConfig cfg)
    {
        List<char> operators = new();

        for (int i = 0; i < cfg.operatorSlots; i++)
        {
            operators.Add('+');
        }

        return operators;
    }

    private (List<List<int>> blockFaces, int result) GenerateFallbackBlocks(PuzzleConfig cfg)
    {
        List<int> slotValues = new();
        List<int> slotDigits = new();

        for (int i = 0; i < cfg.slotCount; i++)
        {
            if (cfg.compositeSlots[i])
            {
                slotValues.Add(10);
                slotDigits.Add(1);
                slotDigits.Add(0);
            }
            else
            {
                slotValues.Add(1);
                slotDigits.Add(1);
            }
        }

        int result = slotValues.Sum();

        List<List<int>> blockFaces = new();

        for (int i = 0; i < cfg.blockCount; i++)
        {
            blockFaces.Add(new List<int>());
        }

        for (int i = 0; i < slotDigits.Count; i++)
        {
            blockFaces[i].Add(slotDigits[i]);
        }

        FillAndShuffleFaces(blockFaces, cfg.facesPerBlock);

        return (blockFaces, result);
    }

    // --- Aplicação na cena ---

    private void ApplyToScene(PuzzleConfig cfg, List<List<int>> blockFaces, List<char> operatorPool, int result)
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

        cfg.targetResult = result;

        foreach (CalculationStationDisplay station in FindObjectsByType<CalculationStationDisplay>(FindObjectsSortMode.None))
        {
            if (station.Config == config || station.Config == cfg)
            {
                station.UseConfig(cfg);
            }
        }
    }

    private PuzzleConfig CreateRuntimeConfig()
    {
        if (_runtimeConfig != null)
        {
            Destroy(_runtimeConfig);
        }

        _runtimeConfig = Instantiate(config);
        _runtimeConfig.name = config.name + " (em jogo)";
        return _runtimeConfig;
    }

    private void OnDestroy()
    {
        if (_runtimeConfig != null)
        {
            Destroy(_runtimeConfig);
        }
    }
}