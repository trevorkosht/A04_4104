using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wind : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2f;
    public int damagePerTick = 5;
    public int numberOfHits = 3;

    [Header("Visuals")]
    private float maxScale = 3f;
    private float currentScaleTime = 0f;
    private float lifetime = 2f;

    private List<BaseEnemy> enemiesInWind = new List<BaseEnemy>();

    void Start()
    {
        float tickInterval = lifetime / numberOfHits;
        Debug.Log($"[Wind] Spawned! Lifetime: {lifetime}s. Ticking damage every {tickInterval}s.");
        StartCoroutine(DealDamageOverTime(tickInterval));
    }

    void Update()
    {
        if (currentScaleTime < lifetime)
        {
            currentScaleTime += Time.deltaTime;
            float scaleProgress = currentScaleTime / lifetime;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, maxScale, scaleProgress);
        }
        else
        {
            Debug.Log("[Wind] Lifetime expired. Destroying spell.");
            Destroy(gameObject);
        }
    }

    IEnumerator DealDamageOverTime(float interval)
    {
        // Wait for the first tick
        yield return new WaitForSeconds(interval);

        while (true)
        {
            Debug.Log($"[Wind] Damage Tick! Enemies currently in list: {enemiesInWind.Count}");

            for (int i = enemiesInWind.Count - 1; i >= 0; i--)
            {
                BaseEnemy enemy = enemiesInWind[i];

                if (enemy != null)
                {
                    Debug.Log($"[Wind] Dealt {damagePerTick} damage to {enemy.name}");
                    enemy.TakeDamage(damagePerTick);
                }
                else
                {
                    enemiesInWind.RemoveAt(i);
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Log exactly what object the wind touched
        Debug.Log($"[Wind] Trigger Enter with: {other.gameObject.name} (Tag: {other.tag})");

        // 2. Try to find the script
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();

        if (enemy != null)
        {
            Debug.Log($"[Wind] Found 'BaseEnemy' script on parent: {enemy.name}");

            if (!enemiesInWind.Contains(enemy))
            {
                enemiesInWind.Add(enemy);
                Debug.Log($"[Wind] ADDED {enemy.name} to target list.");
            }
        }
        else
        {
            Debug.Log($"[Wind] Hit {other.name}, but NO 'BaseEnemy' script found in parent.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null && enemiesInWind.Contains(enemy))
        {
            Debug.Log($"[Wind] {enemy.name} exited the wind area. Removing from list.");
            enemiesInWind.Remove(enemy);
        }
    }
}