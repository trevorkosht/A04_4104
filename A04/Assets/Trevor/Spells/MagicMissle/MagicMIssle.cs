using UnityEngine;
using System.Collections;

public class MagicMissile : SpellController
{
    [Header("Movement")]
    public float speed = 5f; // Set this low in Inspector for "Slow" feel
    public float rotateSpeed = 2f; // Lower rotation = wider, smoother turns
    public float homingDelay = 0.5f; // Time to fly straight before turning

    [Header("Effects")]
    public int manaRestored = 5;
    [SerializeField] private GameObject impactEffect;

    private Transform target;
    private bool canHome = false; // Controls when homing starts

    public override void Initialize(GridSpellSO data)
    {
        base.Initialize(data);
        FindNearestTarget();

        // Start the delay timer
        StartCoroutine(EnableHomingRoutine());
    }

    void Update()
    {
        // Always move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Only rotate if delay is over and we have a target
        if (canHome && target != null)
        {
            HomingBehavior();
        }
    }

    IEnumerator EnableHomingRoutine()
    {
        yield return new WaitForSeconds(homingDelay);
        canHome = true;
    }

    void HomingBehavior()
    {
        Vector3 targetDirection = target.position - transform.position;
        float singleStep = rotateSpeed * Time.deltaTime;

        // RotateTowards makes it turn smoothly rather than snapping
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 20f);
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            // Note: GetComponentInParent is expensive, but okay for single-cast events
            if (hit.GetComponentInParent<BaseEnemy>())
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
        // 1. Check Enemy
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(spellData.power);
            if (PlayerSpellSystem.Instance != null) PlayerSpellSystem.Instance.RestoreMana(manaRestored);

            SpawnEffect();
            Destroy(gameObject);
            return;
        }

        // 2. Check Environment
        // This naturally ignores other missiles unless they are tagged "Environment"
        if (other.CompareTag("Environment"))
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