using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro; // Required for TextMeshPro

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth; // Drag your Player here
    [SerializeField] private Slider healthSlider;
    private void Start()
    {
        if (playerHealth != null)
        {
            // Assuming your PlayerHealth script has public variables for current/max
            // If these are private, you might need to add "Getters" to PlayerHealth
            // Example: playerHealth.GetCurrentHealth()

            // NOTE: Check your PlayerHealth script. If 'currentHealth' is private, 
            // you might need to make it public or add a public method to get it.
            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }
    }
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

    }
}