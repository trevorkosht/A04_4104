using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class BaseEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Death
    }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public EnemyHealth healthSystem;

    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int cost;

    [Header("Sticker System")]
    [SerializeField] StickerData myStickerData;
    [SerializeField] GameObject genericStickerPrefab;

    [Header("Visual Effects")]
    [SerializeField] GameObject deathVFX; // Renamed from 'noDrop' for clarity

    [Header("Flash Settings")]
    [SerializeField] public FlashIndicatorData flashData;
    [SerializeField] private GameObject flashIndicatorPrefab;


    [Header("Patrol")]
    public float patrolRange = 10f;
    public float patrolSpeed = 2f;
    [SerializeField] AudioSource patrolNoise;
    private Vector3 startPosition;
    private Vector3 patrolTarget;
    private bool hasPatrolTarget = false;


    protected EnemyState currentState;
    protected float lastAttackTime;
    protected float despawnTime = 1.0f;
    private float soundTime = 10.0f;
    private float soundTimer = 0f;


    protected virtual void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player not found! Make sure your player GameObject is tagged 'Player'.");
            }
        }
    }


    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;

        // Get health system
        healthSystem = GetComponentInChildren<EnemyHealth>();
        healthSystem.OnDeath += OnHealthDepleted;

        //// Hide flash
        //flash.SetActive(false);
        //flashBase.SetActive(false);

    }

    protected virtual void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDeath -= OnHealthDepleted;
        }
    }

    protected virtual void Update()
    {
        // Debug kill key
        /*
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(10);
        }
        */
        soundTimer += Time.deltaTime;
        switch (currentState)
        {
            case EnemyState.Idle: IdleState(); break;
            case EnemyState.Chase: ChaseState(); break;
            case EnemyState.Attack: AttackState(); break;
            case EnemyState.Death: DeathState(); break;
        }
    }
    public void TakeDamage(int amount)
    {
        healthSystem.TakeDamage(amount);
    }

    public void ApplyBurn(int damagePerTick, float duration, float tickRate)
    {
        StartCoroutine(BurnRoutine(damagePerTick, duration, tickRate));
    }

    private IEnumerator BurnRoutine(int damage, float duration, float rate)
    {
        // Optional: Spawn a visual fire effect on the enemy here
        // GameObject fireVFX = Instantiate(firePrefab, transform);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(rate);

            // Deal the DOT damage
            TakeDamage(damage);
            Debug.Log($"{name} is burning! (-{damage} HP)");

            elapsedTime += rate;
        }

        // Optional: Destroy(fireVFX);
    }

    private void OnHealthDepleted()
    {
        SwitchState(EnemyState.Death);
    }

    protected virtual void IdleState()
    {
        if (DetectPlayer()) SwitchState(EnemyState.Chase);

        if (soundTimer >= soundTime)
        {
            patrolNoise.Play();
            soundTimer = 0f; // Reset timer
        }

        // Patrol behavior
        if (!hasPatrolTarget)
        {
            // Get random patrol point around start position
            Vector2 randomCircle = Random.insideUnitCircle * patrolRange;
            patrolTarget = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Try to find valid position on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(patrolTarget, out hit, patrolRange, NavMesh.AllAreas))
            {
                Vector3 sampledPosition = hit.position;

                // Check if path is clear (no walls between current position and target)
                Vector3 direction = sampledPosition - transform.position;
                float distance = direction.magnitude;

                // Raycast to check for walls using tag
                bool pathClear = true;
                RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, direction.normalized, distance);

                foreach (RaycastHit wallHit in hits)
                {
                    if (wallHit.collider.CompareTag("Environment") || wallHit.collider.CompareTag("Untagged"))
                    {
                        pathClear = false;
                        break;
                    }
                }

                if (pathClear)
                {
                    patrolTarget = sampledPosition;
                    agent.SetDestination(patrolTarget);
                    agent.speed = patrolSpeed;
                    hasPatrolTarget = true;
                    patrolNoise.Play();
                }
            }
        }
        else
        {
            // Check if reached patrol target
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                hasPatrolTarget = false;
            }
        }
    }

    protected virtual void ChaseState()
    {
        if (!DetectPlayer())
        {
            SwitchState(EnemyState.Idle);
            return;
        }

        //Debug.Log("CHASING PLAYERRRRRRR");
        if (agent != null) agent.SetDestination(player.position);

        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            SwitchState(EnemyState.Attack);
            agent.ResetPath();
        }
    }

    protected virtual void AttackState()
    {

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(dir);
            SwitchState(EnemyState.Chase);
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(dir);
            lastAttackTime = Time.time;
            PerformAttack();

        }
    }

    protected virtual void DeathState()
    {
        // 1. Always Play Death Effect
        PlayDeathEffect();

        // 2. Try to spawn Sticker
        if (CollectionManager.Instance != null && myStickerData != null)
        {
            if (!CollectionManager.Instance.HasSticker(myStickerData))
            {
                SpawnSticker();
            }
        }

        Destroy(gameObject);
    }

    private void PlayDeathEffect()
    {
        if (deathVFX != null)
        {
            // Spawn effect slightly above ground
            GameObject effect = Instantiate(deathVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(effect, despawnTime);
        }
    }

    private void SpawnSticker()
    {
        if (genericStickerPrefab != null)
        {
            System.Random rnd = new System.Random();

            // Check rarity and duplicate status
            if (rnd.Next(0, 10) < myStickerData.rarity && !CollectionManager.Instance.collectedStickerIds.Contains(myStickerData.id))
            {
                GameObject drop = Instantiate(genericStickerPrefab, transform.position + Vector3.up, Quaternion.identity);
                StickerPickup pickupScript = drop.GetComponent<StickerPickup>();

                if (pickupScript != null)
                {
                    pickupScript.Initialize(myStickerData);
                }
            }
            // Removed the "else" block that spawned smoke here, 
            // because PlayDeathEffect() handles visuals now.
        }
    }

    // Warning inidcator flash.
    protected virtual void FlashWarning(Vector3 position, Quaternion rotation)
    {
        if (flashIndicatorPrefab == null || flashData == null) return;

        GameObject indicatorObj = Instantiate(flashIndicatorPrefab, position, rotation);
        FlashIndicator indicator = indicatorObj.GetComponent<FlashIndicator>();

        if (indicator != null)
        {
            indicator.Initialize(flashData);
            indicator.StartFlashing(position);
        }
    }

    protected abstract void PerformAttack();

    protected virtual bool DetectPlayer()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    protected void SwitchState(EnemyState newState)
    {
        currentState = newState;
    }
}