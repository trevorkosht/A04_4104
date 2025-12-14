using UnityEngine;
using System; // Required for Action
using System.Collections;
using System.Collections.Generic;

public class WandMeleeController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private WandAnimation wandAnimation;
    [SerializeField] private PlayerSpellSystem spellSystem;
    [SerializeField] private Transform cameraTransform;

    [Header("Combo Configuration")]
    [SerializeField] private List<WandSwingSO> comboChain;
    [SerializeField] private float comboResetTime = 1.0f;
    [SerializeField] private LayerMask hitLayers;

    // NEW EVENT
    public event Action<int> OnMeleeSwing;

    private int currentComboIndex = 0;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            PerformAttack();
        }

        if (Time.time - lastAttackTime > comboResetTime && currentComboIndex > 0 && !isAttacking)
        {
            currentComboIndex = 0;
        }
    }

    private void PerformAttack()
    {
        if (comboChain.Count == 0) return;

        WandSwingSO swing = comboChain[currentComboIndex];

        // Trigger Audio Event
        OnMeleeSwing?.Invoke(currentComboIndex);

        StartCoroutine(AttackRoutine(swing));

        currentComboIndex++;
        if (currentComboIndex >= comboChain.Count) currentComboIndex = 0;

        lastAttackTime = Time.time;
    }

    private IEnumerator AttackRoutine(WandSwingSO swing)
    {
        isAttacking = true;

        StartCoroutine(wandAnimation.PlaySwingRoutine(swing));

        float timer = 0f;
        bool hasHit = false;

        while (timer < swing.duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / swing.duration;

            if (normalizedTime >= swing.hitWindowStart && normalizedTime <= swing.hitWindowEnd)
            {
                if (!hasHit)
                {
                    if (CheckForHit(swing))
                    {
                        hasHit = true;
                    }
                }
            }

            yield return null;
        }

        isAttacking = false;
    }

    // CHANGED: Return type from void to bool
    private bool CheckForHit(WandSwingSO swing)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Using SphereCast to detect enemies
        if (Physics.SphereCast(ray, 0.5f, out hit, swing.range, hitLayers))
        {
            var enemy = hit.collider.GetComponent<EnemyHealth>(); // Ensure this component exists!

            if (enemy != null)
            {
                enemy.TakeDamage(swing.damage);

                Rigidbody enemyRb = hit.collider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 knockDir = (hit.point - transform.position).normalized;
                    enemyRb.AddForce(knockDir * swing.knockback, ForceMode.Impulse);
                }

                Debug.Log($"Hit {hit.collider.name} for {swing.damage} damage!");

                return true; // WE HIT SOMETHING
            }
        }

        return false; // WE MISSED
    }
}