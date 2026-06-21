using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance;

    public AudioSource sfxSource;

    private void Awake()
    {
        Instance = this;
    }

    public void Play(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}