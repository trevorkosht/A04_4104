using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 15f;
    public float rotateSpeed = 5f;
    public int manaRestoredOnHit = 5;
    public int damage = 10;

    [Header("References")]
    [SerializeField] private GameObject impactEffect;

    private Transform target;
    private float lifetime = 4f;

    void Start()
    {
        FindNearestTarget();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        MoveForward();
        if (target != null) HomingBehavior();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void HomingBehavior()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = rotateSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void FindNearestTarget()
    {
        // Simple sphere check to find enemies
        Collider[] hits = Physics.OverlapSphere(transform.position, 20f);
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.GetComponent<BaseEnemy>()) // Check for your BaseEnemy script
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    target = hit.transform;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            // RESTORE MANA
            if (PlayerSpellSystem.Instance != null)
                PlayerSpellSystem.Instance.RestoreMana(manaRestoredOnHit);

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
        if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);
    }
}