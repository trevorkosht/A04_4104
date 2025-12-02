// THE HEALING ZONE ITSELF
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingZone : SpellController
{
    private List<PlayerHealth> playersInZone = new List<PlayerHealth>();

    public override void Initialize(GridSpellSO data)
    {
        base.Initialize(data);
        float interval = (data.tickRate > 0) ? data.tickRate : 1.0f;
        StartCoroutine(HealLogic(interval));
    }

    IEnumerator HealLogic(float interval)
    {
        while (true)
        {
            for (int i = playersInZone.Count - 1; i >= 0; i--)
            {
                PlayerHealth player = playersInZone[i];
                if (player != null)
                {
                    player.Heal(spellData.power); // Uses "Power" as Heal Amount
                }
                else
                {
                    playersInZone.RemoveAt(i);
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && !playersInZone.Contains(health)) playersInZone.Add(health);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && playersInZone.Contains(health)) playersInZone.Remove(health);
        }
    }
}