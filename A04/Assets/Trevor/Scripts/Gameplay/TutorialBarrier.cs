using UnityEngine;
using UnityEngine.Events;

public class TutorialBarrier : MonoBehaviour
{
    // public UnityEvent onDestroyed; // Add this line
    public enum RequiredSpell
    {
        Fireball,
        MagicMissile,
        Wind
    }

    [Header("Settings")]
    [Tooltip("Which spell breaks this wall?")]
    public RequiredSpell weakness;

    [Tooltip("Amount of damage to deal to the player when this wall breaks (Used for Wall 3).")]
    public int damagePlayerOnBreak = 0;

    [Header("Visuals")]
    [SerializeField] private GameObject breakEffect;

    private void OnTriggerEnter(Collider other)
    {
        bool isCorrectSpell = false;

        // Check which spell hit us based on the Enum
        switch (weakness)
        {
            case RequiredSpell.Fireball:
                // Check if the object has the Fireball script
                if (other.GetComponent<Fireball>() != null) isCorrectSpell = true;
                break;

            case RequiredSpell.MagicMissile:
                // Check if the object has the MagicMissile script
                if (other.GetComponent<MagicMissile>() != null) isCorrectSpell = true;
                break;

            case RequiredSpell.Wind:
                // Check if the object has the Wind script
                if (other.GetComponent<Wind>() != null) isCorrectSpell = true;
                break;
        }

        if (isCorrectSpell)
        {
            BreakWall(other.gameObject);
        }
    }

    private void BreakWall(GameObject spellObject)
    {
        Debug.Log($"Tutorial Barrier broken by {weakness}!");

        // 1. Damage Player (For the Wind Wall scenario)
        if (damagePlayerOnBreak > 0)
        {
            // Find the player's health script and apply damage
            PlayerHealth player = FindObjectOfType<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damagePlayerOnBreak);
            }
        }

        // 2. Visuals
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, transform.rotation);
        }

        // 3. Cleanup
        // Destroy the wall
        // onDestroyed?.Invoke();

        Destroy(gameObject);

        // Optional: Destroy the spell projectile too so it looks like it exploded on impact
        // (Don't do this for Wind, as Wind is a lingering zone)
        if (weakness != RequiredSpell.Wind)
        {
            Destroy(spellObject);
        }
    }
}