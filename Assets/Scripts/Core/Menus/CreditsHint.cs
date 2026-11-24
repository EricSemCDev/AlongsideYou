using TMPro;
using UnityEngine;

// Dica dos creditos em texto puro, que muda conforme o dispositivo em uso (teclado ou controle).
[RequireComponent(typeof(TMP_Text))]
public class CreditsHint : MonoBehaviour
{
    private TMP_Text _label;

    private void OnEnable()
    {
        _label = GetComponent<TMP_Text>();
        InputPromptResolver.DeviceChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        InputPromptResolver.DeviceChanged -= Refresh;
    }

    private void Refresh()
    {
        InputPromptResolver.Device device = InputPromptResolver.Current;
        string speedUp = device == InputPromptResolver.Device.KeyboardMouse ? "S" : "Seta para baixo";
        string back = device switch
        {
            InputPromptResolver.Device.Xbox => "B",
            InputPromptResolver.Device.PlayStation => "O",
            InputPromptResolver.Device.Switch => "A",
            _ => "ESC"
        };

        _label.text = "Segure " + speedUp + " para acelerar   |   " + back + " para voltar";
    }
}
