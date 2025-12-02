using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Grid Spell")]
public class GridSpellSO : ScriptableObject
{
    [Header("Identity")]
    public string spellName;
    public Sprite spellIcon;
    public GameObject castEffect; // The Prefab
    public SpellCastStrategy castStrategy;

    [Header("Resources")]
    public int manaCost = 10;
    public float cooldownTime = 5f;

    [Header("Universal Stats")]
    [Tooltip("Damage for attacks, Healing amount for heals.")]
    public int power = 10;

    [Tooltip("Damage Over Time (Burn/Poison) amount per tick.")]
    public int dotDamage = 3;

    [Tooltip("How long the object lasts (0 for instant/infinite).")]
    public float duration = 3f;

    [Tooltip("For DOTs/HOTs: how often it ticks (seconds).")]
    public float tickRate = 1f;

    [Tooltip("How many projectiles to fire (Default is 1).")]
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