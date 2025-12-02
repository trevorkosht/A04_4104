using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strategies/Formation Projectile")]
public class FormationProjectileStrategy : SpellCastStrategy
{
    [Header("Formation Settings")]
    [Tooltip("How wide the formation is (Shoulder to Shoulder distance).")]
    [SerializeField] private float formationWidth = 1.5f;

    [Tooltip("How high the center projectile is relative to the sides (Head height).")]
    [SerializeField] private float archHeight = 0.5f;

    [Header("Launch Physics")]
    [Tooltip("Force projectiles to fly slightly UP initially, even if looking down.")]
    [SerializeField] private float minLaunchPitch = 15f;

    // We don't need these for this strategy
    public override void OnSpellLoaded(SpellGridManager manager) { }
    public override void OnAiming(SpellGridManager manager) { }
    public override void OnCancel(SpellGridManager manager) { }

    public override void Fire(SpellGridManager manager)
    {
        Transform cam = manager.PlayerCamera.transform;
        int projectileCount = manager.LoadedSpell.quantity;

        // Safety check
        if (projectileCount <= 0) projectileCount = 1;

        for (int i = 0; i < projectileCount; i++)
        {
            // 1. Calculate "t" value from -1 (Left) to +1 (Right)
            float t = 0f;
            if (projectileCount > 1)
            {
                // Map current index to range -1 to 1
                t = -1f + (2f * i / (projectileCount - 1f));
            }

            // 2. Position Logic (The Arc)
            // Horizontal: spread out based on camera's Right vector
            Vector3 horizontalOffset = cam.right * (t * formationWidth / 2f);

            // Vertical: Parabola formula (1 - t^2)
            // Center (t=0) is highest. Sides (t=1 or -1) are lowest.
            float curveHeight = 1f - (t * t);
            Vector3 verticalOffset = cam.up * (curveHeight * archHeight);

            // Final Spawn Position (Spawn slightly forward to avoid clipping player)
            Vector3 spawnPos = cam.position + horizontalOffset + verticalOffset + (cam.forward * 0.5f);

            // 3. Rotation Logic (The "Upward Launch")
            Vector3 launchDirection = cam.forward;

            // If looking steeply down, flatten the launch vector so they don't shoot into feet
            if (launchDirection.y < 0) launchDirection.y = 0;

            Quaternion lookRot = Quaternion.LookRotation(launchDirection);

            // Apply upward pitch
            Quaternion pitchRot = Quaternion.Euler(-minLaunchPitch, 0, 0);
            Quaternion finalRotation = lookRot * pitchRot;

            // 4. Instantiate & Initialize
            GameObject spellObj = Instantiate(manager.LoadedSpell.castEffect, spawnPos, finalRotation);

            SpellController controller = spellObj.GetComponent<SpellController>();
            if (controller != null)
            {
                controller.Initialize(manager.LoadedSpell);
            }
        }
    }
}