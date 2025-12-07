using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth health;
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        if (health != null)
        {
            // Subscribe to health changes
            health.OnHealthChanged += UpdateHealthUI;

            // Initialize UI with current values
            UpdateHealthUI(health.currentHealth, health.maxHealth);
        }
        else
        {
            Debug.LogError("EnemyHealth reference not found!", this);
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            if ((int)healthSlider.value != currentHealth)
            {
                int dmg = (int)healthSlider.value - currentHealth;
                showDamage(dmg);
            }
        }
    }
    private void showDamage(int dmg)
    {
        // Create a new GameObject for damage text
        GameObject damageText = new GameObject("DamageText");

        // Make it a child of the current UI object
        damageText.transform.SetParent(transform);

        // Add a Text component
        Text textComponent = damageText.AddComponent<Text>();
        textComponent.text = "-" + dmg;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 20;
        textComponent.color = Color.red;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // Set up RectTransform
        RectTransform rect = damageText.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 30);

        // Random position near the health bar
        float randomX = Random.Range(-30f, 30f);
        float randomY = Random.Range(-20f, 20f);
        rect.anchoredPosition = new Vector2(randomX, randomY);

        // Destroy after 1 second
        Destroy(damageText, 1f);
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        if (health != null)
        {
            health.OnHealthChanged -= UpdateHealthUI;
        }
    }
}
