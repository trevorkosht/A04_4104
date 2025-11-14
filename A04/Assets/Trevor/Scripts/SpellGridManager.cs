using System.Collections.Generic;
using UnityEngine;

public class SpellGridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridVisualizer gridVisualizer;
    [SerializeField] private SpellDatabase spellDatabase;
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float gridDistance = 3f;
    [SerializeField] private float minPatternLength = 3;

    private List<GridCell> currentPath = new List<GridCell>();
    private GridCell lastCell;
    private bool gridActive = false;

    private void Update()
    {
        HandleGridInput();
    }

    private void HandleGridInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleGrid();
        }

        if (gridActive && Input.GetMouseButton(0))
        {
            ProcessDrawing();
        }

        if (Input.GetMouseButtonUp(0) && currentPath.Count > 0)
        {
            CheckForSpellMatch();
        }
    }

    private void ToggleGrid()
    {
        gridActive = !gridActive;

        if (gridActive)
        {
            Vector3 gridPosition = playerCamera.transform.position +
                                 playerCamera.transform.forward * gridDistance;

            // Lock y-rotation to 0 while maintaining proper forward facing
            Vector3 lookDirection = playerCamera.transform.forward;
            lookDirection.y = 0;
            Quaternion gridRotation = Quaternion.LookRotation(lookDirection.normalized);

            gridVisualizer.CreateGrid(gridPosition, gridRotation);
            currentPath.Clear();
        }
        else
        {
            gridVisualizer.DestroyGrid();
            ClearHighlights();
        }
    }

    private void ProcessDrawing()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (gridVisualizer.TryGetCellFromRay(ray, out GridCell currentCell))
        {
            if (currentPath.Count == 0 || currentCell != lastCell)
            {
                currentPath.Add(currentCell);
                gridVisualizer.HighlightCell(currentCell, true);
                lastCell = currentCell;
            }
        }
    }

    private void CheckForSpellMatch()
    {
        if (currentPath.Count < minPatternLength) return;

        string currentPattern = string.Join("-", currentPath);
        foreach (GridSpellSO spell in spellDatabase.gridSpells)
        {
            if (currentPattern.Contains(string.Join("-", spell.pattern)))
            {
                CastSpell(spell);
                break;
            }
        }
        ClearHighlights();
    }

    private void CastSpell(GridSpellSO spell)
    {
        Instantiate(
            spell.castEffect,
            playerCamera.transform.position + playerCamera.transform.forward * 2f,
            playerCamera.transform.rotation
        );
        gridVisualizer.DestroyGrid();
        gridActive = false;
    }

    private void ClearHighlights()
    {
        foreach (GridCell cell in currentPath)
        {
            gridVisualizer.HighlightCell(cell, false);
        }
        currentPath.Clear();
    }
}