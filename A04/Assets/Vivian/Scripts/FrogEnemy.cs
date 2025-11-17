using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FrogEnemy : BaseEnemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float jumpTimer = 0f;
    private bool isJumping = false;
    private Vector3 jumpStartPosition;

    [Header("Jump Settings")]
    [SerializeField] private float jumpInterval = 0.5f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f; // Total jump time

    public void Fire()
    {
        Debug.Log("Launched Bubble");
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
    protected override void PerformAttack()
    {
        // play animation
        Fire();
        // check collsion with player  
        if (agent != null) { agent.ResetPath(); }


    }

    // Change to jumping chase
    protected override void ChaseState()
    {
        // Rotate toward the player
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
        if (agent != null) { agent.SetDestination(player.position); }

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
            if (agent != null) { agent.ResetPath(); }
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
    protected override void DeathState()
    {
        //play death animation

        // Check if sticker has been dropped
        if (BaseEnemy.stickers[0] != 1)
        {
            BaseEnemy.stickers[0] = 1;  // Note if sticker has been dropped.
            //dropsticker

        }
        this.enabled = false;

    }

}
