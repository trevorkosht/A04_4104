using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Required for Coroutines

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;

    [Header("Vignette Settings")]
    [SerializeField] private Image damageVignette; // The red panel
    [SerializeField] private float flashSpeed = 2f; // How fast it fades out
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Red, 50% opacity

    private int lastKnownHealth;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (playerHealth != null)
        {
            // Initialize stats without triggering the flash
            lastKnownHealth = playerHealth.currentHealth;
            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        // Ensure vignette is invisible at start
        if (damageVignette != null)
        {
            damageVignette.color = Color.clear;
            damageVignette.raycastTarget = false; // Important: lets clicks pass through!
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    // This method decides WHAT to do (Update slider? Flash screen?)
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        // 1. Update the Slider
        UpdateHealthUI(currentHealth, maxHealth);

        // 2. Check for Damage (If new health is LOWER than old health)
        if (currentHealth < lastKnownHealth)
        {
            TriggerDamageFlash();
        }

        // 3. Update memory
        lastKnownHealth = currentHealth;
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void TriggerDamageFlash()
    {
        if (damageVignette == null) return;

        // Stop any existing fade so we can restart the flash immediately
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeVignette());
    }

    private IEnumerator FadeVignette()
    {
        // 1. Set color instantly to the flash color
        damageVignette.color = flashColor;

        // 2. Fade out over time
        float alpha = flashColor.a;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * flashSpeed;

            // Create a new color with the reduced alpha
            Color newColor = flashColor;
            newColor.a = alpha;
            damageVignette.color = newColor;

            yield return null; // Wait for next frame
        }

        // 3. Ensure it is perfectly clear at the end
        damageVignette.color = Color.clear;
    }
}