using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashIndicator : MonoBehaviour
{
    [SerializeField] private GameObject flashBase;  // Bottom layer (always visible)
    [SerializeField] private GameObject flash; // Top layer (flashes)
    [SerializeField] float flashDuration = 0.75f;   // Time flashing warning. 
    [SerializeField] float flashSpeed = 0.2f; // Time between flashes.

    private FlashIndicatorData flashData;

    public void Initialize(FlashIndicatorData data)
    {
        flashData = data;

        // Set size for both planes
        Vector3 scale = new Vector3(data.width, 0.1f, data.length);
        flashBase.transform.localScale = scale;
        flash.transform.localScale = scale;

        // Start hidden
        flash.SetActive(false);
        flashBase.SetActive(false);
    }

    public void StartFlashing(Vector3 enemyPosition)
    {
        // Position at enemy's feet
        transform.position = enemyPosition;
        gameObject.SetActive(true);

        StartCoroutine(Flashing());
    }

    private System.Collections.IEnumerator Flashing()
    {
        float timer = 0f;
        // Show indicator.
        flash.SetActive(true);
        flashBase.SetActive(true);

        while (timer < flashDuration)
        {
            // Toggle visibility
            flash.SetActive(!flash.activeSelf);

            // Wait before next toggle
            yield return new WaitForSeconds(flashSpeed);

            timer += flashSpeed;
        }

        // Hide indicator.
        flash.SetActive(false);
        flashBase.SetActive(false);
    }
}

