using UnityEngine;
using System; // Required for Actions

public abstract class SpellController : MonoBehaviour
{
    protected GridSpellSO spellData;

    // --- NEW STATIC EVENT ---
    // This allows any spell to "shout" to the Audio Manager without connecting them manually
    public static event Action<AudioClip, Vector3> OnSpellImpact;

    public virtual void Initialize(GridSpellSO data)
    {
        this.spellData = data;

        // Automatic cleanup based on SO duration
        if (spellData.duration > 0)
        {
            Destroy(gameObject, spellData.duration);
        }
    }

    // --- NEW HELPER METHOD ---
    // Child classes (Fireball, MagicMissile) will call this when they hit something
    protected void PlayImpactSound(Vector3 position)
    {
        // We ensure we have data and a sound clip before shouting
        if (spellData != null && spellData.impactSound != null)
        {
            OnSpellImpact?.Invoke(spellData.impactSound, position);
        }
    }
}