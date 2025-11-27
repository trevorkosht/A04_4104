using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealingZone : MonoBehaviour
{
    [Header("Settings")]
    public int healAmountPerTick = 10;
    public float tickInterval = 1.0f; // Heal every 1 second
    public float duration = 5.0f;     // How long the circle lasts

    private List<PlayerHealth> playersInZone = new List<PlayerHealth>();

    void Start()
    {
        // Destroy the circle automatically after 'duration' seconds
        Destroy(gameObject, duration);

        // Start the healing loop
        StartCoroutine(HealLogic());
    }

    IEnumerator HealLogic()
    {
        while (true)
        {
            // Heal everyone currently in the list
            // We loop backwards to safely remove null players if they disconnect/die
            for (int i = playersInZone.Count - 1; i >= 0; i--)
            {
                PlayerHealth player = playersInZone[i];

                if (player != null)
                {
                    player.Heal(healAmountPerTick);
                    Debug.Log($"[HealingZone] Healed {player.name} for {healAmountPerTick}");
                }
                else
                {
                    playersInZone.RemoveAt(i);
                }
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && !playersInZone.Contains(health))
            {
                playersInZone.Add(health);
                Debug.Log($"[HealingZone] Player entered: {other.name}");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && playersInZone.Contains(health))
            {
                playersInZone.Remove(health);
                Debug.Log($"[HealingZone] Player left: {other.name}");
            }
        }
    }
}