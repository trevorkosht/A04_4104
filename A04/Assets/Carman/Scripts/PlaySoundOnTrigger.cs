using UnityEngine;
using System.Collections;

public class PlaySoundOnTrigger : MonoBehaviour
{
    [Header("Voice Lines")]
    public AudioClip[] sounds;

    [Header("Skip Condition")]
    public string destructibleTag = "Destructible"; // Tag to search for

    private bool hasPlayed = false;
    private Coroutine voiceCoroutine;
    private Destructible objectToWatch;

    void Start()
    {
        // Search for the object with the destructible tag at runtime
        GameObject go = GameObject.FindGameObjectWithTag(destructibleTag);
        if (go != null)
        {
            objectToWatch = go.GetComponent<Destructible>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
        hasPlayed = true;

        if (sounds != null && sounds.Length > 0)
        {
            SoundManager.Instance.PauseMusic();
            voiceCoroutine = StartCoroutine(PlayClipsInSequence(sounds));

            // // Subscribe to destruction event
            // if (objectToWatch != null)
            // {
            //     objectToWatch.onDestroyed.AddListener(SkipVoiceLines);
            // }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayClipsInSequence(AudioClip[] clips)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                SoundManager.Instance.PlayVoiceLine(clip);
                yield return new WaitForSeconds(clip.length);
            }
        }

        SoundManager.Instance.ResumeMusic();
        Destroy(gameObject);
    }

    public void SkipVoiceLines()
    {
        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;
        }

        SoundManager.Instance.ResumeMusic();
        Destroy(gameObject);
    }

    // void OnDestroy()
    // {
    //     if (objectToWatch != null)
    //     {
    //         objectToWatch.onDestroyed.RemoveListener(SkipVoiceLines);
    //     }
    // }
}
