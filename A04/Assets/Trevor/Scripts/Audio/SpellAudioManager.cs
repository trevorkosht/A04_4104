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
        sfxSource.spatialBlend = 0f;
    }

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

        // 2. Subscribe to Player System
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnSpellCast += PlaySpellCastSound;
            PlayerSpellSystem.Instance.OnManaCheckFailed += PlayManaFail;
            PlayerSpellSystem.Instance.OnCooldownCheckFailed += PlayCooldownFail;
        }

        // 3. Subscribe to Static Events
        SpellController.OnSpellImpact += PlayImpactSoundAtLocation;
    }

    private void OnDestroy()
    {
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

    private void PlayBookSummon()
    {
        if (bookSummonClip != null)
        {
            sfxSource.clip = bookSummonClip;
            sfxSource.pitch = 1.0f;
            sfxSource.Play();
        }
    }

    private void PlayGridHighlight()
    {
        if (sfxSource.isPlaying && sfxSource.clip == bookSummonClip)
        {
            sfxSource.Stop();
        }

        PlayClip(gridHighlightClip, "GridHighlight", true);
    }

    private void PlayBookSummonFail() => PlayClip(bookSummonFailClip, "BookFail");
    private void PlayLoadSuccess() => PlayClip(spellLoadSuccessClip, "LoadSuccess");
    private void PlayLoadFail() => PlayClip(spellLoadFailClip, "LoadFail");
    private void PlayManaFail() => PlayClip(manaFailClip, "ManaFail");

    // CHANGED: We now pass '2.0f' as the volume scale (Double Volume)
    private void PlayCooldownFail() => PlayClip(cooldownFailClip, "CooldownFail", false, 2.0f);

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

    // --- Helper ---
    // CHANGED: Added 'volumeScale' parameter with a default of 1.0f
    private void PlayClip(AudioClip clip, string debugName, bool usePitchVariance = false, float volumeScale = 1.0f)
    {
        if (clip == null) return;

        if (randomizePitch && usePitchVariance)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
        }
        else
        {
            sfxSource.pitch = 1.0f;
        }

        // PlayOneShot allows a volume multiplier as the second argument
        sfxSource.PlayOneShot(clip, volumeScale);
    }
}