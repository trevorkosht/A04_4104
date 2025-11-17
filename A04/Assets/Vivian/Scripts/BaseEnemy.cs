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

    [Header("Sticker")]
    [SerializeField] GameObject sticker;


    protected EnemyState currentState;
    protected float lastAttackTime;
    protected Vector3 patrolPoint;

    public static int[] stickers = new int[3];


    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
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
            SwitchState(EnemyState.Idle);
            return;
        }

        agent.SetDestination(player.position);
        // Rotate toward the player
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        // If in attack range, go to AttackState.
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
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
