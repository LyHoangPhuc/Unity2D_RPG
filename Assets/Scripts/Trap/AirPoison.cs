using UnityEngine;
using System.Collections;

public class AirPoison : TrapBase
{
    [Header("Poison Settings")]
    [SerializeField] private float poisonDuration;
    [SerializeField] private float tickInterval;

    [Header("Audio (SFX)")]
    [Tooltip("Chỉ mục SFX khi bắt đầu trúng độc (nếu có)")]
    [SerializeField] private int startSfxIndex = -1;

    protected override void OnTrapEnter(Collider2D other)
    {
        var stats = other.GetComponent<CharacterStats>();
        if (stats != null)
        {
            // Phát SFX khai đầu
            if (startSfxIndex >= 0 && AudioManager.instance != null)
                AudioManager.instance.PlaySFX(startSfxIndex, transform);

            StartCoroutine(ApplyPoison(stats));
        }
    }

    private IEnumerator ApplyPoison(CharacterStats stats)
    {
        float elapsed = 0f;
        while (elapsed < poisonDuration && !stats.isDead)
        {
            stats.TakeDamage(damage);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
