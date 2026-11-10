using UnityEngine;

// Objeto escondido que roda as corrotinas e toca os sons (persiste entre cenas).
public class JuiceRunner : MonoBehaviour
{
    public AudioSource Audio { get; private set; }

    // Fonte so para sons com tom variado (a digitacao do dialogo): mudar o tom aqui nao afeta os outros sons.
    public AudioSource Voice { get; private set; }

    private void Awake()
    {
        Audio = gameObject.AddComponent<AudioSource>();
        Audio.playOnAwake = false;
        Audio.spatialBlend = 0f;

        Voice = gameObject.AddComponent<AudioSource>();
        Voice.playOnAwake = false;
        Voice.spatialBlend = 0f;
    }
}
