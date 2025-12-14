using UnityEngine;
using System; // Required for Action

public class PlayerStickerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3.5f;
    public LayerMask interactionLayer;
    public KeyCode collectKey = KeyCode.F;

    // NEW EVENT
    public event Action OnStickerCollected;

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

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            WorldSticker sticker = hit.collider.GetComponent<WorldSticker>();
            if (sticker == null) sticker = hit.collider.GetComponentInParent<WorldSticker>();

            if (sticker != null)
            {
                if (currentTarget != sticker)
                {
                    if (currentTarget != null) currentTarget.HidePrompt();
                    currentTarget = sticker;
                    currentTarget.ShowPrompt();
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
        if (Input.GetKeyDown(collectKey))
        {
            if (currentTarget != null)
            {
                currentTarget.Collect();
                OnStickerCollected?.Invoke(); // NEW

                currentTarget = null;
            }
        }
    }
}