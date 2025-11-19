using System.Collections.Generic;
using UnityEngine;

public class SpellGridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spellGridPrefab; // Your spellbook prefab
    [SerializeField] private SpellDatabase spellDatabase;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private WandFeedback wandFeedback; // The script on your wand

    [Header("Grid Settings")]
    [SerializeField] private float gridDistance = 2f;
    [SerializeField] private float gridYOffset = -0.5f;
    [SerializeField] private float minPatternLength = 3;

    [Header("Collision Settings")]
    [SerializeField] private float spawnBuffer = 0.1f; // Pushes the book slightly off the wall
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.1f); // Size of the cast box (Half-Width, Half-Height, Half-Thickness)
    [SerializeField] private float minSpawnDistance = 0.5f; // If wall forces book closer than this, don't spawn
    [SerializeField] private LayerMask collisionLayers; // Walls/Floor layers

    // --- State Fields ---
    private List<GridCell> currentPath = new List<GridCell>();
    private GridCell lastCell;
    private bool gridActive = false;

    private GridVisualizer activeGridInstance;
    private GridSpellSO loadedSpell = null;

    // --- Public Properties for Strategies ---
    // The Strategy scripts need to access these to know where to aim/fire from
    public Camera PlayerCamera => playerCamera;
    public GridSpellSO LoadedSpell => loadedSpell;

    private void Update()
    {
        HandleInput();

        // --- STRATEGY DELEGATION ---
        // If a spell is loaded, let the Strategy handle the aiming logic (e.g., moving the AOE circle)
        if (!gridActive && loadedSpell != null && loadedSpell.castStrategy != null)
        {
            loadedSpell.castStrategy.OnAiming(this);
        }
    }

    private void HandleInput()
    {
        // 1. Toggle Grid OR Cancel Spell
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (loadedSpell != null)
            {
                CancelLoadedSpell();
            }
            else
            {
                ToggleGrid();
            }
        }

        // 2. Fire Loaded Spell
        if (Input.GetMouseButtonDown(0) && !gridActive && loadedSpell != null)
        {
            FireLoadedSpell();
        }
        // 3. Draw Pattern (while Grid is active)
        else if (gridActive && activeGridInstance != null && Input.GetMouseButton(0))
        {
            ProcessDrawing();
        }
        // 4. Finish Drawing (Mouse Up)
        else if (Input.GetMouseButtonUp(0) && currentPath.Count > 0)
        {
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
        gridActive = !gridActive;

        if (gridActive)
        {
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
        Vector3 camPos = camTransform.position;

        // --- 1. Determine Direction & Ideal Rotation ---
        Vector3 horizontalForward = camTransform.forward;
        horizontalForward.y = 0;
        horizontalForward.Normalize();

        // Fallback if looking straight down/up
        if (horizontalForward == Vector3.zero) horizontalForward = transform.forward;

        // Calculate the rotation FIRST. We want it to face the player regardless of walls.
        Quaternion facePlayerRot = Quaternion.LookRotation(horizontalForward);

        // --- 2. Prepare BoxCast Data ---
        Vector3 castOrigin = camPos;
        Vector3 castDirection = horizontalForward;
        Vector3 finalSpawnPos;

        // Perform BoxCast to check for walls/obstacles
        // We pass 'facePlayerRot' so the box is rotated correctly while checking
        bool hitWall = Physics.BoxCast(
            castOrigin,
            boxHalfExtents,
            castDirection,
            out RaycastHit hit,
            facePlayerRot,
            gridDistance,
            collisionLayers
        );

        if (hitWall)
        {
            // --- HIT WALL LOGIC ---
            // Calculate safe distance: Hit Distance minus buffer
            float safeDistance = hit.distance - spawnBuffer;

            // SAFETY CHECK: Is the book now inside the player?
            if (safeDistance < minSpawnDistance)
            {
                Debug.Log("Cannot open spellbook: Wall is too close!");
                gridActive = false;
                return; // ABORT SPAWN
            }

            finalSpawnPos = castOrigin + (castDirection * safeDistance);
        }
        else
        {
            // --- CLEAR PATH LOGIC ---
            finalSpawnPos = castOrigin + (castDirection * gridDistance);
        }

        // Apply the Y-Offset (Height adjustment)
        finalSpawnPos.y = camPos.y + gridYOffset;

        // --- 3. Instantiate ---
        GameObject spawnedGrid = Instantiate(spellGridPrefab, finalSpawnPos, facePlayerRot);
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
            activeGridInstance = null;
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
        if (currentPath.Count < minPatternLength) return;

        string currentPattern = string.Join("-", currentPath);

        foreach (GridSpellSO spell in spellDatabase.gridSpells)
        {
            if (currentPattern.Contains(string.Join("-", spell.pattern)))
            {
                // --- SPELL FOUND ---
                loadedSpell = spell;
                Debug.Log("Spell Loaded: " + spell.name);

                // 1. Show Icon
                if (wandFeedback != null) wandFeedback.ShowSpellIcon(loadedSpell.spellIcon);

                // 2. Notify Strategy (This spawns the indicator if needed)
                if (loadedSpell.castStrategy != null)
                {
                    loadedSpell.castStrategy.OnSpellLoaded(this);
                }
                else
                {
                    Debug.LogWarning($"Spell {spell.name} has no Cast Strategy assigned!");
                }

                break; // Stop checking after first match
            }
        }
    }

    private void FireLoadedSpell()
    {
        if (loadedSpell == null) return;

        Debug.Log("Fired Spell: " + loadedSpell.name);

        // 1. Execute Strategy (This instantiates the Fireball or Wind)
        if (loadedSpell.castStrategy != null)
        {
            loadedSpell.castStrategy.Fire(this);
        }

        // 2. Cleanup
        if (wandFeedback != null) wandFeedback.HideSpellIcon();
        loadedSpell = null;
    }

    private void CancelLoadedSpell()
    {
        // 1. Notify Strategy (To destroy the AOE indicator)
        if (loadedSpell != null && loadedSpell.castStrategy != null)
        {
            loadedSpell.castStrategy.OnCancel(this);
        }

        // 2. Cleanup
        loadedSpell = null;
        if (wandFeedback != null) wandFeedback.HideSpellIcon();
        Debug.Log("Spell Canceled");
    }

    private void ClearHighlights()
    {
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