using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using static BaseEnemy;

public class GoatEnemy : BaseEnemy
{

    [Header("Charge Attack Settings")]
    [SerializeField] float chargeSpeed = 5f;
    [SerializeField] float chargeDuration = 1.5f;
    [SerializeField] int chargeDamage = 1;
    [SerializeField] float chargeWindUpTime = 1.5f;


    [Header("Recoil Settings")]
    [SerializeField] float recoilDistance = 6f; // User-specified recoil distance
    [SerializeField] float recoilDuration = 0.5f; // How long recoil takes

    [Header("Charge Visual Effects")]
    [SerializeField] GameObject chargeWindUpEffect;
    [SerializeField] GameObject chargeTrailEffect;
    [SerializeField] GameObject chargeHitEffect;
    [SerializeField] AudioClip chargeWindUpSound;
    [SerializeField] AudioClip chargeAttackSound;

    // Charge state variables
    private Vector3 chargeStartPosition;
    private bool isCharging = false;
    private bool hasHitPlayerThisAttack = false;
    private bool canAttack = true;
    private bool collided = false;


    protected override void Start()
    {
        base.Start();
        cost = 40;
        flashDuration = chargeWindUpTime;
    }
    protected override void Update()
    {
        base.Update();
        if (isCharging)
        {
            // Check for player in front manually
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, transform.forward, (chargeSpeed * chargeDuration));

            // Check all hits by raycast
            foreach (RaycastHit hit in hits)
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.CompareTag("Player") && !hasHitPlayerThisAttack)  // Player takes dmg if raycast hits, make sure it only hits once
                {
                    PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        // Play fire animation
                        GameObject flash = Instantiate(chargeHitEffect, transform.position, Quaternion.LookRotation(transform.position));
                        Destroy(flash, 2.0f);

                        playerHealth.TakeDamage(chargeDamage);
                        hasHitPlayerThisAttack = true;
                    }
                    break;  // Return after hitting player
                }
            }
        }

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
        //chargeStartPosition = transform.position;
        agent.isStopped = true;

        // Phase 1: Wind Up
        Debug.Log("Charging up attack!");

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        //// Save movement distance before warning player of direction.
        Vector3 movement = transform.forward * chargeSpeed * Time.deltaTime;
        FlashWarning();
        yield return new WaitForSeconds(chargeWindUpTime+0.5f);

        // Phase 2: Charge Forward
        isCharging = true;
        //Debug.Log("Charging forward!");

      
        float chargeTimer = 0f;
        Vector3 chargeDirection = transform.forward; // Save direction at start

        while (chargeTimer < chargeDuration && isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Move forward continuously
            transform.position += movement; // This allows Update to be called

            yield return null;

            // Stop early if we hit max distance
            float distanceTraveled = Vector3.Distance(transform.position, chargeStartPosition);
            if (distanceTraveled >= attackRange * 2.5f)
            {
                //Debug.Log($"Max charge distance reached: {distanceTraveled}");
                break;
            }

            yield return null; // This allows Update to be called
        }

        // Phase 3: Recoil - ALWAYS move back after charge
        yield return StartCoroutine(RecoilBack());

        // Final cleanup
        canAttack = true;
        isCharging = false;
        agent.isStopped = false;
        hasHitPlayerThisAttack = false;

        // IMPORTANT: Reset the attack timer to prevent immediate re-attack
        lastAttackTime = Time.time;

        canAttack = true;
        SwitchState(EnemyState.Chase);
    }


    // Simple recoil that moves goat back
    private IEnumerator RecoilBack()
    {
        //Debug.Log($"Recoiling back {recoilDistance} units");

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

    }

}