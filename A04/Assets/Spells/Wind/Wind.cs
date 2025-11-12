using UnityEngine;

public class Wind : MonoBehaviour
{
    public float speed = 2f;
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
}