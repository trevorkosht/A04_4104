using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] float lifeTime = 1.5f;
    [SerializeField] int damage = 10;   // how much damage to deal to the player
    [HideInInspector] public Transform fp;

    private float beamRange;


    private void Start()
    {
        CheckForPlayer();
        Destroy(gameObject, lifeTime); // destroy after X seconds
    }

    public void Initialize(Transform firePointRef)
    {
        fp = firePointRef;
    }
    private void CheckForPlayer()
    {
        RaycastHit hit;

        // Shoot ray from beam origin forward
        if (Physics.Raycast(fp.position, fp.forward, out hit, beamRange))
        {
            // Check what we hit FIRST
            if (hit.collider.CompareTag("Player"))
            {
                // Player is hit first - deal damage
                PlayerHealth player = hit.collider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
            //Do nothing if beam hits anything else
        }
    }

}
