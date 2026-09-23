using System.Collections.Generic;
using UnityEngine;

public class FinnOperatorCollector : MonoBehaviour
{
    // Os 4 operadores sempre existem como categoria, mesmo com 0
    // coletados — é isso que permite o menu sempre mostrar os 4
    // quadrantes, só diferenciando visualmente os que estão em 0.
    private readonly Dictionary<char, int> _operatorCounts = new()
    {
        { '+', 0 },
        { '-', 0 },
        { '*', 0 },
        { '/', 0 },
    };

    private readonly HashSet<OperatorSlotMarker> _nearbyTorches = new();
    private OperatorSlotMarker _highlightedTorch;

    public int GetCount(char symbol)
    {
        return _operatorCounts.TryGetValue(symbol, out int count) ? count : 0;
    }

    public bool TryConsume(char symbol)
    {
        if (GetCount(symbol) <= 0)
        {
            return false;
        }

        _operatorCounts[symbol]--;
        return true;
    }

    public void Return(char symbol)
    {
        if (_operatorCounts.ContainsKey(symbol))
        {
            _operatorCounts[symbol]++;
        }
    }

    private void Update()
    {
        if (InputManager.Instance == null || OperatorMenuController.Instance == null)
        {
            return;
        }

        if (OperatorMenuController.Instance.IsOpen)
        {
            return; // já tem um menu aberto, não abre outro por cima
        }

        UpdateTorchHighlight();

        if (!InputManager.Instance.FinnSelectOperatorPressed)
        {
            return;
        }

        OperatorSlotMarker closestTorch = FindClosestTorch();

        if (closestTorch != null)
        {
            closestTorch.SetHighlighted(false);
            _highlightedTorch = null;
            OperatorMenuController.Instance.Show(closestTorch.transform.position, closestTorch, this);
        }
    }

    private void UpdateTorchHighlight()
    {
        OperatorSlotMarker closest = FindClosestTorch();

        if (closest == _highlightedTorch)
        {
            return;
        }

        _highlightedTorch?.SetHighlighted(false);
        _highlightedTorch = closest;
        _highlightedTorch?.SetHighlighted(true);
    }

    private OperatorSlotMarker FindClosestTorch()
    {
        OperatorSlotMarker closest = null;
        float closestDistance = float.MaxValue;

        foreach (OperatorSlotMarker torch in _nearbyTorches)
        {
            float distance = Vector2.Distance(transform.position, torch.transform.position);

            if (distance < closestDistance)
            {
                closest = torch;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out OperatorSpark spark))
        {
            if (_operatorCounts.ContainsKey(spark.Symbol))
            {
                _operatorCounts[spark.Symbol]++;
            }

            Destroy(spark.gameObject);
        }

        if (other.TryGetComponent(out OperatorSlotMarker torch))
        {
            _nearbyTorches.Add(torch);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out OperatorSlotMarker torch))
        {
            _nearbyTorches.Remove(torch);

            if (torch == _highlightedTorch)
            {
                torch.SetHighlighted(false);
                _highlightedTorch = null;
            }
        }
    }
}