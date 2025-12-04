using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] float lifeTime = 1.5f;
    [SerializeField] int damage = 10;
    [SerializeField] float beamRange = 8f;

    [SerializeField] private Transform beamTip;

    private void Start()
    {
        // Add debug to see what's happening
        Debug.Log($"Beam spawned at: {transform.position}");
        Debug.Log($"Beam rotation: {transform.rotation.eulerAngles}");
        Debug.Log($"Beam forward: {transform.forward}");
        Debug.Log($"Beam right: {transform.right}");

        CheckForPlayer();
        Destroy(gameObject, lifeTime);
    }

    private void CheckForPlayer()
    {
        // Safety check - beamTip might be null
        if (beamTip == null)
        {
            Debug.LogError("BeamTip is not assigned! Assign it in the Inspector or fix the code.");
            return;
        }

        RaycastHit hit;

        // MOST LIKELY FIX: Use transform.forward instead of transform.right
        Vector3 rayDirection = transform.forward; // Change to forward (Z axis)

        // Add layer mask to ignore certain layers
        LayerMask playerLayer = LayerMask.GetMask("Player");
        LayerMask obstacleLayer = LayerMask.GetMask("Default"); // Or whatever layer obstacles are on
        LayerMask raycastMask = playerLayer | obstacleLayer;

        Debug.DrawRay(beamTip.position, rayDirection * beamRange, Color.red, 2f);

        // Shoot the raycast
        if (Physics.Raycast(beamTip.position, rayDirection, out hit, beamRange, raycastMask))
        {
            Debug.Log($"Beam hit: {hit.collider.name} at distance {hit.distance}");

            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth player = hit.collider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                    Debug.Log($"Dealt {damage} damage to player!");
                }
            }
            else
            {
                Debug.Log($"Hit non-player: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("Beam didn't hit anything");
        }
    }

    // Visualize in editor
    private void OnDrawGizmosSelected()
    {
        Vector3 rayStartPoint = (beamTip != null) ? beamTip.position : transform.position;
        Vector3 rayDirection = transform.forward; // Match what's in CheckForPlayer

        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayStartPoint, rayDirection * beamRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(rayStartPoint, 0.1f);
    }
}
