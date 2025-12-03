using UnityEngine;

public class HealTutorialMonitor : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private int lastKnownHealth;

    void Start()
    {
        // 1. Find the player
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            // 2. Record starting health
            lastKnownHealth = playerHealth.currentHealth;

            // 3. Listen for changes
            playerHealth.OnHealthChanged += CheckForHeal;
        }
    }

    private void CheckForHeal(int current, int max)
    {
        // 4. If current is HIGHER than last known, they healed.
        if (current > lastKnownHealth)
        {
            Debug.Log("Player healed! Restoring Mana and destroying tutorial.");

            // --- RESTORE MANA LOGIC ---
            if (PlayerSpellSystem.Instance != null)
            {
                // We pass a large number (like 1000). 
                // Your SpellSystem automatically clamps it to 'maxMana', so this guarantees full mana.
                PlayerSpellSystem.Instance.RestoreMana(1000);
            }

            // Clean up listener
            playerHealth.OnHealthChanged -= CheckForHeal;

            // Goodbye Tutorial
            Destroy(gameObject);
        }

        // Update memory
        lastKnownHealth = current;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= CheckForHeal;
        }
    }
}