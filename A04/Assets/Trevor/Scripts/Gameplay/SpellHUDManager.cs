using UnityEngine;

public class SpellHUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spellSlotPrefab;
    [SerializeField] private Transform slotContainer;

    void Start()
    {
        // Subscribe to the event from the PlayerSpellSystem
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnCooldownStarted += AddCooldownSlot;
        }
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        if (PlayerSpellSystem.Instance != null)
        {
            PlayerSpellSystem.Instance.OnCooldownStarted -= AddCooldownSlot;
        }
    }

    private void AddCooldownSlot(GridSpellSO spell)
    {
        // 1. Create the new UI element inside the layout group
        GameObject newSlot = Instantiate(spellSlotPrefab, slotContainer);

        // 2. Setup the icon
        SpellSlotUI uiScript = newSlot.GetComponent<SpellSlotUI>();
        if (uiScript != null)
        {
            uiScript.Setup(spell);
        }
    }
}