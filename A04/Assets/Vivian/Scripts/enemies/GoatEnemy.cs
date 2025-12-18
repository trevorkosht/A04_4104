//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static BaseEnemy;
//using static UnityEditor.Progress;

//public class GoatEnemy : BaseEnemy
//{

//    [Header("Charge Attack Settings")]
//    public float chargeSpeed = 5f;
//    public float chargeDuration = 1.5f;
//    public int chargeDamage = 1;
//    public float chargeWindUpTime = 1.5f;

//    [Header("Recoil Settings")]
//    public float recoilDistance = 6f; // User-specified recoil distance
//    public float recoilSpeed = 7f; // How long recoil takes

//    [Header("Charge Visual Effects")]
//    public GameObject chargeWindUpEffect;
//    public GameObject chargeTrailEffect;
//    public AudioClip chargeWindUpSound;
//    public AudioClip chargeAttackSound;

//    // Charge state variables
//    private Vector3 chargeStartPosition;
//    private bool isCharging = false;
//    private bool hasHitPlayerThisAttack = false;
//    private bool canAttack = true; 
//    protected override void Start()
//    {
//        base.Start();
//        cost = 40;
//        flashDuration = chargeWindUpTime;
//    }
//    protected override void PerformAttack()
//    {
//        if (!canAttack || isCharging) return;

//        StartCoroutine(ChargeAttackRoutine());
//    }
//    private IEnumerator ChargeAttackRoutine()
//    {
//        canAttack = false;
//        hasHitPlayerThisAttack = false;

//        // Store initial state
//        chargeStartPosition = transform.position;
//        agent.isStopped = true;

//        // Phase 1: Initial Recoil Back
//        //Debug.Log("Initial recoil back!");
//        yield return StartCoroutine(RecoilBack());

//        // Phase 2: Wind Up
//        //Debug.Log("Charging up attack!");

//        Vector3 directionToPlayer = (player.position - transform.position).normalized;
//        transform.rotation = Quaternion.LookRotation(directionToPlayer);

//        // Calculate movement for the entire charge
//        Vector3 movement = transform.forward * chargeSpeed * Time.deltaTime;
//        float maxChargeDistance = attackRange * 2.5f;

//        FlashWarning();
//        yield return new WaitForSeconds(chargeWindUpTime + 0.5f);

//        // Store the position before charging forward
//        Vector3 preChargePosition = transform.position;

//        // Phase 3: Charge Forward
//        isCharging = true;
//        //Debug.Log("Charging forward!");

//        float chargeTimer = 0f;
//        float currentDistance = 0f;

//        while (chargeTimer < chargeDuration && isCharging && currentDistance < maxChargeDistance)
//        {
//            chargeTimer += Time.deltaTime;

//            // Move forward
//            transform.position += movement;
//            currentDistance = Vector3.Distance(transform.position, preChargePosition);

//            // Check for player hit during charge
//            CheckChargeHit();

//            yield return null;
//        }

//        // Store the end position of the charge
//        Vector3 chargeEndPosition = transform.position;

//        // Phase 4: Recoil Back Again
//        //Debug.Log("Recoiling back again!");

//        // Calculate direction back to starting position
//        Vector3 recoilDirection = (preChargePosition - chargeEndPosition).normalized;

//        // Recoil for the same distance we charged
//        float recoilTimer = 0f;
//        float recoilDuration = currentDistance / chargeSpeed; // Time to travel back

//        while (recoilTimer < recoilDuration)
//        {
//            recoilTimer += Time.deltaTime;

//            // Move back towards the pre-charge position
//            Vector3 recoilMovement = recoilDirection * chargeSpeed * Time.deltaTime;
//            transform.position += recoilMovement;

//            // Ensure we don't overshoot
//            if (Vector3.Distance(transform.position, preChargePosition) < 0.1f)
//                break;

//            yield return null;
//        }

//        // Snap to exact position
//        transform.position = preChargePosition;

//        // Final cleanup
//        isCharging = false;
//        agent.isStopped = false;

//        // IMPORTANT: Reset the attack timer to prevent immediate re-attack
//        lastAttackTime = Time.time;

//        canAttack = true;
//        SwitchState(EnemyState.Chase);
//    }
//    private IEnumerator RecoilBack()
//    {
//        Debug.Log($"Recoiling back {recoilDistance} units");

//        Vector3 recoilDirection = -transform.forward;
//        float distanceMoved = 0f;

//        while (distanceMoved < recoilDistance)
//        {
//            float movementThisFrame = recoilSpeed * Time.deltaTime;

//            // Make sure we don't overshoot
//            if (distanceMoved + movementThisFrame > recoilDistance)
//            {
//                movementThisFrame = recoilDistance - distanceMoved;
//            }

//            // Move backwards incrementally
//            transform.position += recoilDirection * movementThisFrame;
//            distanceMoved += movementThisFrame;

//            yield return null;
//        }

//        // Ensure exact distance
//        transform.position += recoilDirection * (recoilDistance - distanceMoved);

//        Debug.Log("Recoil completed");
//    }
//    private void CheckChargeHit()
//    {
//        if (hasHitPlayerThisAttack) return;

//        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, transform.forward, 2f);

