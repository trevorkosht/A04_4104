using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class WandFeedback : MonoBehaviour
{
    // --- Use ONE of these ---
    // A. For a 3D sprite in the world
    [SerializeField] private SpriteRenderer iconSpriteRenderer;

    // B. For a 2D UI element
    // [SerializeField] private Image iconImage;

    [Header("Settings")]
    // Adjust these numbers (X, Y, Z) in the inspector to make the sprite smaller.
    // Try (0.1, 0.1, 0.1) if your sprites are huge.
    [SerializeField] private Vector3 iconScale = new Vector3(0.2f, 0.2f, 0.2f);

    private void Awake()
    {
        HideSpellIcon();
    }

    public void ShowSpellIcon(Sprite icon)
    {
        if (icon == null) return;

        // --- A. For SpriteRenderer ---
        iconSpriteRenderer.sprite = icon;

        // Apply the custom scale here
        iconSpriteRenderer.transform.localScale = iconScale;

        iconSpriteRenderer.gameObject.SetActive(true);

        // --- B. For UI Image ---
        // iconImage.sprite = icon;
        // iconImage.rectTransform.localScale = iconScale; // UI uses RectTransform
        // iconImage.gameObject.SetActive(true);
    }

    public void HideSpellIcon()
    {
        // --- A. For SpriteRenderer ---
        iconSpriteRenderer.sprite = null;
        iconSpriteRenderer.gameObject.SetActive(false);

        // --- B. For UI Image ---
        // iconImage.sprite = null;
        // iconImage.gameObject.SetActive(false);
    }
}