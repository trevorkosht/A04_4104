using UnityEngine;
using UnityEngine.UI; // If you use a UI Image
using UnityEngine.Rendering; // If you use a SpriteRenderer

// This script goes on your Wand GameObject
public class WandFeedback : MonoBehaviour
{
    // --- Use ONE of these ---
    // A. For a 3D sprite in the world (more "on the wand")
    [SerializeField] private SpriteRenderer iconSpriteRenderer;

    // B. For a 2D UI element on the screen (simpler)
    // [SerializeField] private Image iconImage;

    private void Awake()
    {
        // Start with the icon hidden
        HideSpellIcon();
    }

    public void ShowSpellIcon(Sprite icon)
    {
        if (icon == null) return;

        // --- Use the line that matches your setup ---
        // A. For SpriteRenderer
        iconSpriteRenderer.sprite = icon;
        iconSpriteRenderer.gameObject.SetActive(true);

        // B. For UI Image
        // iconImage.sprite = icon;
        // iconImage.gameObject.SetActive(true);
    }

public void HideSpellIcon()
{
    // --- Use the line that matches your setup ---
    // A. For SpriteRenderer
    iconSpriteRenderer.sprite = null;
    iconSpriteRenderer.gameObject.SetActive(false);

    // B. For UI Image
    // iconImage.sprite = null;
    // iconImage.gameObject.SetActive(false);
}
}