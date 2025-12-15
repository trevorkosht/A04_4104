using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strategies/Ground Target")]
public class GroundTargetStrategy : SpellCastStrategy
{
    [Header("Defaults (Fallback)")]
    [Tooltip("Used ONLY if the Spell Data has no indicator assigned.")]
    [SerializeField] private GameObject defaultIndicatorPrefab;
    [SerializeField] private Vector3 defaultRotation = new Vector3(90f, 0f, 0f);

    [Header("Settings")]
    [SerializeField] private float hoverHeight = 0.05f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    private GameObject activeIndicator;
    private Vector3 currentRotation; // Store the rotation we decided to use

    public override void OnSpellLoaded(SpellGridManager manager)
    {
        // 1. Determine which Prefab to use
        GameObject prefabToUse = manager.LoadedSpell.targetIndicator;

        // 2. Determine which Rotation to use
        if (prefabToUse != null)
        {
            // If the spell has its own indicator, use the spell's preferred rotation
            currentRotation = manager.LoadedSpell.indicatorRotation;
            Debug.Log($"Using Specific Indicator: {prefabToUse.name}");
        }
        else
        {
            // Fallback to strategy defaults
            prefabToUse = defaultIndicatorPrefab;
            currentRotation = defaultRotation;
            Debug.Log($"Using Default Indicator: {defaultIndicatorPrefab?.name}");
        }

        // 3. Instantiate
        if (prefabToUse != null)
        {
            activeIndicator = Instantiate(prefabToUse);
            activeIndicator.transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    public override void OnAiming(SpellGridManager manager)
    {
        if (activeIndicator == null) return;

        Ray ray = manager.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, groundLayer))
        {
            Vector3 finalPos = hit.point;
            finalPos.y += hoverHeight;
            activeIndicator.transform.position = finalPos;

            // KEY FIX: Use the 'currentRotation' variable we set in OnSpellLoaded
            // This prevents the strategy from overwriting the spell's rotation preference
            activeIndicator.transform.rotation = Quaternion.Euler(currentRotation);
        }
        else
        {
            activeIndicator.transform.position = ray.origin + (ray.direction * maxRange);
            activeIndicator.transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    public override void Fire(SpellGridManager manager)
    {
        Vector3 targetPos;

        if (activeIndicator != null)
        {
            targetPos = activeIndicator.transform.position;
            targetPos.y -= hoverHeight;
            Destroy(activeIndicator);
        }
        else
        {
            targetPos = manager.PlayerCamera.transform.position;
        }

        Vector3 finalPos = targetPos + spawnOffset;

        if (manager.LoadedSpell.castEffect != null)
        {
            GameObject spellObj = Instantiate(manager.LoadedSpell.castEffect, finalPos, Quaternion.identity);

            SpellController controller = spellObj.GetComponent<SpellController>();
            if (controller != null)
            {
                controller.Initialize(manager.LoadedSpell);
            }
        }
    }

    public override void OnCancel(SpellGridManager manager)
    {
        if (activeIndicator != null) Destroy(activeIndicator);
    }
}