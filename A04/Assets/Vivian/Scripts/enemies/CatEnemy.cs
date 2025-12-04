using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatEnemy : BaseEnemy
{
    [Header("Attack Settings")]
    public Transform firePoint;
    [SerializeField] float lifeTime = 1.5f;
    [SerializeField] int damage = 10;
    [SerializeField] float beamRange = 8f;
    [SerializeField] float beamWidth = 1.5f;
    [SerializeField] GameObject hitVFX;

    protected override void Start()
    {
        base.Start();
        cost = 30;
    }

    private System.Collections.IEnumerator Fire()
    {
        if (agent != null) agent.isStopped = true;
        FlashWarning();
        yield return new WaitForSeconds(1.0f);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, beamWidth, transform.forward, beamRange);
        Vector3 rayStart = firePoint.position;
        Vector3 rayDirection = firePoint.forward;

        // Play fire animation
        GameObject flash = Instantiate(hitVFX, rayStart, Quaternion.LookRotation(rayStart));
        Destroy(flash, 2.0f);

        // Check all hits by raycast
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Player"))  // Player takes dmg if raycast hits
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                   
                }
                break;  // Return after hitting player
            }
           
        }

        yield return null;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }

    }

    protected override void PerformAttack()
    {
        // play animation
        StartCoroutine(Fire());
    }

}
