using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatEnemy : BaseEnemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public void Fire()
    {
        Debug.Log("Launched Beam");
        float spawnDistance = 3.75f; // Distance from firePoint
        Vector3 spawnPosition = firePoint.position + (firePoint.forward * spawnDistance);
        Instantiate(projectilePrefab, spawnPosition, firePoint.rotation);
    }
    protected override void PerformAttack()
    {
        // play animation
        Fire();
        // check collsion with player  
        if (agent != null) { agent.ResetPath(); }
    }

}
