using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    [SerializeField] int damage = 10;   // how much damage to deal to the player

    private void Start()
    {
        Destroy(gameObject, lifeTime); // destroy after X seconds
    }

    private void Update()
    {
        //Debug.Log("Projectile moving: " + transform.position);
        transform.position += (transform.forward + (Vector3.up * 0.17f)) * speed * Time.deltaTime;
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
                Destroy(gameObject);    // Destroy on impact.
            }

        }
        
    }
}
