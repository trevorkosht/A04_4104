using UnityEngine;

public enum StickerSource
{
    EnemyDrop,
    WorldSpawn,
    BossReward
}

[CreateAssetMenu(fileName = "NewSticker", menuName = "Stickers/Sticker Data")]
public class StickerData : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string enemyName; // Acts as "Display Name"

    [Header("Categorization")]
    public StickerSource source; // <--- NEW: Tells the spawner what is allowed

    [Header("Visuals")]
    public Sprite unlockedSprite;
    public Sprite lockedSprite;

    [Header("Stats")]
    public int rarity;

    [Header("Lore")]
    [TextArea] public string description;
}