using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Destructible Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Destruction Effects")]
    private GameObject destructionEffect; // Particle effect khi phá vỡ
    [SerializeField] private int destructionSfxIndex = -1; // Index trong AudioManager

    [Header("Interaction")]
    [SerializeField] private bool canBeAttacked = true;
    [SerializeField] private bool canBeInteracted = false; // Có thể tương tác bằng phím hay không
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Visual Feedback")]
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private ItemDrop itemDropSystem;
    private Color originalColor;
    private bool isDestroyed = false;
    private bool playerNearby = false;

    private void Start()
    {
        currentHealth = maxHealth;
        itemDropSystem = GetComponent<ItemDrop>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Kiểm tra nếu không có ItemDrop component
        if (itemDropSystem == null)
        {
            Debug.LogWarning($"DestructibleObject {gameObject.name} không có ItemDrop component!");
        }
    }

    private void Update()
    {
        // Xử lý tương tác bằng phím
        if (canBeInteracted && playerNearby && !isDestroyed)
        {
            if (Input.GetKeyDown(interactKey))
            {
                TakeDamage(maxHealth); // Phá vỡ ngay lập tức
            }
        }
    }

    // Nhận sát thương từ player attack
    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        // Visual feedback
        if (spriteRenderer != null)
            StartCoroutine(FlashColor());

        // Play hit sound effect
        if (destructionSfxIndex >= 0 && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(destructionSfxIndex, transform);

        // Kiểm tra nếu bị phá vỡ
        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        // Tạo effect phá vỡ
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
        AudioManager.instance.PlaySFX(40, null);

        // Drop items sử dụng ItemDrop system có sẵn
        if (itemDropSystem != null)
        {
            itemDropSystem.GenerateDrop();
        }

        // Destroy object
        Destroy(gameObject, 0.1f);
    }

    private IEnumerator FlashColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    // Detect player cho interaction
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerNearby = false;
        }
    }

    // Method để nhận damage từ player attack
    public void OnPlayerAttack(int damage)
    {
        if (canBeAttacked)
        {
            TakeDamage(damage);
        }
    }
}
