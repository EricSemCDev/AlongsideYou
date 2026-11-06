using System;
using UnityEngine;
using UnityEngine.Events;

// Uma fala do tutorial: o texto, quem fala e para onde a camera olha.
[Serializable]
public class TutorialStep
{
    [TextArea(2, 4)]
    [Tooltip("O texto da fala. Aceita tags do TextMeshPro (ex: <color=#FFB84D>numero</color>) e marcas de botao: {interact} {rotate} {move} {finn_follow} {finn_wheel} {pause}.")]
    public string text = "";

    [TextArea(2, 4)]
    [Tooltip("Opcional: texto usado quando o jogador esta com o CONTROLE. Vazio = usa o texto normal (as marcas {interact} etc. ja trocam o icone sozinhas).")]
    public string gamepadText = "";

    [Tooltip("Nome mostrado acima do texto. Vazio = usa o nome padrao do tutorial.")]
    public string speaker = "";

    [Tooltip("Retrato desta fala. Vazio = usa o retrato padrao do tutorial.")]
    public Sprite portrait;

    [Header("Camera")]
    [Tooltip("O objeto que a camera enquadra nesta fala. Vazio = camera volta para a visao geral da fase.")]
    public Transform focusTarget;

    [Tooltip("Deslocamento do enquadramento, em unidades (ex: Y positivo sobe a camera).")]
    public Vector2 focusOffset = Vector2.zero;

    [Tooltip("Tamanho ortografico durante o foco. Menor = mais perto. 0 = so move, sem mudar o zoom.")]
    public float zoomSize = 5.5f;

    [Header("Eventos")]
    [Tooltip("Roda quando esta fala comeca (ex: acender uma tocha, tocar um som).")]
    public UnityEvent onStepStart;
}
