using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private Image fillImage;

    [Header("Failure Flicker Settings")]
    [SerializeField] private float flickerDuration = 2.0f;
    [SerializeField] private float flickerSpeed = 10.0f;
    [SerializeField] private float minAlpha = 0.2f;

    [Header("Cast Flash Settings")]
    [SerializeField] private Color castFlashColor = new Color(1f, 1f, 1f, 1f); // White flash
    [SerializeField] private float castFlashDuration = 0.2f; // Very fast

    private Coroutine activeCoroutine; // Track the currently running effect
    private Color originalColor;

    void Start()
    {
        if (fillImage == null && manaSlider != null)
            fillImage = manaSlider.fillRect.GetComponent<Image>();

        if (fillImage != null)
            originalColor = fillImage.color;

        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnManaChanged += UpdateManaDisplay;

            // 1. Listen for FAILURE (Low Mana)
            PlayerSpellSystem.Instance.OnManaCheckFailed += TriggerFailureFlicker;

            // 2. Listen for SUCCESS (Spell Cast)
            PlayerSpellSystem.Instance.OnCooldownStarted += OnSpellCast;

            UpdateManaDisplay(PlayerSpellSystem.Instance.currentMana, PlayerSpellSystem.Instance.maxMana);
        }
    }

    void OnDestroy()
    {
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnManaChanged -= UpdateManaDisplay;
            PlayerSpellSystem.Instance.OnManaCheckFailed -= TriggerFailureFlicker;
            PlayerSpellSystem.Instance.OnCooldownStarted -= OnSpellCast; // Unsubscribe!
        }
    }

    private void UpdateManaDisplay(float current, float max)
    {
        if (manaSlider != null)
        {
            manaSlider.maxValue = max;
            manaSlider.value = current;
        }
    }

    // --- Event Handlers ---

    private void OnSpellCast(GridSpellSO spell)
    {
        // When a spell is cast, trigger the "Success" flash
        TriggerCastFlash();
    }

    private void TriggerCastFlash()
    {
        if (fillImage == null) return;

        // Stop any existing flicker/flash so they don't fight
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(CastFlashRoutine());
    }

    private void TriggerFailureFlicker()
    {
        if (fillImage == null) return;

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(FailureFlickerRoutine());
    }

    // --- Coroutines ---

    private IEnumerator CastFlashRoutine()
    {
        float timer = 0f;

        // Flash to the target color (White) immediately
        fillImage.color = castFlashColor;

        // Quickly fade back to original color
        while (timer < castFlashDuration)
        {
            timer += Time.deltaTime;
            float t = timer / castFlashDuration;

            // Lerp back to original
            fillImage.color = Color.Lerp(castFlashColor, originalColor, t);

            yield return null;
        }

        fillImage.color = originalColor;
        activeCoroutine = null;
    }

    private IEnumerator FailureFlickerRoutine()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            timer += Time.deltaTime;
            float wave = (Mathf.Sin(timer * flickerSpeed) + 1f) / 2f;
            float newAlpha = Mathf.Lerp(minAlpha, 1f, wave);

            // Modify Alpha only, keep original RGB
            Color c = originalColor;
            c.a = newAlpha;
            fillImage.color = c;

            yield return null;
        }

        fillImage.color = originalColor;
        activeCoroutine = null;
    }
}