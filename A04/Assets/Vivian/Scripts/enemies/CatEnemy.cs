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
    [SerializeField] GameObject attack;
    [SerializeField] AudioSource scratch;


    protected override void Start()
    {
        base.Start();
        cost = 30;
    }

    private System.Collections.IEnumerator Fire()
    {
        agent.isStopped = true;
        agent.updateRotation = false; // lock rotation

        Vector3 feetPos = transform.position + transform.forward * ((flashData.length / 2)+1);
        feetPos.y = 0.04f;
        FlashWarning(feetPos, transform.rotation);
        yield return new WaitForSeconds(1.0f);
        scratch.Play();

        Vector3 rayStart = firePoint.position;
        Vector3 rayDirection = firePoint.forward;

        // Play fire animation
        GameObject flash = Instantiate(hitVFX, rayStart, Quaternion.LookRotation(rayDirection));
        StartCoroutine(MoveVFXForward(flash, rayDirection, 0.5f)); // Move over 0.5 seconds

        GameObject shoot = Instantiate(attack, rayStart, Quaternion.LookRotation(rayDirection));
        Destroy(shoot, 1.0f);

        // Single raycast to find the FIRST thing hit
        RaycastHit hit;
        if (Physics.Raycast(rayStart, rayDirection, out hit, beamRange))
        {
            Debug.Log("Hit: " + hit.collider.name + " at distance: " + hit.distance);

            // Check if hit object OR any of its parents have the Player tag
            Transform current = hit.transform;
            while (current != null)
            {
                if (current.CompareTag("Player"))
                {
                    PlayerHealth playerHealth = current.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                        break; // Found and damaged player
                    }
                }
                current = current.parent;
            }
        }

        yield return null;

        agent.updateRotation = true; // unlock rotation
        agent.isStopped = false;
        agent.ResetPath();

    }

    private System.Collections.IEnumerator MoveVFXForward(GameObject vfx, Vector3 direction, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = vfx.transform.position;
        float distance = beamRange; // or your desired distance
        Vector3 targetPos = startPos + direction * distance;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            vfx.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Optionally destroy when reaching target
        Destroy(vfx);
    }

    protected override void PerformAttack()
    {
        // play animation
        StartCoroutine(Fire());
    }

}
