using UnityEngine;

public class TutorialInfoInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player needs to be to read/dismiss (in meters)")]
    public float interactRange = 4.0f;
    public KeyCode interactKey = KeyCode.F;

    [Header("UI Reference")]
    [Tooltip("Assign the World Space Canvas that has your text here")]
    public GameObject infoCanvas;

    private Transform playerTransform;

    void Start()
    {
        // 1. Find player by Tag (Robust way)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("TutorialInfoInteraction: Could not find object tagged 'Player'!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 3. THE MAGIC: Pure Math Check (No Colliders)
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactRange)
        {
            // --- PLAYER IS CLOSE ---

            // B. Listen for Input
            if (Input.GetKeyDown(interactKey))
            {
                Dismiss();
            }
        }
    }

    void Dismiss()
    {
        Debug.Log("Tutorial Dismissed.");
        // Destroy the wall (and this script attached to it)
        Destroy(gameObject);
    }
}