using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Grid Spell")]
public class GridSpellSO : ScriptableObject
{
    [Header("Identity")]
    public string spellName;
    public Sprite spellIcon;
    public GameObject castEffect;

    [Tooltip("Optional: If the strategy requires a target indicator (e.g. Ground Target), assign it here.")]
    public GameObject targetIndicator; // NEW FIELD
    [Tooltip("Rotation offset for the indicator. Use (90,0,0) for flat sprites, (0,0,0) for 3D meshes.")]
    public Vector3 indicatorRotation = new Vector3(90f, 0f, 0f);

    public SpellCastStrategy castStrategy;

    [Header("Audio")]
    public AudioClip castSound;
    public AudioClip impactSound;

    // ... (Keep the rest of your existing code below) ...
    [Header("Resources")]
    public int manaCost = 10;
    public float cooldownTime = 5f;

    [Header("Universal Stats")]
    public int power = 10;
    public int dotDamage = 3;
    public float duration = 3f;
    public float tickRate = 1f;
    public int quantity = 1;

    [Header("Pattern")]
    public List<GridCell> pattern = new List<GridCell>();
}

public enum GridCell
{
    TopLeft, TopCenter, TopRight,
    MidLeft, Center, MidRight,
    BottomLeft, BottomCenter, BottomRight
}