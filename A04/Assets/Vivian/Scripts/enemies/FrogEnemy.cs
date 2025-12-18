using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FrogEnemy : BaseEnemy
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    [SerializeField] GameObject hitVFX;
    [SerializeField] AudioSource fireSound;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpDuration = 0.5f; // Total jump time

    private bool isJumping = false;
    private Vector3 jumpStartPosition;
    protected override void Start()
    {
        base.Start();
        cost = 40;
    }
    private System.Collections.IEnumerator Fire()
    {
        // Launch bubble
        // Debug.Log("Launched Bubble");
        fireSound.Play();
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        yield return null;
    }
    protected override void PerformAttack()
    {
        GameObject flash = Instantiate(hitVFX, firePoint.position, firePoint.rotation); // Play fire animation

        StartCoroutine(Fire());
        Destroy(flash,1.0f);
    }

    // Change to jumping chase
    protected override void ChaseState()
    {
        // Rotate toward the player
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        // Handle jumping logic
        if (!isJumping)
        {
            if (!(Time.time - lastAttackTime >= attackCooldown))
            {
                StartCoroutine(Jump());
            }
        }

        // If in attack range, go to AttackState.
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            //Debug.Log("Trigger attack to player");
            SwitchState(EnemyState.Attack);
        }
    }

    private System.Collections.IEnumerator Jump()
    {
        isJumping = true;
        agent.enabled = false;

        jumpStartPosition = transform.position;

        Vector3 forwardDirection = (player.position - transform.position).normalized;
        forwardDirection.y = 0;
        forwardDirection = forwardDirection.normalized;

        Vector3 jumpTargetPosition = jumpStartPosition + forwardDirection * 1f;

        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / jumpDuration;

            // Parabolic arc
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            // Forward movement with arc
            Vector3 horizontalPosition = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, progress);
            Vector3 verticalOffset = Vector3.up * height;

            transform.position = horizontalPosition + verticalOffset;

            // Face the direction we're jumping
            if (forwardDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(forwardDirection);
            }

            yield return null;
        }

        transform.position = jumpTargetPosition;
        agent.enabled = true;

        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        isJumping = false;
    }

}
