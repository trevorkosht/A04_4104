using UnityEngine;
using System.Collections; // Added for System.Action

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // This is an event that other scripts (like our UI) can listen to
    // It will send out the current and max health values
    public event System.Action<int, int> OnHealthChanged;

    private void Start()
    {
        // Start the game with full health
        currentHealth = maxHealth;
    }

    // --- TEST FUNCTION ---
    // So you can test if it works
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth -= amount;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // --- FIRE THE EVENT ---
        // Tell all listening scripts that health has changed
        if (OnHealthChanged != null)
        {
            OnHealthChanged(currentHealth, maxHealth);
        }

        Debug.Log($"Player took {amount} damage, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        // Ensure health doesn't go over max
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // --- FIRE THE EVENT ---
        // Tell all listening scripts that health has changed
        if (OnHealthChanged != null)
        {
            OnHealthChanged(currentHealth, maxHealth);
        }

        Debug.Log($"Player healed {amount}, current health: {currentHealth}");
    }

    private void Die()
    {
        // This is where you would trigger a "Game Over" screen,
        // play a death animation, etc.
        Debug.Log("Player has died!");
        GameManager.Instance.TriggerLose();
        // For now, we'll just disable the component
        this.enabled = false;
        
    }
}