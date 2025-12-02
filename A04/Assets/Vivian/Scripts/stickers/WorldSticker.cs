using UnityEngine;

public class WorldSticker : MonoBehaviour
{
    [Header("Settings")]
    public StickerData data;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("UI Interaction")]
    [SerializeField] private GameObject interactionCanvas; // Assign the Canvas or Panel here
    [SerializeField] private bool billboardUI = true; // Should UI always face player?

    private Transform mainCameraTransform;

    void Start()
    {

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Optional: Make the UI face the player so it's readable
        // regardless of how the sticker is rotated on the wall
        if (billboardUI && interactionCanvas != null && interactionCanvas.activeSelf)
        {
            // Make the canvas look at the camera
            // We rotate 180 degrees because UI looks 'backwards' by default in World Space
            interactionCanvas.transform.LookAt(interactionCanvas.transform.position + mainCameraTransform.rotation * Vector3.forward,
                                               mainCameraTransform.rotation * Vector3.up);
        }
    }

    public void Initialize(StickerData stickerData)
    {
        data = stickerData;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = data.unlockedSprite;
        }
    }

    // --- NEW METHODS FOR UI ---

    public void ShowPrompt()
    {
        if (interactionCanvas != null) interactionCanvas.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactionCanvas != null) interactionCanvas.SetActive(false);
    }

    public void Collect()
    {
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.UnlockSticker(data);
        }
        Destroy(gameObject);
    }
}