using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strategies/Ground Target")]
public class GroundTargetStrategy : SpellCastStrategy
{
    [Header("Visual Settings")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Vector3 indicatorRotation = new Vector3(90f, 0f, 0f);
    [SerializeField] private float hoverHeight = 0.05f;

    [Header("Targeting Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxRange = 20f;

    [Header("Spell Spawn Settings")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    private GameObject activeIndicator;

    public override void OnSpellLoaded(SpellGridManager manager)
    {
        if (indicatorPrefab != null)
        {
            activeIndicator = Instantiate(indicatorPrefab);
            activeIndicator.transform.rotation = Quaternion.Euler(indicatorRotation);
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
            activeIndicator.transform.rotation = Quaternion.Euler(indicatorRotation);
        }
        else
        {
            activeIndicator.transform.position = ray.origin + (ray.direction * maxRange);
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

        // 1. Instantiate
        GameObject spellObj = Instantiate(manager.LoadedSpell.castEffect, finalPos, Quaternion.identity);

        // 2. INJECT DATA
        SpellController controller = spellObj.GetComponent<SpellController>();
        if (controller != null)
        {
            controller.Initialize(manager.LoadedSpell);
        }
        else
        {
            Debug.LogWarning($"Prefab for {manager.LoadedSpell.name} is missing a SpellController script!");
        }
    }

    public override void OnCancel(SpellGridManager manager)
    {
        if (activeIndicator != null) Destroy(activeIndicator);
    }
}