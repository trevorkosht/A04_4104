using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strategies/Projectile")]
public class ProjectileStrategy : SpellCastStrategy
{
    public override void OnSpellLoaded(SpellGridManager manager) { }
    public override void OnAiming(SpellGridManager manager) { }
    public override void OnCancel(SpellGridManager manager) { }

    public override void Fire(SpellGridManager manager)
    {
        Transform cam = manager.PlayerCamera.transform;

        // Simple forward spawn
        GameObject spellObj = Instantiate(
            manager.LoadedSpell.castEffect,
            cam.position + cam.forward * 1.5f,
            cam.rotation
        );

        SpellController controller = spellObj.GetComponent<SpellController>();
        if (controller != null)
        {
            controller.Initialize(manager.LoadedSpell);
        }
    }
}