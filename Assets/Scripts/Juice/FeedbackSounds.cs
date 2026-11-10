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
        AttemptsExhausted,
        BlockSnap,
        TorchLit,
        BarrierOpen,
        Victory,
        UiHover,
        UiClick,
        UiBack,
        PauseOpen,
        PauseClose,
        DialogueType,
        DialogueOpen,
        DialogueClose,
        DialogueAdvance,
        CameraFocus,
        TutorialSkip,
        BlockPickup,
        BlockDrop,
        BlockRotate,
        OperatorCollect,
        WheelOpen,
        WheelHover,
        WheelError,
        TorchOut,
        TorchRelit,
        LeverPull,
        PlateDown,
        GateOpen
    }

    [Range(0f, 1f)] public float volume = 0.8f;

    [Header("Puzzle")]
    public AudioClip resultCorrect;
    public AudioClip resultWrong;
    public AudioClip attemptsExhausted;
    public AudioClip blockSnap;
    public AudioClip torchLit;
    public AudioClip barrierOpen;

    [Header("Fase")]
    public AudioClip victory;

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
    public AudioClip dialogueClose;
    [Tooltip("Apertou para avancar a fala. Vazio = usa o som de clique do menu.")]
    public AudioClip dialogueAdvance;
    [Tooltip("A camera se move para outro objeto durante o tour.")]
    public AudioClip cameraFocus;
    [Tooltip("Pulou o tutorial.")]
    public AudioClip tutorialSkip;

    [Header("Puzzle (acoes)")]
    public AudioClip blockPickup;
    public AudioClip blockDrop;
    public AudioClip blockRotate;
    public AudioClip operatorCollect;
    public AudioClip wheelOpen;
    public AudioClip wheelHover;
    public AudioClip wheelError;
    public AudioClip torchOut;
    public AudioClip torchRelit;
    public AudioClip leverPull;
    public AudioClip plateDown;
    public AudioClip gateOpen;

    // Ajuste fino de cada som (1 = normal; menos = mais baixo). Multiplica o volume geral.
    // Serve para igualar sons que vieram com volumes diferentes, sem editar o arquivo.
    [Header("Volume de cada som (1 = normal)")]
    [Range(0f, 1f)] public float resultCorrectVolume = 1f;
    [Range(0f, 1f)] public float resultWrongVolume = 1f;
    [Range(0f, 1f)] public float attemptsExhaustedVolume = 1f;
    [Range(0f, 1f)] public float blockSnapVolume = 1f;
    [Range(0f, 1f)] public float torchLitVolume = 1f;
    [Range(0f, 1f)] public float barrierOpenVolume = 1f;
    [Range(0f, 1f)] public float victoryVolume = 1f;
    [Range(0f, 1f)] public float uiHoverVolume = 1f;
    [Range(0f, 1f)] public float uiClickVolume = 1f;
    [Range(0f, 1f)] public float uiBackVolume = 1f;
    [Range(0f, 1f)] public float pauseOpenVolume = 1f;
    [Range(0f, 1f)] public float pauseCloseVolume = 1f;
    [Range(0f, 1f)] public float dialogueTypeVolume = 0.5f;
    [Range(0f, 1f)] public float dialogueOpenVolume = 1f;
    [Range(0f, 1f)] public float dialogueCloseVolume = 1f;
    [Range(0f, 1f)] public float dialogueAdvanceVolume = 1f;
    [Range(0f, 1f)] public float cameraFocusVolume = 1f;
    [Range(0f, 1f)] public float tutorialSkipVolume = 1f;
    [Range(0f, 1f)] public float blockPickupVolume = 1f;
    [Range(0f, 1f)] public float blockDropVolume = 1f;
    [Range(0f, 1f)] public float blockRotateVolume = 1f;
    [Range(0f, 1f)] public float operatorCollectVolume = 1f;
    [Range(0f, 1f)] public float wheelOpenVolume = 1f;
    [Range(0f, 1f)] public float wheelHoverVolume = 1f;
    [Range(0f, 1f)] public float wheelErrorVolume = 1f;
    [Range(0f, 1f)] public float torchOutVolume = 1f;
    [Range(0f, 1f)] public float torchRelitVolume = 1f;
    [Range(0f, 1f)] public float leverPullVolume = 1f;
    [Range(0f, 1f)] public float plateDownVolume = 1f;
    [Range(0f, 1f)] public float gateOpenVolume = 1f;

    public float GetVolume(Cue cue)
    {
        switch (cue)
        {
            case Cue.ResultCorrect: return volume * resultCorrectVolume;
            case Cue.ResultWrong: return volume * resultWrongVolume;
            case Cue.AttemptsExhausted: return volume * attemptsExhaustedVolume;
            case Cue.BlockSnap: return volume * blockSnapVolume;
            case Cue.TorchLit: return volume * torchLitVolume;
            case Cue.BarrierOpen: return volume * barrierOpenVolume;
            case Cue.Victory: return volume * victoryVolume;
            case Cue.UiHover: return volume * uiHoverVolume;
            case Cue.UiClick: return volume * uiClickVolume;
            case Cue.UiBack: return volume * uiBackVolume;
            case Cue.PauseOpen: return volume * pauseOpenVolume;
            case Cue.PauseClose: return volume * pauseCloseVolume;
            case Cue.DialogueType: return volume * dialogueTypeVolume;
            case Cue.DialogueOpen: return volume * dialogueOpenVolume;
            case Cue.DialogueClose: return volume * dialogueCloseVolume;
            case Cue.DialogueAdvance: return volume * dialogueAdvanceVolume;
            case Cue.CameraFocus: return volume * cameraFocusVolume;
            case Cue.TutorialSkip: return volume * tutorialSkipVolume;
            case Cue.BlockPickup: return volume * blockPickupVolume;
            case Cue.BlockDrop: return volume * blockDropVolume;
            case Cue.BlockRotate: return volume * blockRotateVolume;
            case Cue.OperatorCollect: return volume * operatorCollectVolume;
            case Cue.WheelOpen: return volume * wheelOpenVolume;
            case Cue.WheelHover: return volume * wheelHoverVolume;
            case Cue.WheelError: return volume * wheelErrorVolume;
            case Cue.TorchOut: return volume * torchOutVolume;
            case Cue.TorchRelit: return volume * torchRelitVolume;
            case Cue.LeverPull: return volume * leverPullVolume;
            case Cue.PlateDown: return volume * plateDownVolume;
            case Cue.GateOpen: return volume * gateOpenVolume;
            default: return volume;
        }
    }
    public AudioClip Get(Cue cue)
    {
        switch (cue)
        {
            case Cue.ResultCorrect: return resultCorrect;
            case Cue.ResultWrong: return resultWrong;
            case Cue.AttemptsExhausted: return attemptsExhausted;
            case Cue.BlockSnap: return blockSnap;
            case Cue.TorchLit: return torchLit;
            case Cue.BarrierOpen: return barrierOpen;
            case Cue.Victory: return victory;
            case Cue.UiHover: return uiHover;
            case Cue.UiClick: return uiClick;
            case Cue.UiBack: return uiBack;
            case Cue.PauseOpen: return pauseOpen;
            case Cue.PauseClose: return pauseClose;
            case Cue.DialogueType: return dialogueType;
            case Cue.DialogueOpen: return dialogueOpen;
            case Cue.DialogueClose: return dialogueClose;
            case Cue.DialogueAdvance: return dialogueAdvance != null ? dialogueAdvance : uiClick;
            case Cue.CameraFocus: return cameraFocus;
            case Cue.TutorialSkip: return tutorialSkip;
            case Cue.BlockPickup: return blockPickup;
            case Cue.BlockDrop: return blockDrop;
            case Cue.BlockRotate: return blockRotate;
            case Cue.OperatorCollect: return operatorCollect;
            case Cue.WheelOpen: return wheelOpen;
            case Cue.WheelHover: return wheelHover;
            case Cue.WheelError: return wheelError;
            case Cue.TorchOut: return torchOut;
            case Cue.TorchRelit: return torchRelit;
            case Cue.LeverPull: return leverPull;
            case Cue.PlateDown: return plateDown;
            case Cue.GateOpen: return gateOpen;
            default: return null;
        }
    }
}
