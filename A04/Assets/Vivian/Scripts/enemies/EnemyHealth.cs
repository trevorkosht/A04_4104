using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30;

    public event Action<int, int> OnHealthChanged; // currentHealth, maxHealth
    public event Action<int> OnDamageTaken; // damageAmount
    public event Action OnDeath;

    private int currentHealth;
    private bool isDead = false;
    private float trembleIntensity = 0.05f;
    private float trembleDuration = 0.5f;
    private bool isTrembling = false;
    private Vector3 originalPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        originalPosition = transform.localPosition;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Start tremble effect
        if (!isTrembling)
        {
            StartCoroutine(TrembleEffect());
        }

        OnDamageTaken?.Invoke(damageAmount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"enemy took {damageAmount} damage, current health {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator TrembleEffect()
    {
        isTrembling = true;
        float timer = 0f;

        // Get the body (sibling of hitbox)
        Transform body = transform.parent.Find("body");
        if (body == null) yield break;

        Vector3 originalBodyPos = body.localPosition;
        Debug.Log("Trembling body visual!");

        while (timer < trembleDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / trembleDuration;
            float currentIntensity = trembleIntensity * (1 - progress);

            // Shake the BODY
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-currentIntensity, currentIntensity),
                UnityEngine.Random.Range(-currentIntensity, currentIntensity),
                UnityEngine.Random.Range(-currentIntensity, currentIntensity)
            );

            body.localPosition = originalBodyPos + randomOffset;
            yield return null;
        }

        body.localPosition = originalBodyPos;
        isTrembling = false;
    }
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
    }
}
