using UnityEngine;
using System;

public class StickerPickup : MonoBehaviour
{
    private StickerData _data;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public event Action OnStickerCollected;

    // Called by the Enemy immediately after spawning this object
    public void Initialize(StickerData data)
    {
        _data = data;

        // Update the visual of the physical object to match the sticker art
        if (spriteRenderer != null && data.unlockedSprite != null)
        {
            spriteRenderer.sprite = data.unlockedSprite;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (_data != null && CollectionManager.Instance != null)
        {
            CollectionManager.Instance.UnlockSticker(_data);
            OnStickerCollected?.Invoke();
        }

        // Optional: Play sound here
        Destroy(gameObject);
    }

    void Update()
    {
        // Simple floating animation
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
        transform.position += new Vector3(0, Mathf.Sin(Time.time * 2) * 0.002f, 0);
    }
}