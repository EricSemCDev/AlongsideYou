using UnityEngine;

// Objeto escondido que roda as corrotinas e toca os sons (persiste entre cenas).
public class JuiceRunner : MonoBehaviour
{
    public AudioSource Audio { get; private set; }

    private void Awake()
    {
        Audio = gameObject.AddComponent<AudioSource>();
        Audio.playOnAwake = false;
        Audio.spatialBlend = 0f;
    }
}
