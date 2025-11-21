using UnityEngine;

public class LightSpell : MonoBehaviour
{
    public float speed = 2f;
    public int healAmount = 20; // Amount to heal
    private float lifetime = 3f;

    [SerializeField] private GameObject collisionEffect;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Check for Player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                SpawnEffect();
                Destroy(gameObject);
                return;
            }
        }

        // 2. Check for Environment
        if (other.CompareTag("Environment"))
        {
            SpawnEffect();
            Destroy(gameObject);
        }
    }

    void SpawnEffect()
    {
        if (collisionEffect != null)
        {
            GameObject spawnedEffect = Instantiate(collisionEffect, transform.position, transform.rotation);
            Destroy(spawnedEffect, 3f);
        }
    }
}