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
    public float yOffset = 0.5f;

    [Header("Sticker")]
    [SerializeField] GameObject sticker;

    private CharacterController characterController;
    protected EnemyState currentState;
    protected float lastAttackTime;
    protected Vector3 patrolPoint;

    public static int[] stickers = new int[3];


    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
        agent = GetComponent<NavMeshAgent>();
        //// Y-axis boost to prevent enemy from sinking
        //agent.enabled = false;
        //transform.position += Vector3.up * yOffset;
        //Debug.Log(transform.position);
        //agent.enabled = true;
        characterController = GetComponent<CharacterController>();


    }

    protected virtual void Update()
    {
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
        // Double check player is in attack range. If not, go back to chasing.
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            SwitchState(EnemyState.Chase);
            return;
        }

        Debug.Log("Attacking player");
        // Attack when player is in range while rotating toward the player.
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        // If not on cooldown, launch attack.
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack(); // <--- child class defines actual attack
        }
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

    /// <summary>
    /// Child scripts can override deathstate so it drops sticker once.
    /// <summary>
    protected abstract void DeathState();


    // ----------------------------------------------------
    //                     UTILITY
    // ----------------------------------------------------
    protected void SwitchState(EnemyState newState)
    {
        currentState = newState;
    }

}
