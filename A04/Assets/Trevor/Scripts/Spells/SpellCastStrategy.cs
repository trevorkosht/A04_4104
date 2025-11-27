using UnityEngine;

public abstract class SpellCastStrategy : ScriptableObject
{
    // Called when the pattern is matched and spell is loaded
    public abstract void OnSpellLoaded(SpellGridManager manager);

    // Called every frame while the spell is waiting to be fired
    public abstract void OnAiming(SpellGridManager manager);

    // Called when the player clicks
    public abstract void Fire(SpellGridManager manager);

    // Called if the player presses 'E' to cancel
    public abstract void OnCancel(SpellGridManager manager);
}