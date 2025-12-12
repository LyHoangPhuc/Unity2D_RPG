using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class TrapBase : MonoBehaviour
{
    [Header("Common Trap Settings")]
    [SerializeField] protected int damage;
    [SerializeField] protected bool oneTime = true;    // chỉ kích hoạt 1 lần?

    protected Collider2D col;

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    // Khi có cái gì đó chạm vào trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CanAffect(other)) 
            return;

        OnTrapEnter(other);

        if (oneTime)
            Destroy(gameObject);
    }

    // Nếu cần tác động liên tục, override OnTriggerStay2D
    protected virtual void OnTriggerStay2D(Collider2D other) { }

    // Kiểm tra đây có phải là đối tượng cần gây sát thương (mặc định là Player/Enemy có CharacterStats)
    protected virtual bool CanAffect(Collider2D other)
    {
        return other.GetComponent<CharacterStats>() != null;
    }

    // Chỉ định hành vi khi chạm
    protected abstract void OnTrapEnter(Collider2D other);
}
