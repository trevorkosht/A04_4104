using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Music")]
    public AudioSource musicSource;

    [Header("SFX")]
    public AudioSource sfxSource;


    public float volume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayMusic();
    }

    // -------------------- MUSIC --------------------

    public void PlayMusic()
    {
        musicSource.volume = 0.5f;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlayVoiceLine(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }


}
