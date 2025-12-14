using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private WandMeleeController meleeController;
    [SerializeField] private PlayerStickerInteraction stickerInteraction;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource footstepSource;

    [Header("Movement Sounds")]
    [SerializeField] private AudioClip[] walkStepClips;
    [SerializeField] private AudioClip[] sprintStepClips;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;

    [Header("Combat Sounds")]
    [SerializeField] private AudioClip swing1Clip;
    [SerializeField] private AudioClip swing2Clip;
    [SerializeField] private AudioClip swing3Clip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip dieClip;

    [Header("Interaction Sounds")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip unpauseClip;
    [SerializeField] private AudioClip manaRestoreClip;
    [SerializeField] private AudioClip healClip;

    private void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
        if (footstepSource == null) footstepSource = gameObject.AddComponent<AudioSource>();

        // IMPORTANT: Ensure the SFX source can play even if we used AudioListener.pause (optional safety)
        sfxSource.ignoreListenerPause = true;
    }

    private void OnEnable()
    {
        // 1. Subscribe to Player Controller Events
        if (playerController != null)
        {
            playerController.OnFootstep += PlayFootstep;
            playerController.OnMovementStop += StopFootsteps;
            playerController.OnJump += PlayJump;
            playerController.OnLand += PlayLand;
        }

        // 2. Subscribe to Melee Events
        if (meleeController != null) meleeController.OnMeleeSwing += PlayMeleeSwing;

        // 3. Subscribe to Health Events
        if (playerHealth != null)
        {
            playerHealth.OnHurt += PlayHurt;
            playerHealth.OnDie += PlayDie;
            playerHealth.OnHeal += PlayHeal;
        }

        // 4. Subscribe to Sticker Events
        if (stickerInteraction != null) stickerInteraction.OnStickerCollected += PlayCollect;

        // 5. Subscribe to Game State Changes (Pause/Unpause)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleGameStateChange;
        }

        if (PlayerSpellSystem.Instance != null) PlayerSpellSystem.Instance.OnManaRestored += PlayManaRestore;
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnFootstep -= PlayFootstep;
            playerController.OnMovementStop -= StopFootsteps;
            playerController.OnJump -= PlayJump;
            playerController.OnLand -= PlayLand;
        }

        if (meleeController != null) meleeController.OnMeleeSwing -= PlayMeleeSwing;

        if (playerHealth != null)
        {
            playerHealth.OnHurt -= PlayHurt;
            playerHealth.OnDie -= PlayDie;
            playerHealth.OnHeal -= PlayHeal;
        }

        if (stickerInteraction != null) stickerInteraction.OnStickerCollected -= PlayCollect;

        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= HandleGameStateChange;

        if (PlayerSpellSystem.Instance != null) PlayerSpellSystem.Instance.OnManaRestored -= PlayManaRestore;
    }

    // --- Event Handlers ---

    private void HandleGameStateChange(GameManager.GameState newState)
    {
        // 
        // We catch the event here and play the sound immediately.
        if (newState == GameManager.GameState.Pause)
        {
            // CHANGED: Use PlayOneShot instead of PlayClipAtPoint
            PlayOneShot(pauseClip);
        }
        else if (newState == GameManager.GameState.Play)
        {
            PlayOneShot(unpauseClip);
        }
    }

    private void PlayFootstep(bool isSprinting)
    {
        AudioClip[] clipsToUse = isSprinting ? sprintStepClips : walkStepClips;
        if (clipsToUse.Length == 0) return;

        AudioClip clip = clipsToUse[Random.Range(0, clipsToUse.Length)];

        footstepSource.clip = clip;
        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        if (!footstepSource.isPlaying) footstepSource.Play();
    }

    private void StopFootsteps()
    {
        if (footstepSource.isPlaying) footstepSource.Stop();
    }

    private void PlayJump() => PlayOneShot(jumpClip);
    private void PlayLand() => PlayOneShot(landClip);

    private void PlayMeleeSwing(int comboIndex)
    {
        AudioClip clip = null;
        switch (comboIndex)
        {
            case 0: clip = swing1Clip; break;
            case 1: clip = swing2Clip; break;
            case 2: clip = swing3Clip; break;
            default: clip = swing1Clip; break;
        }
        PlayOneShot(clip, true);
    }

    private void PlayHurt() => PlayOneShot(hurtClip, true);
    private void PlayDie() => PlayOneShot(dieClip);
    private void PlayCollect() => PlayOneShot(collectClip);
    private void PlayManaRestore() => PlayOneShot(manaRestoreClip);
    private void PlayHeal() => PlayOneShot(healClip);

    // --- Helper ---
    private void PlayOneShot(AudioClip clip, bool randomizePitch = false)
    {
        if (clip == null) return;
        sfxSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
        sfxSource.PlayOneShot(clip);
    }
}