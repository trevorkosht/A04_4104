//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public abstract class BaseEnemy : MonoBehaviour
//{
//    public enum EnemyState
//    {
//        Idle,
//        Chase,
//        Attack,
//        Death
//    }

//    [Header("References")]
//    public NavMeshAgent agent;
//    public Transform player;
//    public EnemyHeath healthSystem;


//    [Header("Settings")]
//    public float detectionRange = 10f;
//    public float attackRange = 2f;
//    public float attackCooldown = 1.5f;

//    // --- NEW STICKER SYSTEM ---
//    [Header("Sticker System")]
//    [SerializeField] StickerData myStickerData;       // Drag the specific Data (e.g. Goblin)
//    [SerializeField] GameObject genericStickerPrefab; // Drag the ONE generic pickup prefab

//    public static int cost;

//    protected EnemyState currentState;
//    protected float lastAttackTime;
//    private int currentHealth;

//    protected virtual void Start()
//    {
//        currentState = EnemyState.Idle;
//        agent = GetComponent<NavMeshAgent>();
//        healthSystem = GetComponent<EnemyHeath>();
//        healthSystem.OnDeath += OnHealthDepleted;
//    }

//    protected virtual void Update()
//    {
//        // Debug kill key
//        if (Input.GetKeyDown(KeyCode.J))
//        {
//            TakeDamage(10);
//        }

//        switch (currentState)
//        {
//            case EnemyState.Idle: IdleState(); break;
//            case EnemyState.Chase: ChaseState(); break;
//            case EnemyState.Attack: AttackState(); break;
//            case EnemyState.Death: DeathState(); break;
//        }
//    }

//    public void TakeDamage(int amount)
//    {
//        healthSystem?.TakeDamage(amount);

//        if (currentHealth <= 0)
//        {
//            SwitchState(EnemyState.Death);
//        }
//    }

//    // --- STATES ---

//    protected virtual void IdleState()
//    {
//        if (DetectPlayer()) SwitchState(EnemyState.Chase);
//    }

//    protected virtual void ChaseState()
//    {
//        if (!DetectPlayer())
//        {
//            SwitchState(EnemyState.Idle);
//            return;
//        }

//        if (agent != null) agent.SetDestination(player.position);

//        Vector3 dir = (player.position - transform.position).normalized;
//        transform.rotation = Quaternion.LookRotation(dir);

//        if (Vector3.Distance(transform.position, player.position) <= attackRange)
//        {
//            SwitchState(EnemyState.Attack);
//            agent.ResetPath();
//        }
//    }

//    protected virtual void AttackState()
//    {
//        Vector3 dir = (player.position - transform.position).normalized;
//        transform.rotation = Quaternion.LookRotation(dir);

//        if (Vector3.Distance(transform.position, player.position) > attackRange)
//        {
//            SwitchState(EnemyState.Chase);
//            return;
//        }

//        if (Time.time - lastAttackTime >= attackCooldown)
//        {
//            lastAttackTime = Time.time;
//            PerformAttack();
//        }
//    }

//    protected virtual void DeathState()
//    {
//        // 1. Check if we have collected this sticker yet
//        if (CollectionManager.Instance != null && myStickerData != null)
//        {
//            // Only spawn if the player DOES NOT have the sticker yet
//            if (!CollectionManager.Instance.HasSticker(myStickerData))
//            {
//                SpawnSticker();
//            }
//        }

//        // 2. Play animation / Destroy object
//        Destroy(gameObject);
//    }

//    private void SpawnSticker()
//    {
//        if (genericStickerPrefab != null)
//        {
//            // Spawn the generic ball/item
//            GameObject drop = Instantiate(genericStickerPrefab, transform.position + Vector3.up, Quaternion.identity);

//            // Inject the specific data into it
//            StickerPickup pickupScript = drop.GetComponent<StickerPickup>();
//            if (pickupScript != null)
//            {
//                pickupScript.Initialize(myStickerData);
//            }
//        }
//    }

//    // --- ABSTRACTS & UTILS ---

//    protected abstract void PerformAttack();

//    protected virtual bool DetectPlayer()
//    {
//        return Vector3.Distance(transform.position, player.position) <= detectionRange;
//    }

//    protected void SwitchState(EnemyState newState)
//    {
//        currentState = newState;
//    }
//}

using UnityEngine;
using UnityEngine.AI;

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

    [Header("Sticker System")]
    [SerializeField] StickerData myStickerData;
    [SerializeField] GameObject genericStickerPrefab;

    protected EnemyState currentState;
    protected float lastAttackTime;

    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
        agent = GetComponent<NavMeshAgent>();

        // Get health system
        healthSystem = GetComponentInChildren<EnemyHealth>();
        healthSystem.OnDeath += OnHealthDepleted;
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
        Debug.Log("Enemy damg");
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
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            SwitchState(EnemyState.Chase);
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack();
        }
    }

    protected virtual void DeathState()
    {
        if (CollectionManager.Instance != null && myStickerData != null)
        {
            if (!CollectionManager.Instance.HasSticker(myStickerData))
            {
                SpawnSticker();
            }
        }

        Destroy(gameObject);
    }

    private void SpawnSticker()
    {
        if (genericStickerPrefab != null)
        {
            GameObject drop = Instantiate(genericStickerPrefab, transform.position + Vector3.up, Quaternion.identity);
            StickerPickup pickupScript = drop.GetComponent<StickerPickup>();
            if (pickupScript != null)
            {
                pickupScript.Initialize(myStickerData);
            }
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