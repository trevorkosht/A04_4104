using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseEnemy;

public class GoatEnemy : BaseEnemy
{
    [Header("Charge Attack Settings")]
    public float chargeSpeed = 10f;
    public float chargeDuration = 1.5f;
    public int chargeDamage = 1;
    public float chargeWindUpTime = 0.8f;
    public float returnSpeed = 5f;

    [Header("Charge Visual Effects")]
    public GameObject chargeWindUpEffect;
    public GameObject chargeTrailEffect;
    public AudioClip chargeWindUpSound;
    public AudioClip chargeAttackSound;

    // Charge state variables
    private Vector3 chargeStartPosition;
    private Vector3 chargeTargetPosition;
    private bool isCharging = false;
    private bool isReturning = false;
    private bool hasHitPlayerThisAttack = false;

    protected override void PerformAttack()
    {
        StartCoroutine(ChargeAttackRoutine());
    }

    private IEnumerator ChargeAttackRoutine()
    {
        hasHitPlayerThisAttack = false;

        // Store initial state
        chargeStartPosition = transform.position;
        agent.isStopped = true; // Stop NavMesh agent during charge

        // Phase 1: Wind Up
        Debug.Log("Charging up attack!");

        // Face player during wind up
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        //// Visual and audio effects for wind up
        //if (chargeWindUpEffect != null)
        //    Instantiate(chargeWindUpEffect, transform.position, transform.rotation);

        //if (chargeWindUpSound != null)
        //    AudioSource.PlayClipAtPoint(chargeWindUpSound, transform.position);

        //// Animation trigger for wind up
        //if (animator != null)
        //    animator.SetTrigger("ChargeWindUp");

        yield return new WaitForSeconds(chargeWindUpTime);

        // Phase 2: Charge Forward
        isCharging = true;
        Debug.Log("Charging forward!");

        // Calculate charge target (charge past player a bit)
        chargeTargetPosition = transform.position + transform.forward * (attackRange * 3f);

        //// Visual effects for charging
        //if (chargeTrailEffect != null)
        //    Instantiate(chargeTrailEffect, transform.position, transform.rotation, transform);

        //if (chargeAttackSound != null)
        //    AudioSource.PlayClipAtPoint(chargeAttackSound, transform.position);

        // Animation trigger for charge
        //if (animator != null)
        //    animator.SetBool("IsCharging", true);

        float chargeTimer = 0f;

        while (chargeTimer < chargeDuration && isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Move forward during charge
            Vector3 movement = transform.forward * chargeSpeed * Time.deltaTime;
            transform.position += movement;

            // Check for player hit during charge
            CheckChargeHit();

            // Optional: Stop early if we hit a wall or reached target
            if (Vector3.Distance(transform.position, chargeStartPosition) >= attackRange * 2.5f)
                break;

            yield return null;
        }

        // Phase 3: Return to Start Position
        isCharging = false;
        isReturning = true;
        Debug.Log("Returning to start position");

        //if (animator != null)
        //    animator.SetBool("IsCharging", false);

        agent.isStopped = false; // Re-enable NavMesh agent
        agent.SetDestination(chargeStartPosition);


        // Final cleanup
        isReturning = false;

        Debug.Log("Charge attack completed!");

        // Return to chase state to continue pursuing player
        SwitchState(EnemyState.Chase);
    }

    private void CheckChargeHit()
    {
        // If we already hit the player this charge attack, don't check again
        if (hasHitPlayerThisAttack) return;

        // Check for player in front during charge
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, transform.forward, 2f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(chargeDamage);
                    Debug.Log($"Charged into player for {chargeDamage} damage!");
                    hasHitPlayerThisAttack = true; // Mark that we've hit the player THIS attack
                }
                break; // Only hit player once per attack
            }
        }
    }

    // Optional: Handle collisions with walls/obstacles
    private void OnCollisionEnter(Collision collision)
    {
        if (isCharging && !collision.collider.CompareTag("Player"))
        {
            // Stop charge early if hitting a wall
            if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Obstacle"))
            {
                isCharging = false;
                Debug.Log("Charge interrupted by obstacle");
            }
        }
    }

    // Make sure to clean up if enemy gets destroyed during charge
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // Gizmos for visualization
    private void OnDrawGizmosSelected()
    {
        if (isCharging)
        {
            // Draw charge direction
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 5f);

            // Draw charge detection sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}
