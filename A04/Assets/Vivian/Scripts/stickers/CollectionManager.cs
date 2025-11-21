using UnityEngine;
using System.Collections.Generic;
using System;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    // DRAG ALL YOUR STICKER DATA ASSETS HERE IN THE INSPECTOR
    public List<StickerData> allStickers;

    // A set of IDs the player has collected
    private HashSet<string> collectedStickerIds = new HashSet<string>();

    // Event to notify UI when something changes
    public event Action OnStickerAdded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // TODO: Load saved data here (e.g., from PlayerPrefs)
    }

    public void UnlockSticker(StickerData data)
    {
        // Only unlock if we don't have it yet
        if (!collectedStickerIds.Contains(data.id))
        {
            collectedStickerIds.Add(data.id);
            Debug.Log($"Unlocked Sticker: {data.enemyName}");

            // Notify the UI to update
            OnStickerAdded?.Invoke();

            // TODO: Save progress here
        }
    }

    public bool HasSticker(StickerData data)
    {
        return collectedStickerIds.Contains(data.id);
    }
}