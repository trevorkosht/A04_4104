using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GridSpellSO))]
public class GridSpellSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector for everything except the pattern
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "pattern");

        GridSpellSO spell = (GridSpellSO)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spell Pattern", EditorStyles.boldLabel);

        // Define the 3x3 Grid
        GridCell[] gridPositions = {
            GridCell.TopLeft, GridCell.TopCenter, GridCell.TopRight,
            GridCell.MidLeft, GridCell.Center, GridCell.MidRight,
            GridCell.BottomLeft, GridCell.BottomCenter, GridCell.BottomRight
        };

        // Draw the 3x3 Grid of Toggles
        EditorGUILayout.BeginVertical("box");
        for (int r = 0; r < 3; r++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < 3; c++)
            {
                int index = r * 3 + c;
                GridCell cell = gridPositions[index];

                bool isSelected = spell.pattern.Contains(cell);
                bool newSelected = GUILayout.Toggle(isSelected, "", "Button", GUILayout.Width(40), GUILayout.Height(40));

                if (newSelected != isSelected)
                {
                    Undo.RecordObject(spell, "Toggle Pattern Cell");
                    if (newSelected) spell.pattern.Add(cell);
                    else spell.pattern.Remove(cell);
                    EditorUtility.SetDirty(spell);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}