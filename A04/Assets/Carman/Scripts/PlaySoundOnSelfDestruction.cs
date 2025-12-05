using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Destructible))]
public class PlaySoundOnSelfDestruction : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] voiceLines;

    [Header("Optional: Object to Watch (Skip Voice Lines)")]
    public Destructible objectToWatch; // e.g., final tutorial wall

    private Destructible destructible;
    private Coroutine voiceCoroutine;

    void Awake()
    {
        destructible = GetComponent<Destructible>();

        // if (destructible != null)
        // {
        //     destructible.onDestroyed.AddListener(OnWallDestroyed);
        // }
        // else
        // {
        //     Debug.LogWarning("No Destructible component found on this object!");
        // }

        // if (objectToWatch != null)
        // {
        //     objectToWatch.onDestroyed.AddListener(SkipVoiceLines);
        // }
    }

    private void OnWallDestroyed()
    {
        // Only start if there are voice lines
        if (voiceLines != null && voiceLines.Length > 0)
        {
            // Start coroutine on a persistent object (SoundManager)
            if (SoundManager.Instance != null)
            {
                voiceCoroutine = StartCoroutine(PlayVoiceLinesSequence());
            }
            else
            {
                Debug.LogWarning("SoundManager instance not found!");
            }
        }
    }

    private IEnumerator PlayVoiceLinesSequence()
    {
        // Pause music while voice lines play
        SoundManager.Instance.PauseMusic();

        foreach (AudioClip clip in voiceLines)
        {
            if (clip != null)
            {
                SoundManager.Instance.PlayVoiceLine(clip);
                yield return new WaitForSeconds(clip.length);
            }
        }

        // Resume music when done
        SoundManager.Instance.ResumeMusic();
    }

    /// <summary>
    /// Stops the voice lines immediately and resumes music.
    /// </summary>
    public void SkipVoiceLines()
    {
        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;

            SoundManager.Instance.ResumeMusic();
        }
    }

    // void OnDestroy()
    // {
    //     if (destructible != null)
    //     {
    //         destructible.onDestroyed.RemoveListener(OnWallDestroyed);
    //     }

    //     if (objectToWatch != null)
    //     {
    //         objectToWatch.onDestroyed.RemoveListener(SkipVoiceLines);
    //     }
    // }
}
