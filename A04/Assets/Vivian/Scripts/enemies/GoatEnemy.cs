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
    public float chargeWindUpTime = 1.5f;

    [Header("Recoil Settings")]
    public float recoilDistance = 3f; // User-specified recoil distance
    public float recoilDuration = 0.5f; // How long recoil takes

    [Header("Charge Visual Effects")]
    public GameObject chargeWindUpEffect;
    public GameObject chargeTrailEffect;
    public AudioClip chargeWindUpSound;
    public AudioClip chargeAttackSound;

    // Charge state variables
    private Vector3 chargeStartPosition;
    private bool isCharging = false;
    private bool hasHitPlayerThisAttack = false;
    private bool canAttack = true;

    protected override void Start()
    {
        base.Start();
        cost = 40;
    }
    protected override void PerformAttack()
    {
        if (!canAttack || isCharging) return;

        StartCoroutine(ChargeAttackRoutine());
    }

    private IEnumerator ChargeAttackRoutine()
    {
        canAttack = false;
        hasHitPlayerThisAttack = false;

        // Store initial state
        chargeStartPosition = transform.position;
        agent.isStopped = true;

        // Phase 1: Wind Up
        Debug.Log("Charging up attack!");

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        // Save movement distance before warning player of direction.
        Vector3 movement = transform.forward * chargeSpeed * Time.deltaTime;
        FlashWarning();
        yield return new WaitForSeconds(chargeWindUpTime);

        // Phase 2: Charge Forward
        isCharging = true;
        Debug.Log("Charging forward!");

        float chargeTimer = 0f;

        while (chargeTimer < chargeDuration && isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Move forward to saved movement during charge
            transform.position += movement;

            // Check for player hit during charge
            CheckChargeHit();

            // Stop early if we hit max distance
            if (Vector3.Distance(transform.position, chargeStartPosition) >= attackRange * 2.5f)
                break;

            yield return null;
        }

        // Phase 3: Recoil - ALWAYS move back after charge
        yield return StartCoroutine(RecoilBack());

        // Final cleanup
        canAttack = true;
        isCharging = false;
        agent.isStopped = false;

        // IMPORTANT: Reset the attack timer to prevent immediate re-attack
        lastAttackTime = Time.time;

        canAttack = true;
        SwitchState(EnemyState.Chase);
    }


    // Simple recoil that moves goat back
    private IEnumerator RecoilBack()
    {
        Debug.Log($"Recoiling back {recoilDistance} units");

        Vector3 recoilStartPosition = transform.position;
        Vector3 recoilTargetPosition = transform.position + (-transform.forward * recoilDistance);

        float recoilTimer = 0f;

        while (recoilTimer < recoilDuration)
        {
            recoilTimer += Time.deltaTime;
            float progress = recoilTimer / recoilDuration;

            // Smooth movement back
            transform.position = Vector3.Lerp(recoilStartPosition, recoilTargetPosition, progress);

            yield return null;
        }

        // Ensure exact position
        Debug.Log("Recoil completed");
    }

    private void CheckChargeHit()
    {
        if (hasHitPlayerThisAttack) return;

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
                    hasHitPlayerThisAttack = true;

                    // Stop charging when hitting player
                    isCharging = false;
                }
                break;
            }
        }
    }

    // Handle collisions with walls/obstacles
    private void OnCollisionEnter(Collision collision)
    {
        if (isCharging && !collision.collider.CompareTag("Player"))
        {
            isCharging = false;
            Debug.Log("Charge interrupted by environment");
        }
    }
}