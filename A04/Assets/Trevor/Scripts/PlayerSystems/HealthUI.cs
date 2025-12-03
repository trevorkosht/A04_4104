using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;

    [Header("Vignette Settings")]
    [SerializeField] private Image damageVignette;
    [SerializeField] private float flashSpeed = 2f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 1f); // Set Alpha to 1 here, we control it via curve

    [Header("Low Health Curve")]
    [Tooltip("X Axis = Health % (0 to 1). Y Axis = Opacity (0 to 1).")]
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private float globalOpacityMultiplier = 0.8f; // Cap the max opacity so it doesn't blind the player

    private int lastKnownHealth;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (playerHealth != null)
        {
            lastKnownHealth = playerHealth.currentHealth;
            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
            UpdateVignetteState(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        if (damageVignette != null)
        {
            damageVignette.raycastTarget = false;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth < lastKnownHealth)
        {
            // Took damage -> Flash
            TriggerDamageFlash(currentHealth, maxHealth);
        }
        else
        {
            // Healed -> Just set the steady state
            UpdateVignetteState(currentHealth, maxHealth);
        }

        lastKnownHealth = currentHealth;
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // --- NEW CURVE LOGIC ---
    private float GetTargetOpacity(int current, int max)
    {
        // 1. Get health as 0.0 to 1.0
        float healthPercent = (float)current / max;

        // 2. Read the graph from the Inspector
        // The graph usually goes from 0 (Left) to 1 (Right).
        // On the graph: Left (0) is Dead, Right (1) is Full Health.
        float curveValue = opacityCurve.Evaluate(healthPercent);

        // 3. Multiply by our global cap
        return curveValue * globalOpacityMultiplier;
    }

    private void UpdateVignetteState(int current, int max)
    {
        if (damageVignette == null) return;
        if (fadeCoroutine != null) return; // Don't interrupt a flash

        float targetAlpha = GetTargetOpacity(current, max);

        Color c = flashColor;
        c.a = targetAlpha;
        damageVignette.color = c;
    }

    private void TriggerDamageFlash(int current, int max)
    {
        if (damageVignette == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeVignette(current, max));
    }

    private IEnumerator FadeVignette(int current, int max)
    {
        // 1. Spike alpha high (Hit Effect)
        // We use the greater of: 0.5f OR the current low-health glow
        // This ensures the flash is always brighter than the steady state
        float targetLowHealthAlpha = GetTargetOpacity(current, max);
        float flashAlpha = Mathf.Max(0.5f, targetLowHealthAlpha + 0.2f);

        Color flashC = flashColor;
        flashC.a = flashAlpha;
        damageVignette.color = flashC;

        float currentAlpha = flashAlpha;

        // 2. Fade down to the Target (Steady State)
        while (currentAlpha > targetLowHealthAlpha)
        {
            currentAlpha -= Time.deltaTime * flashSpeed;

            // Don't fade lower than the steady state requires
            if (currentAlpha < targetLowHealthAlpha) currentAlpha = targetLowHealthAlpha;

            Color newColor = flashColor;
            newColor.a = currentAlpha;
            damageVignette.color = newColor;

            yield return null;
        }

        fadeCoroutine = null;
    }
}