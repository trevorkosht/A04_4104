using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro; // Required for TextMeshPro

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth; // Drag your Player here
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText; // e.g., "80 / 100"

    private void OnEnable()
    {
        // "Subscribe" to the OnHealthChanged event
        // When the event fires, our UpdateHealthUI method will be called
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;

            // Also call it once immediately to set the initial health
            // Note: This is a bit of a hack. You'd ideally use currentHealth
            // from a Start() method, but PlayerHealth's Start might run after this one.
            // The event is the safest way.
        }
    }

    private void OnDisable()
    {
        // "Unsubscribe" when this UI object is disabled or destroyed
        // This is very important to prevent errors
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // Update the slider's max value and current value
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        // Update the text
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}