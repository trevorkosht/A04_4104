using UnityEngine;

public class TimeWarpBolt : MonoBehaviour
{
    public float speed = 12f;
    public float cooldownReductionAmount = 2.0f; // Reduces all cooldowns by 2 seconds
    public int damage = 15;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            // REDUCE COOLDOWNS
            if (PlayerSpellSystem.Instance != null)
                PlayerSpellSystem.Instance.ReduceAllCooldowns(cooldownReductionAmount);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}