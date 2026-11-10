using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Sons de hover e clique para qualquer Button. E colocado SOZINHO pelo
// MenuGamepadNavigation em todo botao que nao tem UiButtonHighlight (esse ja toca os
// sons), entao menus novos ou botoes esquecidos tambem fazem barulho.
// Botao cujo nome tem "voltar" ou "back" toca o som de voltar no clique.
public class UiButtonSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private float _enabledAt;
    private bool _isBack;
    private bool _pointerInside;
    private Selectable _selectable;

    private void Awake()
    {
        _selectable = GetComponent<Selectable>();
        string lowerName = name.ToLowerInvariant();
        _isBack = lowerName.Contains("voltar") || lowerName.Contains("back");

        var button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(PlayClick);
        }
    }

    private void OnEnable()
    {
        _enabledAt = Time.unscaledTime;
        _pointerInside = false; // o menu pode ter fechado com o mouse em cima
    }

    // Teclado/controle: mudar a selecao toca o hover. Com o mouse em cima, o hover ja tocou
    // ao entrar; o clique tambem seleciona o botao, e tocar de novo seria um som a mais.
    public void OnSelect(BaseEventData eventData)
    {
        if (!_pointerInside)
        {
            PlayHover();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerInside = true;
        PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerInside = false;
    }

    // Nao toca nos primeiros instantes: quando o menu abre e o primeiro botao e selecionado sozinho.
    private void PlayHover()
    {
        if (_selectable != null && _selectable.interactable && Time.unscaledTime - _enabledAt > 0.2f)
        {
            FeedbackAudio.Play(FeedbackSounds.Cue.UiHover);
        }
    }

    private void PlayClick()
    {
        FeedbackAudio.Play(_isBack ? FeedbackSounds.Cue.UiBack : FeedbackSounds.Cue.UiClick);
    }
}
