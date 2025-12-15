using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Needed for shuffling

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
        GameObject containerObj = new GameObject("--- SPAWNED STICKERS ---");
        stickerContainer = containerObj.transform;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        SpawnStickers();
    }

    void SpawnStickers()
    {
        if (CollectionManager.Instance == null) return;

        // 1. --- FILTERING ---
        List<StickerData> validStickers = new List<StickerData>();
        foreach (var sticker in CollectionManager.Instance.allStickers)
        {
            if (sticker.source == StickerSource.WorldSpawn)
            {
                validStickers.Add(sticker);
            }
        }

        if (validStickers.Count == 0)
        {
            Debug.LogError("StickerSpawner: No WorldSpawn stickers found!");
            return;
        }

        // 2. --- BUILD THE "DECK" (Guaranteed Coverage) ---
        List<StickerData> spawnQueue = new List<StickerData>();

        // Step A: Add one of EVERY valid type to ensure representation
        foreach (var s in validStickers)
        {
            spawnQueue.Add(s);
        }

        // Step B: If we need more stickers than we have types, fill the rest randomly
        while (spawnQueue.Count < totalStickersToSpawn)
        {
            spawnQueue.Add(validStickers[UnityEngine.Random.Range(0, validStickers.Count)]);
        }

        // Step C: If we have MORE types than spawn slots, we trim the list (or you could increase totalStickersToSpawn)
        if (spawnQueue.Count > totalStickersToSpawn)
        {
            // Optional warning
            Debug.LogWarning($"You have {validStickers.Count} types but only {totalStickersToSpawn} slots. Some stickers won't spawn.");
            spawnQueue = spawnQueue.GetRange(0, totalStickersToSpawn);
        }

        // Step D: Shuffle the queue so the required ones aren't always first
        ShuffleList(spawnQueue);

        // 3. --- SPAWN LOGIC ---
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

        // We use a separate index to track which sticker from our queue we are trying to place
        int queueIndex = 0;

        while (queueIndex < spawnQueue.Count && attempts < maxAttempts)
        {
            attempts++;
            if (potentialSurfaces.Count == 0) break;

            Collider targetSurface = potentialSurfaces[UnityEngine.Random.Range(0, potentialSurfaces.Count)];

            // Try to place the SPECIFIC sticker at the current queue index
            if (TryPlaceSticker(targetSurface, spawnQueue[queueIndex]))
            {
                spawnedCount++;
                queueIndex++; // Only move to the next sticker type if we successfully placed this one
            }
        }

        Debug.Log($"StickerSpawner: Finished. Spawned {spawnedCount} stickers. (Target was {totalStickersToSpawn})");
    }

    // Changed signature: passing specific StickerData instead of a list
    bool TryPlaceSticker(Collider surface, StickerData specificSticker)
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
            if (Mathf.Abs(hit.normal.y) > 0.5f) return false;

            Vector3 checkPos = hit.point + (hit.normal * 0.2f);
            if (Physics.CheckSphere(checkPos, obstacleCheckRadius, environmentLayer))
            {
                Collider[] overlaps = Physics.OverlapSphere(checkPos, obstacleCheckRadius, environmentLayer);
                foreach (var col in overlaps)
                {
                    if (col != surface) return false;
                }
            }

            // --- SPAWN ---
            Quaternion lookRot = Quaternion.LookRotation(hit.normal);
            GameObject newSticker = Instantiate(stickerPrefab, hit.point + (hit.normal * wallOffset), lookRot);

            newSticker.transform.localScale = Vector3.one * stickerScale;
            newSticker.SetActive(true);

            // Use the specific data passed in
            newSticker.GetComponent<WorldSticker>().Initialize(specificSticker);

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

    // Simple Fisher-Yates shuffle
    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}