using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [Header("References")]
    // Drag your 9 cell GameObjects here in order (0-8)
    [SerializeField] private GameObject[] gridCells = new GameObject[9];

    [Header("Settings")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    // This method is called by the manager when instantiated
    public void ShowGrid(bool show)
    {
        // This is useful if your prefab root starts disabled
        gameObject.SetActive(show);
    }

    // Applies the highlight or default material to a cell
    public void HighlightCell(GridCell cell, bool highlight)
    {
        int cellIndex = (int)cell;
        if (cellIndex >= 0 && cellIndex < gridCells.Length && gridCells[cellIndex] != null)
        {
            gridCells[cellIndex].GetComponent<Renderer>().material =
                highlight ? highlightMaterial : defaultMaterial;
        }
    }

    // Checks if the ray hits one of our 9 cells
    public bool TryGetCellFromRay(Ray ray, out GridCell cell)
    {
        // Make sure you have your cells on the "SpellGrid" layer
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("SpellGrid")))
        {
            // Loop through our known cells and see if the raycast hit one
            for (int i = 0; i < gridCells.Length; i++)
            {
                if (gridCells[i] != null && hit.collider.gameObject == gridCells[i])
                {
                    // If it hit, cast the index 'i' to our GridCell enum
                    cell = (GridCell)i;
                    return true;
                }
            }
        }

        cell = GridCell.Center; // Default fallback
        return false;
    }
}