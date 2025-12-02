using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickerSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject stickerPrefab;
    public int totalStickersToSpawn = 10;
    public LayerMask environmentLayer;

    [Header("Placement Rules")]
    public float wallOffset = 0.02f;
    public float obstacleCheckRadius = 0.4f;
    public float stickerScale = 0.5f;

    [Header("Debug")]
    public bool showDebugLines = true;

    private Transform stickerContainer;

    void Start()
    {
        // Organize hierarchy
        GameObject containerObj = new GameObject("--- SPAWNED STICKERS ---");
        stickerContainer = containerObj.transform;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Wait for Dungeon Generator
        yield return new WaitForSeconds(2.0f);
        SpawnStickers();
    }

    void SpawnStickers()
    {
        if (CollectionManager.Instance == null) return;

        // 1. --- FILTERING LOGIC ---
        // Get ALL stickers, but create a new list for only the valid ones
        List<StickerData> validStickers = new List<StickerData>();

        foreach (var sticker in CollectionManager.Instance.allStickers)
        {
            // Only add if it is marked as WorldSpawn
            if (sticker.source == StickerSource.WorldSpawn)
            {
                validStickers.Add(sticker);
            }
        }

        // Safety Check
        if (validStickers.Count == 0)
        {
            Debug.LogError("StickerSpawner: No stickers marked as 'WorldSpawn' found in CollectionManager!");
            return;
        }

        // 2. --- SPAWN LOGIC ---
        Collider[] allColliders = FindObjectsOfType<Collider>();
        List<Collider> potentialSurfaces = new List<Collider>();

        foreach (var col in allColliders)
        {
            if (((1 << col.gameObject.layer) & environmentLayer) != 0)
            {
                if (!col.isTrigger) potentialSurfaces.Add(col);
            }
        }

        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = totalStickersToSpawn * 20;

        while (spawnedCount < totalStickersToSpawn && attempts < maxAttempts)
        {
            attempts++;
            if (potentialSurfaces.Count == 0) break;

            Collider targetSurface = potentialSurfaces[UnityEngine.Random.Range(0, potentialSurfaces.Count)];

            // Pass the VALID list, not the FULL list
            if (TryPlaceSticker(targetSurface, validStickers))
            {
                spawnedCount++;
            }
        }

        Debug.Log($"StickerSpawner: Finished. Spawned {spawnedCount} stickers.");
    }

    bool TryPlaceSticker(Collider surface, List<StickerData> validData)
    {
        Vector3 randomPointInBounds = GetRandomPointInBounds(surface.bounds);
        Vector3 surfacePoint = surface.ClosestPoint(randomPointInBounds);

        Vector3 dirFromCenter = (surfacePoint - surface.bounds.center).normalized;
        if (dirFromCenter == Vector3.zero) dirFromCenter = Vector3.up;

        Vector3 rayOrigin = surfacePoint + (dirFromCenter * 0.5f);
        Vector3 rayDir = (surfacePoint - rayOrigin).normalized;

        if (showDebugLines) Debug.DrawRay(rayOrigin, rayDir, Color.yellow, 2.0f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDir, out hit, 1.5f, environmentLayer))
        {
            // Reject Floors/Ceilings
            if (Mathf.Abs(hit.normal.y) > 0.5f) return false;

            // Reject Obstacles
            Vector3 checkPos = hit.point + (hit.normal * 0.2f);
            if (Physics.CheckSphere(checkPos, obstacleCheckRadius, environmentLayer))
            {
                Collider[] overlaps = Physics.OverlapSphere(checkPos, obstacleCheckRadius, environmentLayer);
                foreach (var col in overlaps)
                {
                    if (col != surface) return false;
                }
            }

            // --- SPAWN & FORCE ENABLE ---
            Quaternion lookRot = Quaternion.LookRotation(hit.normal);
            GameObject newSticker = Instantiate(stickerPrefab, hit.point + (hit.normal * wallOffset), lookRot);

            newSticker.transform.localScale = Vector3.one * stickerScale;

            // FORCE ENABLE: Fixes the "Spawning Disabled" issue
            newSticker.SetActive(true);

            // Assign Random Data from VALID list
            StickerData data = validData[UnityEngine.Random.Range(0, validData.Count)];
            newSticker.GetComponent<WorldSticker>().Initialize(data);

            newSticker.transform.SetParent(stickerContainer);

            return true;
        }

        return false;
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}