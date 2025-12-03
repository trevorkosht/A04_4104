using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatEnemy : BaseEnemy
{
    [Header("Cat Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected override void Start()
    {
        base.Start();
        cost = 30;
    }

    private System.Collections.IEnumerator Fire()
    {
        Debug.Log("Launched Beam");
        float spawnDistance = 3.75f; // Distance from firePoint
        Vector3 spawnPosition = firePoint.position + (firePoint.forward * spawnDistance);
        FlashWarning();
        yield return new WaitForSeconds(1.0f);
        Beam beamScript = projectilePrefab.GetComponent<Beam>();
        beamScript.Initialize(firePoint);
        Instantiate(projectilePrefab, spawnPosition, firePoint.rotation);
    }


    protected override void PerformAttack()
    {
        // play animation
        StartCoroutine(Fire());
        if (agent != null) { agent.ResetPath(); }
    }

}
