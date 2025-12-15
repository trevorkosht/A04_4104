using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneOnInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 4.0f;
    public KeyCode interactKey = KeyCode.G;

    [Header("Scene Settings")]
    public string sceneToLoad = "PlayGround";

    private Transform playerTransform;
    private bool isLoading = false;

    void Start()
    {
        // DEBUG 1: Initialization
        //Debug.Log($"[SwitchScene] Initialized on object: {gameObject.name}. Waiting for Player...");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            // DEBUG 2: Player Found
            //Debug.Log($"[SwitchScene] SUCCESS: Player found! Name: {player.name}");
        }
        else
        {
            // DEBUG 2: Player Missing
            Debug.LogError("[SwitchScene] ERROR: Could not find object with tag 'Player'! Make sure your Player object has the Tag 'Player' set in the Inspector.");
        }
    }

    void Update()
    {
        if (playerTransform == null || isLoading) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // --- NEW DEBUG ---
        // This prints the exact coordinate the script is looking at.
        // If you move and these numbers don't change, it found the wrong object!
        //Debug.Log($"Tracking Object: {playerTransform.name} | Position: {playerTransform.position} | Dist: {distance}");

        if (distance <= interactRange)
        {
            if (Input.GetKeyDown(interactKey))
            {
                LoadTargetScene();
            }
        }
        else if (Input.GetKeyDown(interactKey))
        {
            Debug.Log($"[SwitchScene] Too far! Distance: {distance} / {interactRange}");
        }
    }


    void LoadTargetScene()
    {
        isLoading = true;

        // DEBUG 6: Verifying Scene Existence
        if (Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.Log($"[SwitchScene] Scene '{sceneToLoad}' found in build settings. Switching now...");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError($"[SwitchScene] ERROR: Scene '{sceneToLoad}' cannot be loaded! Check your spelling and go to File -> Build Settings to ensure it is added to the list.");
            isLoading = false; // Reset so we can try again
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}