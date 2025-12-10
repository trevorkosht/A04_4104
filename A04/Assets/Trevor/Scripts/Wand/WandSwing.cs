using UnityEngine;

[CreateAssetMenu(fileName = "NewWandSwing", menuName = "Wand/Swing Profile")]
public class WandSwingSO : ScriptableObject
{
    [Header("Combat Stats")]
    public int damage = 20;
    public float range = 2.5f;
    public float knockback = 5f;

    [Header("Timing")]
    public float duration = 0.4f; // How long the swing takes
    public float hitWindowStart = 0.1f; // When damage detection starts (0 to 1 normalized time)
    public float hitWindowEnd = 0.6f;   // When damage detection ends

    [Header("Procedural Motion")]
    // These curves define the rotation logic for the procedural animation
    public Vector3 startRotation;
    public Vector3 endRotation;
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Optional: Add a position offset curve if you want the wand to thrust forward slightly
    public Vector3 punchOffset = new Vector3(0, 0, 0.5f);
}