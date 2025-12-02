using UnityEngine;

public class Fireball : SpellController
{
    public float speed = 10f;
    [SerializeField] private GameObject collisionEffect;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            // 1. Deal Initial Impact Damage
            enemy.TakeDamage(spellData.power);

            // 2. Apply Burn Effect
            // We pass the DOT stats from the Scriptable Object
            if (spellData.dotDamage > 0 && spellData.duration > 0)
            {
                enemy.ApplyBurn(spellData.dotDamage, spellData.duration, spellData.tickRate);
            }

            SpawnEffect();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Environment"))
        {
            SpawnEffect();
            Destroy(gameObject);
        }
    }

    void SpawnEffect()
    {
        if (collisionEffect) Instantiate(collisionEffect, transform.position, transform.rotation);
    }
}