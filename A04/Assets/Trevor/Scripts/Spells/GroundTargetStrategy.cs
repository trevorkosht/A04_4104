using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strategies/Ground Target")]
public class GroundTargetStrategy : SpellCastStrategy
{
    [Header("Visual Settings")]
    [SerializeField] private GameObject indicatorPrefab;
    [Tooltip("How should the indicator be rotated to lie flat? (90, 0, 0) for Sprites, (0, 0, 0) for Cylinders.")]
    [SerializeField] private Vector3 indicatorRotation = new Vector3(90f, 0f, 0f);
    [Tooltip("Lift the indicator slightly so it doesn't flicker on the floor.")]
    [SerializeField] private float hoverHeight = 0.05f;

    [Header("Targeting Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxRange = 20f;

    [Header("Spell Spawn Settings")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // e.g., (0, 5, 0) for falling lights

    // Private state
    private GameObject activeIndicator;

    public override void OnSpellLoaded(SpellGridManager manager)
    {
        if (indicatorPrefab != null)
        {
            activeIndicator = Instantiate(indicatorPrefab);
            // Apply the fixed rotation immediately
            activeIndicator.transform.rotation = Quaternion.Euler(indicatorRotation);
        }
    }

    public override void OnAiming(SpellGridManager manager)
    {
        if (activeIndicator == null) return;

        Ray ray = manager.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, groundLayer))
        {
            // 1. Move to the hit point
            Vector3 finalPos = hit.point;

            // 2. Add a tiny lift so it doesn't "z-fight" (flicker) with the floor texture
            finalPos.y += hoverHeight;

            activeIndicator.transform.position = finalPos;

            // 3. STRICTLY ENFORCE ROTATION
            // We reset the rotation every frame to ensure it never tilts, even if it hits a weird bump.
            activeIndicator.transform.rotation = Quaternion.Euler(indicatorRotation);
        }
        else
        {
            // If looking at the sky, hide it or push it far away
            activeIndicator.transform.position = ray.origin + (ray.direction * maxRange);
        }
    }

    public override void Fire(SpellGridManager manager)
    {
        Vector3 targetPos;

        if (activeIndicator != null)
        {
            // We use the indicator's X and Z, but we might want the spell to start exactly at floor level,
            // so we subtract the hoverHeight we added for visuals.
            targetPos = activeIndicator.transform.position;
            targetPos.y -= hoverHeight;

            Destroy(activeIndicator);
        }
        else
        {
            targetPos = manager.PlayerCamera.transform.position;
        }

        // Calculate final spawn position (e.g. Light spell needs to be high up)
        Vector3 finalPos = targetPos + spawnOffset;

        // Spells like Light or Wind usually spawn upright (Identity)
        Instantiate(manager.LoadedSpell.castEffect, finalPos, Quaternion.identity);
    }

    public override void OnCancel(SpellGridManager manager)
    {
        if (activeIndicator != null)
        {
            Destroy(activeIndicator);
        }
    }
}