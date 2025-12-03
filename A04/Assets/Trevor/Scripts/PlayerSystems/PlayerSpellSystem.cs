using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerSpellSystem : MonoBehaviour
{
    public static PlayerSpellSystem Instance { get; private set; }

    [Header("Mana Settings")]
    public float currentMana = 100f;
    public float maxMana = 100f;
    public float manaRegenRate = 5f;

    // Dictionary to track cooldowns
    private Dictionary<GridSpellSO, float> activeCooldowns = new Dictionary<GridSpellSO, float>();

    // Events
    public event Action<float, float> OnManaChanged;
    public event Action<GridSpellSO> OnCooldownStarted;

    // NEW: Event for when a cast fails due to low mana
    public event Action OnManaCheckFailed;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
    }

    public bool CanCast(GridSpellSO spell)
    {
        // Check Mana
        if (currentMana < spell.manaCost)
        {
            // FIRE THE EVENT HERE
            OnManaCheckFailed?.Invoke();
            return false;
        }

        // Check Cooldown
        if (IsOnCooldown(spell)) return false;

        return true;
    }

    public void CastSpell(GridSpellSO spell)
    {
        currentMana -= spell.manaCost;
        OnManaChanged?.Invoke(currentMana, maxMana);

        float readyTime = Time.time + spell.cooldownTime;
        if (activeCooldowns.ContainsKey(spell)) activeCooldowns[spell] = readyTime;
        else activeCooldowns.Add(spell, readyTime);

        OnCooldownStarted?.Invoke(spell);
    }

    public bool IsOnCooldown(GridSpellSO spell)
    {
        if (!activeCooldowns.ContainsKey(spell)) return false;
        return activeCooldowns[spell] > Time.time;
    }

    public float GetCooldownProgress(GridSpellSO spell)
    {
        if (!activeCooldowns.ContainsKey(spell)) return 0f;

        float readyTime = activeCooldowns[spell];
        float totalDuration = spell.cooldownTime;
        float remaining = readyTime - Time.time;

        return Mathf.Clamp01(remaining / totalDuration);
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    public void ReduceAllCooldowns(float seconds)
    {
        List<GridSpellSO> keys = new List<GridSpellSO>(activeCooldowns.Keys);
        foreach (var key in keys)
        {
            activeCooldowns[key] -= seconds;
        }
    }
}