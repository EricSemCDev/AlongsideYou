using System.Collections.Generic;
using UnityEngine;

public class FinnOperatorCollector : MonoBehaviour
{
    private readonly List<char> _collectedOperators = new();

    public IReadOnlyList<char> CollectedOperators => _collectedOperators;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out OperatorSpark spark))
        {
            _collectedOperators.Add(spark.Symbol);
            Debug.Log($"Coletados: {string.Join(", ", _collectedOperators)}");
            Destroy(spark.gameObject);
        }
    }
}