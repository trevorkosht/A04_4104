using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatEnemy : BaseEnemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public void Fire()
    {
        Debug.Log("Launched Bubble");
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

    }
    protected override void PerformAttack()
    {
        // play animation
        Fire();
        // check collsion with player  
        if (agent != null) { agent.ResetPath(); }
    }

}
