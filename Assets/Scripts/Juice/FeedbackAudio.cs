using UnityEngine;

// Toca um som de FeedbackSounds. Sem o asset (ou sem clipe), fica em silencio.
public static class FeedbackAudio
{
    private static FeedbackSounds _sounds;
    private static bool _loaded;

    // pitch diferente de 1 toca na fonte de tom variado (ex: a digitacao do dialogo).
    public static void Play(FeedbackSounds.Cue cue, float pitch = 1f, float volumeScale = 1f)
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
            AudioSource source = Mathf.Approximately(pitch, 1f) ? GameJuice.Audio : GameJuice.Voice;
            source.pitch = pitch;
            source.PlayOneShot(clip, _sounds.GetVolume(cue) * volumeScale);
        }
    }
}
