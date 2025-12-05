using UnityEngine;

[CreateAssetMenu(fileName = "FlashData", menuName = "Enemy/Flash Indicator Data")]
public class FlashIndicatorData : ScriptableObject
{
    [Header("Size Settings")]
    public float width;
    public float length;
}
