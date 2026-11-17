using System.Collections;
using UnityEngine;

// Musica de fundo unica: toca em loop com um fade de entrada e sobrevive as trocas de
// cena, entao nao reinicia nem corta ao sair do menu. Coloque em um objeto vazio no
// MainMenu (copias repetidas se destroem sozinhas).
public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;

    [SerializeField] private AudioClip music;
    [Range(0f, 1f)] [SerializeField] private float volume = 0.5f;
    [SerializeField] private float fadeInSeconds = 2f;

    private IEnumerator Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            yield break;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = music;
        source.loop = true;
        source.volume = 0f;
        source.Play();

        for (float t = 0f; t < fadeInSeconds; t += Time.unscaledDeltaTime)
        {
            source.volume = volume * t / fadeInSeconds;
            yield return null;
        }

        source.volume = volume;
    }
}
