using UnityEngine;

// Abre quando Al e Finn estão, ao mesmo tempo, cada um na sua própria
// placa de pressão — reforça o Pilar 1 do GDD ("nenhuma fase pode ser
// completada por um só"), sem depender de matemática nenhuma.
public class PartnershipGate : MonoBehaviour
{
    [SerializeField] private GateController gate;
    [SerializeField] private PressurePlate alPlate;
    [SerializeField] private PressurePlate finnPlate;

    private void Update()
    {
        if (gate == null || alPlate == null || finnPlate == null)
        {
            return;
        }

        gate.SetOpen(alPlate.IsOccupied && finnPlate.IsOccupied);
    }
}