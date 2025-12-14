using UnityEngine;
using System.Collections;

public class MagicMissile : SpellController
{
    [Header("Movement")]
    public float speed = 5f;
    public float rotateSpeed = 2f;
    public float homingDelay = 0.5f;

    [Header("Effects")]
    public int manaRestored = 5;
    [SerializeField] private GameObject impactEffect;

    private Transform target;
    private bool canHome = false;

    public override void Initialize(GridSpellSO data)
    {
        base.Initialize(data);
        FindNearestTarget();

        StartCoroutine(EnableHomingRoutine());
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

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

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 20f);
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
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

            // --- AUDIO TRIGGER ---
            PlayImpactSound(transform.position);
            // ---------------------

            SpawnEffect();
            Destroy(gameObject);
            return;
        }

        // 2. Check Environment
        if (other.CompareTag("Environment"))
        {
            // --- AUDIO TRIGGER ---
            PlayImpactSound(transform.position);
            // ---------------------

            SpawnEffect();
            Destroy(gameObject);
        }
    }

    void SpawnEffect()
    {
        if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);
    }
}