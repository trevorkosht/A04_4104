using UnityEngine;
using System.Collections.Generic;

public class SpellBookCooldownDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spellSlotPrefab;
    [SerializeField] private Transform slotContainer; // Drag "Self" here if this script is on the container

    // Keep track of created slots so we don't create duplicates
    private Dictionary<GridSpellSO, GameObject> createdSlots = new Dictionary<GridSpellSO, GameObject>();

    private void OnEnable()
    {
        // 1. When the book opens, Sync with the system
        RefreshUI();

        // 2. Subscribe to events (in case a cooldown finishes or starts while book is open)
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnCooldownStarted += AddSlot;
            // You might want an event for OnCooldownEnded too, to remove the slot!
        }
    }

    private void OnDisable()
    {
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnCooldownStarted -= AddSlot;
        }
    }

    private void Update()
    {
        // Optional: Check for expired cooldowns and remove their slots
        // This is a simple way to clean up slots whose cooldowns finished
        List<GridSpellSO> spellsToRemove = new List<GridSpellSO>();

        foreach (var kvp in createdSlots)
        {
            if (PlayerSpellSystem.Instance != null && !PlayerSpellSystem.Instance.IsOnCooldown(kvp.Key))
            {
                spellsToRemove.Add(kvp.Key);
            }
        }

        foreach (var spell in spellsToRemove)
        {
            RemoveSlot(spell);
        }
    }

    private void RefreshUI()
    {
        // Clear existing to be safe
        foreach (Transform child in slotContainer) Destroy(child.gameObject);
        createdSlots.Clear();

        if (PlayerSpellSystem.Instance == null) return;

        // Get all currently active cooldowns from the system
        List<GridSpellSO> activeSpells = PlayerSpellSystem.Instance.GetActiveCooldowns();

        foreach (var spell in activeSpells)
        {
            AddSlot(spell);
        }
    }

    private void AddSlot(GridSpellSO spell)
    {
        // Don't create duplicates
        if (createdSlots.ContainsKey(spell)) return;

        GameObject newSlot = Instantiate(spellSlotPrefab, slotContainer);

        SpellSlotUI uiScript = newSlot.GetComponent<SpellSlotUI>();
        if (uiScript != null)
        {
            uiScript.Setup(spell);
        }

        createdSlots.Add(spell, newSlot);
    }

    private void RemoveSlot(GridSpellSO spell)
    {
        if (createdSlots.ContainsKey(spell))
        {
            Destroy(createdSlots[spell]);
            createdSlots.Remove(spell);
        }
    }
}