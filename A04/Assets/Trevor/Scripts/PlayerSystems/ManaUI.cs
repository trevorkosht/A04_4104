using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Required for Coroutines

public class ManaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private Image fillImage; // The "Fill" image inside the slider

    [Header("Flicker Settings")]
    [SerializeField] private float flickerDuration = 2.0f; // How long it lasts
    [SerializeField] private float flickerSpeed = 10.0f;   // How fast it blinks
    [SerializeField] private float minAlpha = 0.2f;        // How transparent it gets (0 is invisible)

    private Coroutine flickerCoroutine;
    private Color originalColor;

    void Start()
    {
        // Try to auto-find the fill image if not assigned
        if (fillImage == null && manaSlider != null)
        {
            fillImage = manaSlider.fillRect.GetComponent<Image>();
        }

        if (fillImage != null)
        {
            originalColor = fillImage.color;
        }

        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnManaChanged += UpdateManaDisplay;

            // SUBSCRIBE to the failure event
            PlayerSpellSystem.Instance.OnManaCheckFailed += TriggerManaFlicker;

            UpdateManaDisplay(PlayerSpellSystem.Instance.currentMana, PlayerSpellSystem.Instance.maxMana);
        }
        else
        {
            Debug.LogWarning("ManaUI: PlayerSpellSystem not found!");
        }
    }

    void OnDestroy()
    {
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnManaChanged -= UpdateManaDisplay;
            PlayerSpellSystem.Instance.OnManaCheckFailed -= TriggerManaFlicker;
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

    private void TriggerManaFlicker()
    {
        if (fillImage == null) return;

        // If we are already flickering, stop and restart so the timer resets
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);

        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            timer += Time.deltaTime;

            // Calculate a smooth wave using Sine
            // (Sin goes from -1 to 1. We map it to 0 to 1).
            float wave = (Mathf.Sin(timer * flickerSpeed) + 1f) / 2f;

            // Interpolate between Min Alpha and Full Alpha
            float newAlpha = Mathf.Lerp(minAlpha, 1f, wave);

            // Apply the alpha
            Color c = originalColor;
            c.a = newAlpha;
            fillImage.color = c;

            yield return null; // Wait for next frame
        }

        // Reset to normal at the end
        fillImage.color = originalColor;
        flickerCoroutine = null;
    }
}