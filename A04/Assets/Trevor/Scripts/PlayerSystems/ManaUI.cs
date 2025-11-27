using UnityEngine;
using UnityEngine.UI; // Required for Slider

public class ManaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider manaSlider; // Drag your Slider here

    void Start()
    {
        // Check if the system exists
        if (PlayerSpellSystem.Instance != null)
        {
            // Subscribe to the event so it updates automatically later
            PlayerSpellSystem.Instance.OnManaChanged += UpdateManaDisplay;

            // Force an immediate update right now so it's not empty at start
            UpdateManaDisplay(PlayerSpellSystem.Instance.currentMana, PlayerSpellSystem.Instance.maxMana);
        }
        else
        {
            Debug.LogWarning("ManaUI: PlayerSpellSystem not found!");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent errors when scene changes
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnManaChanged -= UpdateManaDisplay;
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
}