using UnityEngine;

public class PlayerStickerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3.5f;
    public LayerMask interactionLayer;
    public KeyCode collectKey = KeyCode.F;

    private WorldSticker currentTarget;
    private Camera playerCam;

    void Start()
    {
        playerCam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    void HandleRaycast()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // DEBUG: Draw the ray so you can see where you are aiming in Scene View
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // DEBUG: Print what we hit
            // Debug.Log($"Hit: {hit.collider.gameObject.name} on Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            WorldSticker sticker = hit.collider.GetComponent<WorldSticker>();
            if (sticker == null) sticker = hit.collider.GetComponentInParent<WorldSticker>();

            if (sticker != null)
            {
                if (currentTarget != sticker)
                {
                    if (currentTarget != null) currentTarget.HidePrompt();
                    currentTarget = sticker;
                    currentTarget.ShowPrompt();
                    Debug.Log("Found Sticker! Showing Prompt.");
                }
                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.HidePrompt();
            currentTarget = null;
        }
    }

    void HandleInput()
    {
        // Debugging: Prove the F key actually works
        if (Input.GetKeyDown(collectKey))
        {
            Debug.Log("F Key Pressed");

            if (currentTarget != null)
            {
                Debug.Log($"Attempting to collect {currentTarget.name}...");
                currentTarget.Collect();

                // Immediately hide/clear so we don't try to collect a destroyed object
                currentTarget = null;
            }
            else
            {
                Debug.Log("F pressed, but currentTarget is null!");
            }
        }
    }
}