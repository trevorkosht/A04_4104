using UnityEngine;
using System.Collections.Generic;
using System;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    [Header("Data")]
    public List<StickerData> allStickers; // The full list of possible stickers

    public HashSet<string> collectedStickerIds = new HashSet<string>();

    public event Action OnStickerAdded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UnlockSticker(StickerData data)
    {
        if (!collectedStickerIds.Contains(data.id))
        {
            // 1. Add the sticker
            collectedStickerIds.Add(data.id);
            Debug.Log($"Unlocked Sticker: {data.enemyName}");

            // 2. Update UI
            OnStickerAdded?.Invoke();

            // 3. --- WIN CONDITION CHECK ---
            CheckForWin();
        }
    }

    private void CheckForWin()
    {
        // If the number of IDs we have matches the number of items in the list...
        if (collectedStickerIds.Count >= allStickers.Count)
        {
            Debug.Log("All Stickers Collected! Triggering Win.");

            // Tell the Game Manager to end the game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerWin();
            }
        }
    }

    public bool HasSticker(StickerData data)
    {
        return collectedStickerIds.Contains(data.id);
    }
}