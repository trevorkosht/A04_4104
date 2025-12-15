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

    private Dictionary<GridSpellSO, float> activeCooldowns = new Dictionary<GridSpellSO, float>();

    // Events
    public event Action<float, float> OnManaChanged;
    public event Action<GridSpellSO> OnCooldownStarted;

    // FAIL EVENTS
    public event Action OnManaCheckFailed;
    public event Action OnCooldownCheckFailed;

    public event Action<GridSpellSO> OnSpellCast;
    public event Action OnManaRestored;

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
        // 1. Check Mana
        if (currentMana < spell.manaCost)
        {
            OnManaCheckFailed?.Invoke();
            return false;
        }

        // 2. Check Cooldown
        if (IsOnCooldown(spell))
        {
            OnCooldownCheckFailed?.Invoke();
            return false;
        }

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
        OnSpellCast?.Invoke(spell);
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
        OnManaRestored?.Invoke();
    }

    public void ReduceAllCooldowns(float seconds)
    {
        List<GridSpellSO> keys = new List<GridSpellSO>(activeCooldowns.Keys);
        foreach (var key in keys)
        {
            activeCooldowns[key] -= seconds;
        }
    }

    public List<GridSpellSO> GetActiveCooldowns()
    {
        return new List<GridSpellSO>(activeCooldowns.Keys);
    }

    // --- NEW HELPER METHOD ---
    // Allows the SpellGridManager to trigger the "Cooldown Buzz" sound manually
    public void TriggerCooldownFail()
    {
        OnCooldownCheckFailed?.Invoke();
    }
}