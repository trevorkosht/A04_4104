using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellGridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spellGridPrefab;
    [SerializeField] private SpellDatabase spellDatabase;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private WandFeedback wandFeedback;

    [Header("Grid Settings")]
    [SerializeField] private float gridDistance = 2f;
    [SerializeField] private float gridYOffset = -0.5f;
    [SerializeField] private float minPatternLength = 3;

    [Header("Collision Settings")]
    [SerializeField] private float spawnBuffer = 0.1f;
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.1f);
    [SerializeField] private float minSpawnDistance = 0.5f;
    [SerializeField] private LayerMask collisionLayers;

    public event Action OnGridOpened;
    public event Action OnGridOpenFailed;
    public event Action OnCellHighlighted;
    public event Action OnSpellLoadSuccess;
    public event Action OnSpellLoadFailed;

    private List<GridCell> currentPath = new List<GridCell>();
    private GridCell lastCell;
    private bool gridActive = false;
    private GridVisualizer activeGridInstance;
    private GridSpellSO loadedSpell = null;

    public Camera PlayerCamera => playerCamera;
    public GridSpellSO LoadedSpell => loadedSpell;

    private void Update()
    {
        HandleInput();
        if (!gridActive && loadedSpell != null && loadedSpell.castStrategy != null)
        {
            loadedSpell.castStrategy.OnAiming(this);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (loadedSpell != null) CancelLoadedSpell();
            else ToggleGrid();
        }
        else if (Input.GetMouseButtonDown(0) && !gridActive && loadedSpell != null)
        {
            FireLoadedSpell();
        }
        else if (gridActive && activeGridInstance != null && Input.GetMouseButton(0))
        {
            ProcessDrawing();
        }
        else if (Input.GetMouseButtonUp(0) && currentPath.Count > 0)
        {
            CheckForSpellMatch();
            if (gridActive) CloseGrid();
        }
    }

    private void ToggleGrid() { gridActive = !gridActive; if (gridActive) OpenGrid(); else CloseGrid(); }

    private void OpenGrid()
    {
        Transform camTransform = playerCamera.transform;
        Vector3 horizontalForward = camTransform.forward;
        horizontalForward.y = 0;
        horizontalForward.Normalize();
        if (horizontalForward == Vector3.zero) horizontalForward = transform.forward;
        Quaternion facePlayerRot = Quaternion.LookRotation(horizontalForward);

        bool hitWall = Physics.BoxCast(camTransform.position, boxHalfExtents, horizontalForward, out RaycastHit hit, facePlayerRot, gridDistance, collisionLayers);

        if (hitWall && (hit.distance - spawnBuffer) < minSpawnDistance)
        {
            Debug.Log("Wall too close");
            gridActive = false;
            OnGridOpenFailed?.Invoke();
            return;
        }

        Vector3 finalSpawnPos = hitWall
            ? camTransform.position + (horizontalForward * (hit.distance - spawnBuffer))
            : camTransform.position + (horizontalForward * gridDistance);

        finalSpawnPos.y = camTransform.position.y + gridYOffset;

        GameObject spawnedGrid = Instantiate(spellGridPrefab, finalSpawnPos, facePlayerRot);
        activeGridInstance = spawnedGrid.GetComponent<GridVisualizer>();
        if (activeGridInstance) activeGridInstance.ShowGrid(true);
        currentPath.Clear();

        OnGridOpened?.Invoke();
    }

    private void CloseGrid()
    {
        gridActive = false;
        if (activeGridInstance) Destroy(activeGridInstance.gameObject);
        ClearHighlights();
    }

    private void ProcessDrawing()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (activeGridInstance.TryGetCellFromRay(ray, out GridCell currentCell))
        {
            if (currentPath.Count == 0 || currentCell != lastCell)
            {
                currentPath.Add(currentCell);
                activeGridInstance.HighlightCell(currentCell, true);
                lastCell = currentCell;

                OnCellHighlighted?.Invoke();
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
                // --- NEW LOGIC START ---
                // Before loading, check if the spell is on cooldown
                if (PlayerSpellSystem.Instance != null && PlayerSpellSystem.Instance.IsOnCooldown(spell))
                {
                    Debug.Log($"Spell {spell.spellName} recognized, but is on Cooldown. Load Aborted.");

                    // Trigger the specific "Cooldown Buzz" sound
                    PlayerSpellSystem.Instance.TriggerCooldownFail();

                    // Return immediately. 
                    // We treat this as a "Handled Failure" - we found a match, but rejected it.
                    // We do NOT fire OnSpellLoadFailed (generic fail) to avoid double sounds.
                    return;
                }
                // --- NEW LOGIC END ---

                loadedSpell = spell;
                Debug.Log("Spell Loaded: " + spell.name);
                if (wandFeedback != null) wandFeedback.ShowSpellIcon(loadedSpell.spellIcon);
                if (loadedSpell.castStrategy != null) loadedSpell.castStrategy.OnSpellLoaded(this);

                OnSpellLoadSuccess?.Invoke();
                return; // Match found and loaded, exit function
            }
        }

        // If loop finishes without returning, no match was found (or accepted)
        OnSpellLoadFailed?.Invoke();
    }

    private void FireLoadedSpell()
    {
        if (loadedSpell == null) return;

        if (PlayerSpellSystem.Instance != null)
        {
            if (!PlayerSpellSystem.Instance.CanCast(loadedSpell)) return;
            PlayerSpellSystem.Instance.CastSpell(loadedSpell);
        }

        Debug.Log("Fired Spell: " + loadedSpell.name);
        if (loadedSpell.castStrategy != null) loadedSpell.castStrategy.Fire(this);

        if (wandFeedback != null) wandFeedback.HideSpellIcon();
        loadedSpell = null;
    }

    private void CancelLoadedSpell()
    {
        if (loadedSpell != null && loadedSpell.castStrategy != null) loadedSpell.castStrategy.OnCancel(this);
        loadedSpell = null;
        if (wandFeedback != null) wandFeedback.HideSpellIcon();
    }

    private void ClearHighlights()
    {
        if (activeGridInstance != null)
        {
            foreach (GridCell cell in currentPath) activeGridInstance.HighlightCell(cell, false);
        }
        currentPath.Clear();
    }
}