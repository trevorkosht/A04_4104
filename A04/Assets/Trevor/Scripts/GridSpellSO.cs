using UnityEngine;

[CreateAssetMenu(fileName = "NewGridSpell", menuName = "Spells/Grid Spell")]
public class GridSpellSO : ScriptableObject
{
    public string spellName;
    public GridCell[] pattern;
    public GameObject castEffect;

    public Sprite spellIcon;
}

public enum GridCell
{
    TopLeft, TopCenter, TopRight,
    MidLeft, Center, MidRight,
    BottomLeft, BottomCenter, BottomRight
}