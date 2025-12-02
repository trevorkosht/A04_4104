using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wind : SpellController
{
    private List<BaseEnemy> enemiesInWind = new List<BaseEnemy>();
    private float maxScale = 3f;
    private float currentScaleTime = 0f;

    public override void Initialize(GridSpellSO data)
    {
        base.Initialize(data);
        // Use tickRate from SO, default to 0.5s if not set
        float interval = (data.tickRate > 0) ? data.tickRate : 0.5f;
        StartCoroutine(DealDamageOverTime(interval));
    }

    void Update()
    {
        // Visual Scaling
        if (currentScaleTime < spellData.duration)
        {
            currentScaleTime += Time.deltaTime;
            float scaleProgress = currentScaleTime / spellData.duration;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, maxScale, scaleProgress);
        }
    }

    IEnumerator DealDamageOverTime(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            for (int i = enemiesInWind.Count - 1; i >= 0; i--)
            {
                if (enemiesInWind[i] != null)
                    enemiesInWind[i].TakeDamage(spellData.power); // From SO
                else
                    enemiesInWind.RemoveAt(i);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null && !enemiesInWind.Contains(enemy)) enemiesInWind.Add(enemy);
    }

    void OnTriggerExit(Collider other)
    {
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null && enemiesInWind.Contains(enemy)) enemiesInWind.Remove(enemy);
    }
}