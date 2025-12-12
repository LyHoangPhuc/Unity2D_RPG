using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectTrigger : MonoBehaviour
{
    [Header("Auto Destruction")]
    [SerializeField] private bool destroyOnPlayerTouch = false;
    [SerializeField] private bool destroyOnEnemyTouch = false;
    [SerializeField] private float destroyDelay = 0f;

    private DestructibleObject destructibleObject;

    private void Start()
    {
        destructibleObject = GetComponentInParent<DestructibleObject>();
        if (destructibleObject == null)
        {
            Debug.LogError("DestructibleObjectTrigger cần có DestructibleObject component!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (destructibleObject == null) return;

        bool shouldDestroy = false;

        if (destroyOnPlayerTouch && other.GetComponent<Player>() != null)
        {
            shouldDestroy = true;
        }

        if (destroyOnEnemyTouch && other.GetComponent<Enemy>() != null)
        {
            shouldDestroy = true;
        }

        if (shouldDestroy)
        {
            if (destroyDelay > 0)
            {
                Invoke(nameof(DestroyObject), destroyDelay);
            }
            else
            {
                DestroyObject();
            }
        }
    }

    private void DestroyObject()
    {
        destructibleObject.TakeDamage(999999); // Damage lớn để phá vỡ ngay
    }
}
