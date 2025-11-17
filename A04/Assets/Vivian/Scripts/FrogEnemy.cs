using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : BaseEnemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public void Fire()
    {
        Debug.Log("Launched Bubble");
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
    protected override void PerformAttack()
    {
        // play animation
        Fire();
        // check collsion with player  

    }

    // Change to jumping chase
    protected override void ChaseState()
    {
        // Rotate toward the player
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    protected override void DeathState()
    {
        //play death animation

        // Check if sticker has been dropped
        if (BaseEnemy.stickers[0] != 1)
        {
            BaseEnemy.stickers[0] = 1;  // Note if sticker has been dropped.
            //dropsticker

        }
        this.enabled = false;

    }

}
