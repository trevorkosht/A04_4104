using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float lifeTime = 1.5f;
    [SerializeField] int damage = 10;   // how much damage to deal to the player

    private void Start()
    {
        Destroy(gameObject, lifeTime); // destroy after X seconds
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if object is player
        if (other.CompareTag("Player"))
        {
            // Get players health
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            // If player heath exists, player takes damage.
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
