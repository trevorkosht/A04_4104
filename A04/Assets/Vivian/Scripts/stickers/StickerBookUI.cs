using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StickerBookUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform gridContainer; // The Grid Layout Group parent
    public GameObject slotPrefab;   // Prefab containing an Image component

    // Dictionary to map IDs to UI Images for fast updates
    private Dictionary<string, Image> slotMap = new Dictionary<string, Image>();

    void Start()
    {
        // Initialize the book when the game starts
        InitializeBook();

        // Subscribe to the event so the UI updates automatically
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.OnStickerAdded += RefreshUI;
        }
    }

    void InitializeBook()
    {
        // 1. Clear any existing dummy slots
        foreach (Transform child in gridContainer) Destroy(child.gameObject);
        slotMap.Clear();

        // 2. Create a slot for every sticker that exists in the game
        if (CollectionManager.Instance != null)
        {
            foreach (var sticker in CollectionManager.Instance.allStickers)
            {
                GameObject newSlot = Instantiate(slotPrefab, gridContainer);

                // If your prefab has a script, get that. Otherwise get the Image.
                Image slotImage = newSlot.GetComponent<Image>();

                if (slotImage != null)
                {
                    slotMap.Add(sticker.id, slotImage);
                }
            }
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        if (CollectionManager.Instance == null) return;

        foreach (var sticker in CollectionManager.Instance.allStickers)
        {
            if (slotMap.ContainsKey(sticker.id))
            {
                Image uiImage = slotMap[sticker.id];

                if (CollectionManager.Instance.HasSticker(sticker))
                {
                    // UNLOCKED: Show full color sprite
                    uiImage.sprite = sticker.unlockedSprite;
                    uiImage.color = Color.white;
                }
                else
                {
                    // LOCKED: Show silhouette or locked sprite
                    if (sticker.lockedSprite != null)
                    {
                        uiImage.sprite = sticker.lockedSprite;
                        uiImage.color = Color.white;
                    }
                    else
                    {
                        // Fallback: Use the normal sprite but tint it black
                        uiImage.sprite = sticker.unlockedSprite;
                        uiImage.color = Color.black;
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent errors when changing scenes
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.OnStickerAdded -= RefreshUI;
        }
    }
}