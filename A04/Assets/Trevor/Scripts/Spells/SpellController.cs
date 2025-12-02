using UnityEngine;

public abstract class SpellController : MonoBehaviour
{
    protected GridSpellSO spellData;

    public virtual void Initialize(GridSpellSO data)
    {
        this.spellData = data;

        // Automatic cleanup based on SO duration
        if (spellData.duration > 0)
        {
            Destroy(gameObject, spellData.duration);
        }
    }
}