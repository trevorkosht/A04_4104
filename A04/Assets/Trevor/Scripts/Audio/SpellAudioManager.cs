using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpellAudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpellGridManager gridManager;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private bool randomizePitch = true;

    [Header("UI / Interaction Sounds")]
    [SerializeField] private AudioClip bookSummonClip;
    [SerializeField] private AudioClip bookSummonFailClip;
    [SerializeField] private AudioClip gridHighlightClip;
    [SerializeField] private AudioClip spellLoadSuccessClip;
    [SerializeField] private AudioClip spellLoadFailClip;

    [Header("Fail Sounds")]
    [SerializeField] private AudioClip manaFailClip;
    [SerializeField] private AudioClip cooldownFailClip;

    private void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
        // Ensure this is 2D for UI sounds (optional code force)
        sfxSource.spatialBlend = 0f;
    }

    // CHANGED: Moved from OnEnable to Start to ensure Singletons exist
    private void Start()
    {
        // 1. Subscribe to Grid Manager
        if (gridManager != null)
        {
            gridManager.OnGridOpened += PlayBookSummon;
            gridManager.OnGridOpenFailed += PlayBookSummonFail;
            gridManager.OnCellHighlighted += PlayGridHighlight;
            gridManager.OnSpellLoadSuccess += PlayLoadSuccess;
            gridManager.OnSpellLoadFailed += PlayLoadFail;
        }
        else
        {
            Debug.LogError("SpellAudioManager: GridManager reference is MISSING in Inspector!");
        }

        // 2. Subscribe to Player System (Singleton)
        if (PlayerSpellSystem.Instance != null)
        {
            Debug.Log("SpellAudioManager: Successfully connected to PlayerSpellSystem.");
            PlayerSpellSystem.Instance.OnSpellCast += PlaySpellCastSound;
            PlayerSpellSystem.Instance.OnManaCheckFailed += PlayManaFail;
            PlayerSpellSystem.Instance.OnCooldownCheckFailed += PlayCooldownFail;
        }
        else
        {
            Debug.LogError("SpellAudioManager: PlayerSpellSystem.Instance is NULL! Check Script Execution Order.");
        }

        // 3. Subscribe to Static Events
        SpellController.OnSpellImpact += PlayImpactSoundAtLocation;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (gridManager != null)
        {
            gridManager.OnGridOpened -= PlayBookSummon;
            gridManager.OnGridOpenFailed -= PlayBookSummonFail;
            gridManager.OnCellHighlighted -= PlayGridHighlight;
            gridManager.OnSpellLoadSuccess -= PlayLoadSuccess;
            gridManager.OnSpellLoadFailed -= PlayLoadFail;
        }

        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnSpellCast -= PlaySpellCastSound;
            PlayerSpellSystem.Instance.OnManaCheckFailed -= PlayManaFail;
            PlayerSpellSystem.Instance.OnCooldownCheckFailed -= PlayCooldownFail;
        }

        SpellController.OnSpellImpact -= PlayImpactSoundAtLocation;
    }

    // --- Audio Handlers ---
    private void PlayBookSummon() => PlayClip(bookSummonClip, "BookSummon");
    private void PlayBookSummonFail() => PlayClip(bookSummonFailClip, "BookFail");
    private void PlayGridHighlight() => PlayClip(gridHighlightClip, "GridHighlight", true);
    private void PlayLoadSuccess() => PlayClip(spellLoadSuccessClip, "LoadSuccess");
    private void PlayLoadFail() => PlayClip(spellLoadFailClip, "LoadFail");

    // FAIL HANDLERS
    private void PlayManaFail() => PlayClip(manaFailClip, "ManaFail");
    private void PlayCooldownFail() => PlayClip(cooldownFailClip, "CooldownFail");

    private void PlaySpellCastSound(GridSpellSO spell)
    {
        if (spell == null) return;
        PlayClip(spell.castSound, $"Cast ({spell.name})");
    }

    private void PlayImpactSoundAtLocation(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position);
    }

    // --- Debugging PlayClip ---
    private void PlayClip(AudioClip clip, string debugName, bool usePitchVariance = false)
    {
        // DEBUG: Check if clip is missing
        if (clip == null)
        {
            Debug.LogWarning($"SpellAudioManager: Requested to play '{debugName}' but the AudioClip is NULL in the Inspector!");
            return;
        }

        // DEBUG: Check if volume is zero
        if (sfxSource.volume <= 0)
        {
            Debug.LogWarning("SpellAudioManager: AudioSource Volume is 0!");
        }

        if (randomizePitch && usePitchVariance)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
        }
        else
        {
            sfxSource.pitch = 1.0f;
        }

        sfxSource.PlayOneShot(clip);
        // Debug.Log($"Played Sound: {debugName}"); // Uncomment if you want to see every sound in console
    }
}