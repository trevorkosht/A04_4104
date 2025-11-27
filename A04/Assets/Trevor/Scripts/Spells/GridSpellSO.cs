using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGridSpell", menuName = "Spells/Grid Spell")]
public class GridSpellSO : ScriptableObject
{
    [Header("Identity")]
    public string spellName;
    public Sprite spellIcon;
    public GameObject castEffect; // The projectile prefab
    public SpellCastStrategy castStrategy; // Keep your existing strategy reference

    [Header("Stats")]
    public int manaCost = 10;
    public float cooldownTime = 5f;
    public int damage = 10;

    [Header("Pattern (Draw in Inspector)")]
    public List<GridCell> pattern = new List<GridCell>();
}

public enum GridCell
{
    TopLeft, TopCenter, TopRight,
    MidLeft, Center, MidRight,
    BottomLeft, BottomCenter, BottomRight
}