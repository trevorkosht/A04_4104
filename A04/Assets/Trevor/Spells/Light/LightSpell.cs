using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// THE FALLING PROJECTILE
public class LightSpell : SpellController
{
    public float speed = 10f;
    [SerializeField] private GameObject healingZonePrefab;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Player"))
        {
            SpawnHealingZone();
            Destroy(gameObject);
        }
    }

    void SpawnHealingZone()
    {
        if (healingZonePrefab != null)
        {
            GameObject zoneObj = Instantiate(healingZonePrefab, transform.position, Quaternion.identity);

            // PASS THE SO DATA TO THE NEW OBJECT
            SpellController zoneScript = zoneObj.GetComponent<SpellController>();
            if (zoneScript != null)
            {
                zoneScript.Initialize(spellData);
            }
        }
    }
}