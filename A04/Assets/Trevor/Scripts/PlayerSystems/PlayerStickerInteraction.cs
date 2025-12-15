using UnityEngine;
using System; // Required for Action

public class PlayerStickerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3.5f;
    public LayerMask interactionLayer;
    public KeyCode collectKey = KeyCode.F;

    // EVENT for picking up a duplicate/already known sticker
    public event Action OnStickerReCollected;

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
                // 1. Check if we ALREADY have this sticker
                bool isDuplicate = false;
                if (CollectionManager.Instance != null && currentTarget.data != null)
                {
                    isDuplicate = CollectionManager.Instance.HasSticker(currentTarget.data);
                }

                // 2. Play Audio Logic
                if (isDuplicate)
                {
                    // We already have it, so the CollectionManager won't fire.
                    // We fire our own "Generic Pickup" event here.
                    OnStickerReCollected?.Invoke();
                }
                // Else: It is new. CollectionManager.UnlockSticker will fire "OnStickerAdded".

                // 3. Destroy/Collect the object
                currentTarget.Collect();
                currentTarget = null;
            }
        }
    }
}