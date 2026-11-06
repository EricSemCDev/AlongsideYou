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
        Victory
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
            default: return null;
        }
    }
}
