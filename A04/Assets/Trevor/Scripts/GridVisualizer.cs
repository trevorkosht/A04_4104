using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gridDistance = 2f;
    [SerializeField] private float cellSize = 0.3f;
    [SerializeField] private float cellSpacing = 0.05f;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial; 

    private GameObject[,] gridCells = new GameObject[3, 3];

    public void CreateGrid(Vector3 position, Quaternion rotation)
    {
        DestroyGrid();
        transform.position = position;
        transform.rotation = Quaternion.identity; // Parent has no rotation

        float totalWidth = (cellSize * 3) + (cellSpacing * 2);
        float startX = -totalWidth / 2 + cellSize / 2;
        float startY = totalWidth / 2 - cellSize / 2;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.SetParent(transform);

                // Position cells with proper spacing
                cell.transform.localPosition = new Vector3(
                    startX + (x * (cellSize + cellSpacing)),
                    startY - (y * (cellSize + cellSpacing)),
                    gridDistance
                );

                // Set all rotations to zero
                cell.transform.localRotation = Quaternion.identity;
                cell.transform.localScale = Vector3.one * cellSize;
                cell.layer = LayerMask.NameToLayer("SpellGrid");
                cell.GetComponent<Renderer>().material = defaultMaterial;

                gridCells[x, y] = cell;
            }
        }

        // Rotate the entire grid at once
        transform.rotation = rotation;
    }

    public void DestroyGrid()
    {
        foreach (GameObject cell in gridCells)
        {
            if (cell != null) Destroy(cell);
        }
        gridCells = new GameObject[3, 3];
    }

    public void HighlightCell(GridCell cell, bool highlight)
    {
        int x = (int)cell % 3;
        int y = (int)cell / 3;
        if (x >= 0 && x < 3 && y >= 0 && y < 3 && gridCells[x, y] != null)
        {
            gridCells[x, y].GetComponent<Renderer>().material =
                highlight ? highlightMaterial : defaultMaterial;
        }
    }

    public bool TryGetCellFromRay(Ray ray, out GridCell cell)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("SpellGrid")))
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (gridCells[x, y] != null && hit.collider.gameObject == gridCells[x, y])
                    {
                        cell = (GridCell)(y * 3 + x);
                        return true;
                    }
                }
            }
        }
        cell = GridCell.Center;
        return false;
    }
}