//        foreach (RaycastHit hit in hits)
//        {
//            if (hit.collider.CompareTag("Player"))
//            {
//                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
//                if (playerHealth != null)
//                {
//                    playerHealth.TakeDamage(chargeDamage);
//                    Debug.Log($"Charged into player for {chargeDamage} damage!");
//                    hasHitPlayerThisAttack = true;

//                    // Stop charging when hitting player
//                    isCharging = false;
//                }
//                break;
//            } else if (hit.collider.CompareTag("Environment"))
//            {
//                break;
//            }
//        }
//    }

//    // Handle collisions with walls/obstacles
//    private void OnCollisionEnter(Collision collision)
//    {
//        if (isCharging && !collision.collider.CompareTag("Player"))
//        {
//            isCharging = false;
//            Debug.Log("Charge interrupted by environment");
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseEnemy;

public class GoatEnemy : BaseEnemy
{

    [Header("Charge Attack Settings")]
    public float chargeSpeed = 5f;
    public float chargeDuration = 1.5f;
    public int chargeDamage = 1;
    public float chargeWindUpTime = 1.5f;

    [Header("Recoil Settings")]
    public float recoilDistance = 6f; // User-specified recoil distance
    public float recoilSpeed = 7f; // How long recoil takes

    [Header("Charge Visual Effects")]
    public GameObject chargeWindUpEffect;
    public GameObject chargeTrailEffect;
    public AudioSource chargeWindUpSound;
    public AudioSource chargeAttackSound;

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
        chargeWindUpSound.Play();

        // Phase 1: Initial Recoil Back
        //Debug.Log("Initial recoil back!");
        yield return StartCoroutine(RecoilBack());

        // Store initial state
        chargeStartPosition = transform.position;
        agent.isStopped = true;
        agent.updateRotation = false; // Lock rotation by preventing agent from rotating

        // Phase 2: Wind Up
        //Debug.Log("Charging up attack!");

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        // Calculate movement for the entire charge
        Vector3 movement = transform.forward * chargeSpeed * Time.deltaTime;
        float maxChargeDistance = attackRange * 2.5f;

        Vector3 feetPos = transform.position + transform.forward * ((flashData.length / 2) + 1f);
        feetPos.y = 0.02f;
        FlashWarning(feetPos, transform.rotation);
        yield return new WaitForSeconds(chargeWindUpTime + 0.5f);

        // Store the position before charging forward
        Vector3 preChargePosition = transform.position;

        // Phase 3: Charge Forward
        isCharging = true;
        //Debug.Log("Charging forward!");

        float chargeTimer = 0f;
        float currentDistance = 0f;

        while (chargeTimer < chargeDuration && isCharging && currentDistance < maxChargeDistance)
        {
            chargeTimer += Time.deltaTime;

            // Move forward
            transform.position += movement;
            currentDistance = Vector3.Distance(transform.position, preChargePosition);

            // Check for player hit during charge
            CheckChargeHit();

            yield return null;
        }

        // Store the end position of the charge
        Vector3 chargeEndPosition = transform.position;

        // Phase 4: Recoil Back Again
        //Debug.Log("Recoiling back again!");

        // Calculate direction back to starting position
        Vector3 recoilDirection = (preChargePosition - chargeEndPosition).normalized;

        // Recoil for the same distance we charged
        float recoilTimer = 0f;
        float recoilDuration = currentDistance / chargeSpeed; // Time to travel back

        while (recoilTimer < recoilDuration)
        {
            recoilTimer += Time.deltaTime;

            // Move back towards the pre-charge position
            Vector3 recoilMovement = recoilDirection * chargeSpeed * Time.deltaTime;
            transform.position += recoilMovement;

            // Ensure we don't overshoot
            if (Vector3.Distance(transform.position, preChargePosition) < 0.1f)
                break;

            yield return null;
        }

        // Snap to exact position
        transform.position = preChargePosition;

        // Final cleanup
        isCharging = false;
        agent.isStopped = false;
        agent.updateRotation = true; // unlock rotation

        // IMPORTANT: Reset the attack timer to prevent immediate re-attack
        lastAttackTime = Time.time;

        canAttack = true;
        SwitchState(EnemyState.Chase);
    }
    private IEnumerator RecoilBack()
    {
        Debug.Log($"Recoiling back {recoilDistance} units");

        Vector3 recoilDirection = -transform.forward;
        float distanceMoved = 0f;

        while (distanceMoved < recoilDistance)
        {
            float movementThisFrame = recoilSpeed * Time.deltaTime;

            // Make sure we don't overshoot
            if (distanceMoved + movementThisFrame > recoilDistance)
            {
                movementThisFrame = recoilDistance - distanceMoved;
            }

            // Move backwards incrementally
            transform.position += recoilDirection * movementThisFrame;
            distanceMoved += movementThisFrame;

            yield return null;
        }

        // Ensure exact distance
        transform.position += recoilDirection * (recoilDistance - distanceMoved);

        Debug.Log("Recoil completed");
    }
    private void CheckChargeHit()
    {
        chargeAttackSound.Play();

        if (hasHitPlayerThisAttack) return;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, transform.forward, 0.1f);

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