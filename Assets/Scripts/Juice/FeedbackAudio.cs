using UnityEngine;

// Toca um som de FeedbackSounds. Sem o asset (ou sem clipe), fica em silencio.
public static class FeedbackAudio
{
    private static FeedbackSounds _sounds;
    private static bool _loaded;

    public static void Play(FeedbackSounds.Cue cue)
    {
        if (!_loaded)
        {
            _sounds = Resources.Load<FeedbackSounds>("FeedbackSounds");
            _loaded = true;
        }

        if (_sounds == null)
        {
            return;
        }

        AudioClip clip = _sounds.Get(cue);

        if (clip != null)
        {
            GameJuice.Audio.PlayOneShot(clip, _sounds.volume);
        }
    }
}
