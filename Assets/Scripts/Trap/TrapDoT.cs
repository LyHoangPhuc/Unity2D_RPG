using UnityEngine;
using System.Collections;

public class TrapDoT : TrapBase
{
    [Header("Lava Settings")]
    [SerializeField] private float tickInterval = 0.5f;

    [Header("Audio (SFX)")]
    [Tooltip("Chỉ mục SFX cho lava tick (nếu có)")]
    [SerializeField] private int tickSfxIndex = -1;

    protected override void OnTrapEnter(Collider2D other)
    {
        StartCoroutine(DamageOverTime(other));
    }

    private IEnumerator DamageOverTime(Collider2D other)
    {
        var stats = other.GetComponent<CharacterStats>();
        while (stats != null && col.IsTouching(other))
        {
            stats.TakeDamage(damage);

            // Phát SFX cho mỗi lần tick (nếu index >= 0)
            if (tickSfxIndex >= 0 && AudioManager.instance != null)
                AudioManager.instance.PlaySFX(tickSfxIndex, transform);

            yield return new WaitForSeconds(tickInterval);
        }
    }
}
