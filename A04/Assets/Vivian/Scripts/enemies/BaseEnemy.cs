using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    [SerializeField] int maxEntityHealth = 30;
    //[SerializeField] float playerDistRange = 5f;

    [Header("Sticker")]
    [SerializeField] GameObject sticker;

    public static int cost;

    protected EnemyState currentState;
    protected float lastAttackTime;
    protected Vector3 patrolPoint;
    private int currentHealth;

    public static int[] stickers = new int[3];
    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxEntityHealth;


    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(10);
        }
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;

            case EnemyState.Chase:
                ChaseState();
                break;

            case EnemyState.Attack:
                AttackState();
                break;

            case EnemyState.Death:
                DeathState();
                break;

        }
    }

    // -------------------------------------
    //        ENEMY HEALTH DEFAULT LOGIC
    // -------------------------------------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxEntityHealth);


        Debug.Log($"Enemy took {amount} damage, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            SwitchState(EnemyState.Death);
        }
    }


    // -------------------------------------
    //        STATE MACHINE DEFAULT LOGIC
    // -------------------------------------

    protected virtual void IdleState()
    {
        Debug.Log("Idling");
        if (DetectPlayer())
        {
            SwitchState(EnemyState.Chase);
            return;
        }
    }


    protected virtual void ChaseState()
    {
        if (!DetectPlayer())
        {
            //Debug.Log("still idling");

            SwitchState(EnemyState.Idle);
            return;
        }

        //Debug.Log("Chasing player");
        agent.SetDestination(player.position);
        // Rotate toward the player
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        if (agent != null) 
        {
            agent.SetDestination(player.position);
        }


        // If in attack range, go to AttackState.
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            //Debug.Log("Trigger attack to player");
            SwitchState(EnemyState.Attack);
            agent.ResetPath();
        }
    }

    protected virtual void AttackState()
    {
        Debug.Log("Attacking player");
        // Attack when player is in range while rotating toward the player.
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        // Double check player is in attack range. If not, go back to chasing.
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            SwitchState(EnemyState.Chase);
            return;
        }

        // If not on cooldown, launch attack.
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack(); // <--- child class defines actual attack
        }
    }

    protected virtual void DeathState()
    {
        //play death animation

        // Check if sticker has been dropped
        if (stickers[0] != 1)
        {
            stickers[0] = 1;  // Note if sticker has been dropped.
            Instantiate(sticker, transform.position, player.rotation);

        }
        Destroy(gameObject);
    }

    // ----------------------------------------------------
    //               METHODS CHILD CLASSES OVERRIDE
    // ----------------------------------------------------

    /// <summary>
    /// Child scripts override this to define their own attack behavior.
    /// </summary>
    protected abstract void PerformAttack();

    /// <summary>
    /// Child scripts can override detection if they want special vision logic.
    /// <summary>
    protected virtual bool DetectPlayer()
    {
        //Debug.Log("Searching for player");
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    // ----------------------------------------------------
    //                     UTILITY
    // ----------------------------------------------------
    protected void SwitchState(EnemyState newState)
    {
        currentState = newState;
    }

}
