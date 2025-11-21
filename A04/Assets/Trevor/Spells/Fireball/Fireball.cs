using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10; // Amount of damage to deal
    private float lifetime = 3f;
    [SerializeField] private GameObject collisionEffect;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Check for Enemy
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            SpawnEffect();
            Destroy(gameObject);
            return; // Stop here so we don't hit environment too
        }

        // 2. Check for Environment (Walls/Floors)
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
            Instantiate(collisionEffect, transform.position, transform.rotation);
        }
    }
}