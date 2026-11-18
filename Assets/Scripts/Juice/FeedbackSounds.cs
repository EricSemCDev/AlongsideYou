using UnityEngine;

// Os sons de feedback do jogo (acertar, errar, encaixar o bloco...). Sem nenhum
// codigo: crie o asset pelo menu Alongside You > Juice > Criar FeedbackSounds,
// arraste os clipes e pronto. Um clipe vazio simplesmente fica em silencio.
// O asset fica em Assets/Resources/FeedbackSounds.asset (o jogo o carrega por la).
[CreateAssetMenu(fileName = "FeedbackSounds", menuName = "Alongside You/Feedback Sounds")]
public class FeedbackSounds : ScriptableObject
{
    public enum Cue
    {
        ResultCorrect,
        ResultWrong,
        BlockSnap,
        TorchLit,
        Victory,
        UiHover,
        UiClick,
        UiBack,
        PauseOpen,
        PauseClose,
        DialogueType,
        DialogueOpen,
        DialogueAdvance,
        CameraFocus,
        BlockPickup,
        BlockDrop,
        BlockRotate,
        OperatorCollect,
        WheelOpen,
        WheelHover,
        WheelError,
        StarEarned
    }

    [Range(0f, 1f)] public float volume = 0.8f;

    [Header("Puzzle")]
    public AudioClip resultCorrect;
    public AudioClip resultWrong;
    public AudioClip blockSnap;
    public AudioClip torchLit;

    [Header("Fase")]
    public AudioClip victory;
    [Tooltip("Cada estrela que enche na tela de resultado (toca de 1 a 3 vezes).")]
    public AudioClip starEarned;

    [Header("Menus")]
    public AudioClip uiHover;
    public AudioClip uiClick;
    public AudioClip uiBack;
    public AudioClip pauseOpen;
    public AudioClip pauseClose;

    [Header("Dialogo e tutorial")]
    [Tooltip("Tique de cada letra (toca a cada 2 letras, com o tom variando um pouco). Bem curto.")]
    public AudioClip dialogueType;
    public AudioClip dialogueOpen;
    [Tooltip("Apertou para avancar a fala. Vazio = usa o som de clique do menu.")]
    public AudioClip dialogueAdvance;
    [Tooltip("A camera se move para outro objeto durante o tour.")]
    public AudioClip cameraFocus;

    [Header("Puzzle (acoes)")]
    public AudioClip blockPickup;
    public AudioClip blockDrop;
    public AudioClip blockRotate;
    public AudioClip operatorCollect;
    public AudioClip wheelOpen;
    public AudioClip wheelHover;
    public AudioClip wheelError;

    // Ajuste fino de cada som (1 = normal; menos = mais baixo). Multiplica o volume geral.
    // Serve para igualar sons que vieram com volumes diferentes, sem editar o arquivo.
    [Header("Volume de cada som (1 = normal)")]
    [Range(0f, 1f)] public float resultCorrectVolume = 1f;
    [Range(0f, 1f)] public float resultWrongVolume = 1f;
    [Range(0f, 1f)] public float blockSnapVolume = 1f;
    [Range(0f, 1f)] public float torchLitVolume = 1f;
    [Range(0f, 1f)] public float victoryVolume = 1f;
    [Range(0f, 1f)] public float starEarnedVolume = 1f;
    [Range(0f, 1f)] public float uiHoverVolume = 1f;
    [Range(0f, 1f)] public float uiClickVolume = 1f;
    [Range(0f, 1f)] public float uiBackVolume = 1f;
    [Range(0f, 1f)] public float pauseOpenVolume = 1f;
    [Range(0f, 1f)] public float pauseCloseVolume = 1f;
    [Range(0f, 1f)] public float dialogueTypeVolume = 0.5f;
    [Range(0f, 1f)] public float dialogueOpenVolume = 1f;
    [Range(0f, 1f)] public float dialogueAdvanceVolume = 1f;
    [Range(0f, 1f)] public float cameraFocusVolume = 1f;
    [Range(0f, 1f)] public float blockPickupVolume = 1f;
    [Range(0f, 1f)] public float blockDropVolume = 1f;
    [Range(0f, 1f)] public float blockRotateVolume = 1f;
    [Range(0f, 1f)] public float operatorCollectVolume = 1f;
    [Range(0f, 1f)] public float wheelOpenVolume = 1f;
    [Range(0f, 1f)] public float wheelHoverVolume = 1f;
    [Range(0f, 1f)] public float wheelErrorVolume = 1f;

    public float GetVolume(Cue cue)
    {
        switch (cue)
        {
            case Cue.ResultCorrect: return volume * resultCorrectVolume;
            case Cue.ResultWrong: return volume * resultWrongVolume;
            case Cue.BlockSnap: return volume * blockSnapVolume;
            case Cue.TorchLit: return volume * torchLitVolume;
            case Cue.Victory: return volume * victoryVolume;
            case Cue.StarEarned: return volume * starEarnedVolume;
            case Cue.UiHover: return volume * uiHoverVolume;
            case Cue.UiClick: return volume * uiClickVolume;
            case Cue.UiBack: return volume * uiBackVolume;
            case Cue.PauseOpen: return volume * pauseOpenVolume;
            case Cue.PauseClose: return volume * pauseCloseVolume;
            case Cue.DialogueType: return volume * dialogueTypeVolume;
            case Cue.DialogueOpen: return volume * dialogueOpenVolume;
            case Cue.DialogueAdvance: return volume * dialogueAdvanceVolume;
            case Cue.CameraFocus: return volume * cameraFocusVolume;
            case Cue.BlockPickup: return volume * blockPickupVolume;
            case Cue.BlockDrop: return volume * blockDropVolume;
            case Cue.BlockRotate: return volume * blockRotateVolume;
            case Cue.OperatorCollect: return volume * operatorCollectVolume;
            case Cue.WheelOpen: return volume * wheelOpenVolume;
            case Cue.WheelHover: return volume * wheelHoverVolume;
            case Cue.WheelError: return volume * wheelErrorVolume;
            default: return volume;
        }
    }
    public AudioClip Get(Cue cue)
    {
        switch (cue)
        {
            case Cue.ResultCorrect: return resultCorrect;
            case Cue.ResultWrong: return resultWrong;
            case Cue.BlockSnap: return blockSnap;
            case Cue.TorchLit: return torchLit;
            case Cue.Victory: return victory;
            case Cue.StarEarned: return starEarned;
            case Cue.UiHover: return uiHover;
            case Cue.UiClick: return uiClick;
            case Cue.UiBack: return uiBack;
            case Cue.PauseOpen: return pauseOpen;
            case Cue.PauseClose: return pauseClose;
            case Cue.DialogueType: return dialogueType;
            case Cue.DialogueOpen: return dialogueOpen;
            case Cue.DialogueAdvance: return dialogueAdvance != null ? dialogueAdvance : uiClick;
            case Cue.CameraFocus: return cameraFocus;
            case Cue.BlockPickup: return blockPickup;
            case Cue.BlockDrop: return blockDrop;
            case Cue.BlockRotate: return blockRotate;
            case Cue.OperatorCollect: return operatorCollect;
            case Cue.WheelOpen: return wheelOpen;
            case Cue.WheelHover: return wheelHover;
            case Cue.WheelError: return wheelError;
            default: return null;
        }
    }
}
