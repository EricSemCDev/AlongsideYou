using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Creditos subindo como em filme. Segurar Seta para baixo / S (ou direcional/analogico
// para baixo no controle) acelera. Quando o fim sai da tela, volta sozinho ao menu.
// Esc / B (no controle) tambem voltam, tratados pelo MainMenuUI. Nao usa Espaco, Enter
// nem A para acelerar, para nao acionar botoes selecionados.
// Coloque no CreditsPanel; "text" e o texto dos creditos e "viewport" e a area (com
// Rect Mask 2D) por onde ele passa.
public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float fastMultiplier = 5f;
    [Tooltip("Segundos parado no fim antes de voltar ao menu.")]
    [SerializeField] private float endDelay = 2f;

    private RectTransform _rect;
    private float _endTimer;

    private void OnEnable()
    {
        _rect = text.rectTransform;
        _rect.anchorMin = _rect.anchorMax = new Vector2(0.5f, 1f);
        _rect.pivot = new Vector2(0.5f, 1f);

        float width = viewport.rect.width;
        _rect.sizeDelta = new Vector2(width, text.GetPreferredValues(width, 0f).y);
        _rect.anchoredPosition = new Vector2(0f, -viewport.rect.height); // comeca logo abaixo da area
        _endTimer = 0f;
    }

    private void Update()
    {
        float step = speed * (FastHeld() ? fastMultiplier : 1f) * Time.unscaledDeltaTime;
        float end = _rect.sizeDelta.y; // quando o fim do texto passa do topo da area
        float y = Mathf.Min(_rect.anchoredPosition.y + step, end);
        _rect.anchoredPosition = new Vector2(0f, y);

        if (y >= end)
        {
            _endTimer += Time.unscaledDeltaTime;

            if (_endTimer >= endDelay)
            {
                _endTimer = float.MinValue; // so volta uma vez
                FindFirstObjectByType<MainMenuUI>()?.OnVoltarClicked();
            }
        }
    }

    private static bool FastHeld()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null && (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed))
            || (gamepad != null && (gamepad.dpad.down.isPressed || gamepad.leftStick.down.isPressed));
    }
}
