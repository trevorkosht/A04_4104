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
    public VoiceLine[] voiceLines;

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

                if (voiceLines == null || voiceLines.Length == 0) return;

                hasPlayed = true;

                SoundManager.Instance.PauseMusic();
                voiceCoroutine = StartCoroutine(PlayClipsInSequence(voiceLines));
            }
        }
    }

    private IEnumerator PlayClipsInSequence(VoiceLine[] lines)
    {
        foreach (VoiceLine line in lines)
        {
            if (line.clip != null)
            {
                SoundManager.Instance.PlayVoiceLine(line.clip);

                SubtitleManager.Instance.ShowSubtitle(
                    line.subtitle,
                    line.clip.length
                );

                yield return new WaitForSeconds(line.clip.length);
            }
        }

        SubtitleManager.Instance.ClearSubtitle();
        SoundManager.Instance.ResumeMusic();
        Destroy(gameObject);
    }
}
