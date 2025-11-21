using UnityEngine;

public class Wind : MonoBehaviour
{
    public float speed = 2f;
    public int damage = 5; // Damage per hit

    private float maxScale = 3f;
    private float currentScaleTime = 0f;
    private float lifetime = 2f;

    void Update()
    {
        // Scale up the wind over time
        if (currentScaleTime < lifetime)
        {
            currentScaleTime += Time.deltaTime;
            float scaleProgress = currentScaleTime / lifetime;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, maxScale, scaleProgress);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if we hit an enemy
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // We do NOT destroy the wind here, because wind should 
            // likely pass through enemies to hit multiple of them.
        }
    }
}