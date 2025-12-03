using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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

    [Header("Warning System")]
    [SerializeField] GameObject flash;
    [SerializeField] GameObject flashBase;
    public float flashDuration = .75f;   // Time flashing warning. 
    public float flashSpeed = 0.2f; // Time between flashes.


    protected EnemyState currentState;
    protected float lastAttackTime;
    protected float despawnTime = 1.0f;

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

        // Get health system
        healthSystem = GetComponentInChildren<EnemyHealth>();
        healthSystem.OnDeath += OnHealthDepleted;

        // Hide flash
        flash.SetActive(false);
        flashBase.SetActive(false);

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
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(10);
        }

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
    protected virtual void FlashWarning()
    {
        StartCoroutine(Flashing());
    }

    private System.Collections.IEnumerator Flashing()
    {
        float timer = 0f;

        // Get NavMeshAgent reference
        Vector3 originalDirection = agent.transform.forward;

        if (agent != null)
        {
            // Store original state
            Vector3 originalVelocity = agent.velocity;

            // Stop movement but keep agent enabled
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            // Lock rotation by preventing agent from rotating
            agent.updateRotation = false;
            agent.transform.forward = originalDirection;
        }


        // Show indicator.
        flash.SetActive(true);
        flashBase.SetActive(true);

        while (timer < flashDuration)
        {
            // Toggle visibility
            flash.SetActive(!flash.activeSelf);

            // Wait before next toggle
            yield return new WaitForSeconds(flashSpeed);

            timer += flashSpeed;
        }

        // Hide indicator.
        flash.SetActive(false);
        flashBase.SetActive(false);
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