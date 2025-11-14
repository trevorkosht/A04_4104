using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpellDatabase", menuName = "Spells/Database")]
public class SpellDatabase : ScriptableObject
{
    public List<GridSpellSO> gridSpells;
}