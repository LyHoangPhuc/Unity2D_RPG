using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    [Header("Particle Prefabs")]
    [SerializeField] private GameObject attackEffectPrefab;
    [SerializeField] private GameObject jumpEffectPrefab;
    [SerializeField] private GameObject landEffectPrefab;
    [SerializeField] private GameObject dashEffectPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayAttackEffect(Vector3 position, bool facingRight = true)
    {
        if (attackEffectPrefab != null)
        {
            GameObject effect = Instantiate(attackEffectPrefab, position, Quaternion.identity);

            // Flip effect nếu nhân vật quay trái
                if (!facingRight)
                {
                    Vector3 scale = effect.transform.localScale;
                    scale.x *= -1;
                    effect.transform.localScale = scale;
                }

            Destroy(effect, 2f);
        }
    }

    public void PlayJumpEffect(Vector3 position)
    {
        if (jumpEffectPrefab != null)
        {
            GameObject effect = Instantiate(jumpEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void PlayLandEffect(Vector3 position)
    {
        if (landEffectPrefab != null)
        {
            GameObject effect = Instantiate(landEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void PlayDashEffect(Vector3 position)
    {
        if (dashEffectPrefab != null)
        {
            GameObject effect = Instantiate(dashEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
}
