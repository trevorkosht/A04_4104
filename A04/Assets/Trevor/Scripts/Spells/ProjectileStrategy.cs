using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strategies/Projectile")]
public class ProjectileStrategy : SpellCastStrategy
{
    public override void OnSpellLoaded(SpellGridManager manager)
    {
        // Projectiles usually don't need a reticle/indicator
    }

    public override void OnAiming(SpellGridManager manager)
    {
        // Optional: You could rotate the wand to face the crosshair here
    }

    public override void Fire(SpellGridManager manager)
    {
        // Spawn at camera position + forward offset
        Transform cam = manager.PlayerCamera.transform;

        Instantiate(
            manager.LoadedSpell.castEffect,
            cam.position + cam.forward * 1.5f,
            cam.rotation
        );
    }

    public override void OnCancel(SpellGridManager manager)
    {
        // Nothing to clean up
    }
}