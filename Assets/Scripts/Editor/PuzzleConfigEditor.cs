using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleConfig))]
public class PuzzleConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Preencher a partir da cena"))
        {
            FillFromScene((PuzzleConfig)target);
        }
    }

    private void FillFromScene(PuzzleConfig config)
    {
        var blocks = FindObjectsByType<DadoBlock>(FindObjectsSortMode.None);
        var slots = FindObjectsByType<NumericSlot>(FindObjectsSortMode.None);
        var operatorSlotMarkers = FindObjectsByType<OperatorSlotMarker>(FindObjectsSortMode.None);
        var operatorSparks = FindObjectsByType<OperatorSpark>(FindObjectsSortMode.None);

        Undo.RecordObject(config, "Preencher PuzzleConfig a partir da cena");

        // Contagem honesta, refletindo exatamente o que existe na cena.
        // Qualquer ajuste de mínimo necessário é responsabilidade exclusiva
        // de PuzzleConfig.Validate(), chamado logo abaixo.
        config.blockCount = blocks.Length;
        config.slotCount = slots.Length;
        config.operatorSlots = operatorSlotMarkers.Length;
        config.operatorCount = operatorSparks.Length;

        config.compositeSlots = new List<bool>();
        foreach (NumericSlot slot in slots)
        {
            config.compositeSlots.Add(slot.IsComposite);
        }

        config.Validate(); // agora só emite avisos, nunca muda os valores contados

        EditorUtility.SetDirty(config);

        Debug.Log(
            $"[{config.name}] Preenchido a partir da cena: " +
            $"blockCount={config.blockCount}, slotCount={config.slotCount}, " +
            $"operatorSlots={config.operatorSlots}, operatorCount={config.operatorCount}, " +
            $"compositeSlots com {config.compositeSlots.Count} entradas."
        );
    }
}