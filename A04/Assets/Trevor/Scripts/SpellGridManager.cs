using System.Collections.Generic;
using UnityEngine;

public class SpellGridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spellGridPrefab; // Your spellbook prefab
    [SerializeField] private SpellDatabase spellDatabase;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private WandFeedback wandFeedback; // The script on your wand

    [Header("Settings")]
    [SerializeField] private float gridDistance = 2f;
    [SerializeField] private float gridYOffset = -0.5f;
    [SerializeField] private float minPatternLength = 3;

    [Header("Collision")]
    [SerializeField] private float spawnBuffer = 0.1f; // Pushes the book off the wall
    [SerializeField] private LayerMask collisionLayers; // What layers to check for (walls, floor, etc.)

    // --- Private Fields ---
    private List<GridCell> currentPath = new List<GridCell>();
    private GridCell lastCell;
    private bool gridActive = false;

    private GridVisualizer activeGridInstance;
    private GridSpellSO loadedSpell = null; // The spell that's ready to fire

    private void Update()
    {
        HandleGridInput();
    }

    private void HandleGridInput()
    {
        // Toggle the grid on/off or cancel a loaded spell
        if (Input.GetKeyDown(KeyCode.E))
        {
            // If we have a spell loaded, pressing 'E' cancels it
            if (loadedSpell != null)
            {
                loadedSpell = null;
                if (wandFeedback != null) wandFeedback.HideSpellIcon();
                Debug.Log("Spell Canceled");
            }
            // Otherwise, toggle the grid
            else
            {
                ToggleGrid();
            }
        }

        // --- FIRING LOGIC ---
        // 1. Check for Firing (if grid is NOT active and a spell IS loaded)
        if (Input.GetMouseButtonDown(0) && !gridActive && loadedSpell != null)
        {
            FireLoadedSpell();
        }
        // 2. Check for Drawing (if grid IS active)
        else if (gridActive && activeGridInstance != null && Input.GetMouseButton(0))
        {
            ProcessDrawing();
        }

        // 3. Check for Finishing a Drawing (Mouse Up)
        if (Input.GetMouseButtonUp(0) && currentPath.Count > 0)
        {
            // This now "loads" the spell instead of casting
            CheckForSpellMatch();

            // Always close the grid after finishing a drawing
            if (gridActive)
            {
                CloseGrid();
            }
        }
    }

    private void ToggleGrid()
    {
        // Note: The loadedSpell check was moved to HandleGridInput
        gridActive = !gridActive;

        if (gridActive)
        {
            // Only spawn if one isn't already active
            if (activeGridInstance == null)
            {
                OpenGrid();
            }
        }
        else
        {
            CloseGrid();
        }
    }

    private void OpenGrid()
    {
        Transform camTransform = playerCamera.transform;

        // --- Y-Stable Position Logic ---
        Vector3 camPos = camTransform.position;

        // Get the camera's forward direction but ignore its Y component
        Vector3 horizontalForward = camTransform.forward;
        horizontalForward.y = 0;
        horizontalForward.Normalize();

        // Fallback in case player is looking straight up or down
        if (horizontalForward == Vector3.zero)
        {
            horizontalForward = transform.forward; // Assumes this script is on the player
            horizontalForward.y = 0;
            horizontalForward.Normalize();

            if (horizontalForward == Vector3.zero)
            {
                horizontalForward = Vector3.forward;
            }
        }

        // Calculate the XZ position
        Vector3 idealSpawnPos = camPos + (horizontalForward * gridDistance);
        // Set the Y position explicitly based on camera's Y + offset
        idealSpawnPos.y = camPos.y + gridYOffset;

        // --- Collision Check Logic ---
        Vector3 rayStartPosition = camPos + (camTransform.forward * 0.1f);
        Vector3 rayDirection = idealSpawnPos - rayStartPosition;
        float rayDistance = rayDirection.magnitude;
        Ray ray = new Ray(rayStartPosition, rayDirection.normalized);

        Vector3 finalSpawnPos;
        Quaternion finalSpawnRot;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, collisionLayers))
        {
            // --- WE HIT A WALL/FLOOR ---
            finalSpawnPos = hit.point + (hit.normal * spawnBuffer);
            // This makes it lie flat on the surface, facing out
            finalSpawnRot = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            // --- PATH IS CLEAR ---
            finalSpawnPos = idealSpawnPos;

            // Calculate direction FROM camera TO the book's position (Fixed Backwards Spawn)
            Vector3 lookDirection = finalSpawnPos - camPos;
            lookDirection.y = 0; // Keep the book upright
            finalSpawnRot = Quaternion.LookRotation(lookDirection.normalized);
        }

        // --- Instantiate ---
        GameObject spawnedGrid = Instantiate(spellGridPrefab, finalSpawnPos, finalSpawnRot);
        activeGridInstance = spawnedGrid.GetComponent<GridVisualizer>();

        if (activeGridInstance == null)
        {
            Debug.LogError("Spell Grid Prefab is missing the GridVisualizer component!");
            Destroy(spawnedGrid);
            gridActive = false;
            return;
        }

        activeGridInstance.ShowGrid(true);
        currentPath.Clear();
    }

    private void CloseGrid()
    {
        gridActive = false;

        if (activeGridInstance != null)
        {
            Destroy(activeGridInstance.gameObject);
            activeGridInstance = null; // This is the key to allowing a new one to spawn
        }

        ClearHighlights();
    }

    private void ProcessDrawing()
    {
        // Raycast from center of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (activeGridInstance.TryGetCellFromRay(ray, out GridCell currentCell))
        {
            if (currentPath.Count == 0 || currentCell != lastCell)
            {
                currentPath.Add(currentCell);
                activeGridInstance.HighlightCell(currentCell, true);
                lastCell = currentCell;
            }
        }
    }

    private void CheckForSpellMatch()
    {
        // Don't check if we didn't draw enough
        if (currentPath.Count < minPatternLength) return;

        string currentPattern = string.Join("-", currentPath);
        foreach (GridSpellSO spell in spellDatabase.gridSpells)
        {
            if (currentPattern.Contains(string.Join("-", spell.pattern)))
            {
                // --- LOAD THE SPELL ---
                loadedSpell = spell;
                Debug.Log("Spell Loaded: " + spell.name);

                // Tell the wand to show the icon
                if (wandFeedback != null)
                {
                    wandFeedback.ShowSpellIcon(loadedSpell.spellIcon);
                }

                break; // Found a match, stop checking
            }
        }
    }

    private void FireLoadedSpell()
    {
        if (loadedSpell == null) return;

        Debug.Log("Fired Spell: " + loadedSpell.name);

        // Instantiate the effect of the loaded spell
        Instantiate(
            loadedSpell.castEffect,
            playerCamera.transform.position + playerCamera.transform.forward * 2f,
            playerCamera.transform.rotation
        );

        // --- IMPORTANT: Clear the spell ---

        // Tell the wand to hide the icon
        if (wandFeedback != null)
        {
            wandFeedback.HideSpellIcon();
        }
        loadedSpell = null;
    }

    private void ClearHighlights()
    {
        // Only try to clear if the grid still exists
        if (activeGridInstance != null)
        {
            foreach (GridCell cell in currentPath)
            {
                activeGridInstance.HighlightCell(cell, false);
            }
        }
        currentPath.Clear();
    }
}