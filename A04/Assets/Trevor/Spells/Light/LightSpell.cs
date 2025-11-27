using UnityEngine;

public class LightSpell : MonoBehaviour
{
    public float speed = 10f; // Speed of falling

    // Assign your Healing Circle Prefab here in the Inspector
    [SerializeField] private GameObject healingZonePrefab;

    void Update()
    {
        // Move the projectile down
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // We only care if we hit the ground (Environment)
        if (other.CompareTag("Environment"))
        {
            SpawnHealingZone();
            Destroy(gameObject); // Destroy the falling projectile
        }
        // Optional: If we hit the player directly, spawn the circle at their feet
        else if (other.CompareTag("Player"))
        {
            SpawnHealingZone();
            Destroy(gameObject);
        }
    }

    void SpawnHealingZone()
    {
        if (healingZonePrefab != null)
        {
            // Spawn the circle exactly where the projectile hit
            // Quaternion.identity means "no rotation" (flat on ground)
            Instantiate(healingZonePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("LightSpell: No Healing Zone Prefab assigned!");
        }
    }
}