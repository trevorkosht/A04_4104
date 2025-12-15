using UnityEngine;
using System.Collections;

public class PlaySoundOnInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player needs to be to read/dismiss (in meters)")]
    public float interactRange = 4.0f;
    public KeyCode interactKey = KeyCode.G;
    private Transform playerTransform;

    [Header("Voice Lines")]
    public AudioClip[] sounds;

    private bool hasPlayed = false;
    private Coroutine voiceCoroutine;

    void Start()
    {
        // 1. Find player by Tag (Robust way)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("TutorialInfoInteraction: Could not find object tagged 'Player'!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 3. THE MAGIC: Pure Math Check (No Colliders)
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactRange)
        {
            // --- PLAYER IS CLOSE ---

            // B. Listen for Input
            if (Input.GetKeyDown(interactKey))
            {
                if (hasPlayed) return;

                if (sounds == null || sounds.Length == 0) return;

                hasPlayed = true;

                SoundManager.Instance.PauseMusic();
                voiceCoroutine = StartCoroutine(PlayClipsInSequence(sounds));
            }
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
}
