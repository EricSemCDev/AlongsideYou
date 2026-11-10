using UnityEngine;

// Toca um som de FeedbackSounds. Sem o asset (ou sem clipe), fica em silencio.
public static class FeedbackAudio
{
    private static FeedbackSounds _sounds;
    private static bool _loaded;

    // Som de um objeto do mundo (tocha, placa, portao): fica mais baixo quanto mais longe da camera
    // e some alem de maxDistance (unidades). O jogo e 2D, entao e so um ajuste de volume.
    public static void PlayAt(FeedbackSounds.Cue cue, Vector3 worldPosition, float maxDistance = 16f)
    {
        Camera camera = Camera.main;
        float distance = camera != null ? Vector2.Distance(camera.transform.position, worldPosition) : 0f;
        float closeness = Mathf.Clamp01(1f - (distance / maxDistance));

        if (closeness > 0.02f)
        {
            Play(cue, 1f, closeness);
        }
    }

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
