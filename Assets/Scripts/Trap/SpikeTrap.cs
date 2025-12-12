using UnityEngine;

public class SpikeTrap : TrapBase
{
    [Header("Audio (SFX)")]
    [Tooltip("Chỉ mục trong AudioManager.sfx[]")]
    [SerializeField] private int sfxIndex;

    protected override void OnTrapEnter(Collider2D other)
    {
        var stats = other.GetComponent<CharacterStats>();
        stats?.TakeDamage(damage);

        // Dùng AudioManager để phát SFX (tự check khoảng cách)
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(sfxIndex, transform);
    }
}
