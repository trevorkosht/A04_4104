using UnityEngine;

[CreateAssetMenu(fileName = "NewSticker", menuName = "Stickers/Sticker Data")]
public class StickerData : ScriptableObject
{
    [Header("Identity")]
    public string id; // Unique ID (e.g., "enemy_001")
    public string enemyName;

    [Header("Visuals")]
    public Sprite unlockedSprite; // The colorful art
    public Sprite lockedSprite;   // The silhouette or question mark (optional)

    [Header("Lore (Optional)")]
    [TextArea] public string description;
}