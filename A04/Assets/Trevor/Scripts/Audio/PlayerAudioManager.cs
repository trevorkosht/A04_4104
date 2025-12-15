using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private WandMeleeController meleeController;
    [SerializeField] private PlayerHealth playerHealth;
    // We need THIS reference back to listen for the "Duplicate" event
    [SerializeField] private PlayerStickerInteraction stickerInteraction;

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
    [Tooltip("Played when a NEW sticker is unlocked (Fanfare)")]
    [SerializeField] private AudioClip stickerUnlockClip;

    [Tooltip("Played when picking up a DUPLICATE sticker (Simple Pop)")]
    [SerializeField] private AudioClip stickerPickupClip; // NEW SLOT

    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip unpauseClip;
    [SerializeField] private AudioClip manaRestoreClip;
    [SerializeField] private AudioClip healClip;

    private void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
        if (footstepSource == null) footstepSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f;
    }

    private void Start()
    {
        // 1. Player Controller
        if (playerController != null)
        {
            playerController.OnFootstep += PlayFootstep;
            playerController.OnMovementStop += StopFootsteps;
            playerController.OnJump += PlayJump;
            playerController.OnLand += PlayLand;
        }

        // 2. Melee
        if (meleeController != null) meleeController.OnMeleeSwing += PlayMeleeSwing;

        // 3. Health
        if (playerHealth != null)
        {
            playerHealth.OnHurt += PlayHurt;
            playerHealth.OnDie += PlayDie;
            playerHealth.OnHeal += PlayHeal;
        }

        // 4. COLLECTION MANAGER (Handles NEW Unlocks)
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.OnStickerAdded += PlayStickerUnlock;
        }

        // 5. STICKER INTERACTION (Handles DUPLICATE Pickups)
        if (stickerInteraction != null)
        {
            stickerInteraction.OnStickerReCollected += PlayStickerPickup;
        }

        // 6. Game Manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleGameStateChange;
        }

        // 7. Mana
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnManaRestored += PlayManaRestore;
        }
    }

    private void OnDestroy()
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

        if (CollectionManager.Instance != null) CollectionManager.Instance.OnStickerAdded -= PlayStickerUnlock;

        if (stickerInteraction != null) stickerInteraction.OnStickerReCollected -= PlayStickerPickup;

        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= HandleGameStateChange;

        if (PlayerSpellSystem.Instance != null) PlayerSpellSystem.Instance.OnManaRestored -= PlayManaRestore;
    }

    // --- Audio Handlers ---

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

    // NEW: Play the "Fanfare" for a new sticker
    private void PlayStickerUnlock() => PlayOneShot(stickerUnlockClip);

    // NEW: Play the "Pop" for a duplicate sticker
    private void PlayStickerPickup() => PlayOneShot(stickerPickupClip);

    private void PlayManaRestore() => PlayOneShot(manaRestoreClip);
    private void PlayHeal() => PlayOneShot(healClip);

    private void HandleGameStateChange(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Pause) PlayOneShot(pauseClip);
        else if (newState == GameManager.GameState.Play) PlayOneShot(unpauseClip);
    }

    private void PlayOneShot(AudioClip clip, bool randomizePitch = false)
    {
        if (clip == null) return;
        sfxSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
        sfxSource.PlayOneShot(clip);
    }
}