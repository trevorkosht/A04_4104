using UnityEngine;
using UnityEngine.UI;

public class SpellSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image spellIcon;
    [SerializeField] private Image cooldownOverlay;

    private GridSpellSO mySpell;

    public void Setup(GridSpellSO spell)
    {
        mySpell = spell;

        if (spellIcon != null)
        {
            spellIcon.sprite = spell.spellIcon;
            spellIcon.preserveAspect = true;
        }

        // Start full gray
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1f;
        }
    }

    void Update()
    {
        if (mySpell == null) return;

        // Check with the system
        if (PlayerSpellSystem.Instance != null)
        {
            // 1. If the spell is NO LONGER on cooldown, destroy this UI slot
            if (!PlayerSpellSystem.Instance.IsOnCooldown(mySpell))
            {
                Destroy(gameObject);
                return;
            }

            // 2. Otherwise, update the spiral visual
            if (cooldownOverlay != null)
            {
                float progress = PlayerSpellSystem.Instance.GetCooldownProgress(mySpell);
                cooldownOverlay.fillAmount = progress;
            }
        }
    }
}