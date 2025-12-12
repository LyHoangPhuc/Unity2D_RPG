using UnityEngine;

/// <summary>
/// Component để tự động track enemy kills cho quest system
/// Attach vào GameObject có QuestManager hoặc GameManager
/// </summary>
public class EnemyKillTracker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool autoTrackAllEnemies = true;
    [SerializeField] private bool debugMode = true;

    private void Start()
    {
        if (autoTrackAllEnemies)
        {
            // Find và subscribe tất cả enemies hiện có
            SubscribeToExistingEnemies();
        }
    }

    private void SubscribeToExistingEnemies()
    {
        EnemyStats[] allEnemies = FindObjectsOfType<EnemyStats>();

        foreach (EnemyStats enemy in allEnemies)
        {
            SubscribeToEnemy(enemy);
        }

        //if (debugMode)
        //    Debug.Log($"EnemyKillTracker: Subscribed to {allEnemies.Length} existing enemies");
    }

    /// <summary>
    /// Subscribe to một enemy cụ thể
    /// </summary>
    public void SubscribeToEnemy(EnemyStats enemy)
    {
        if (enemy == null) return;

        enemy.OnEnemyKilled += OnEnemyDied;

        //if (debugMode)
        //    Debug.Log($"EnemyKillTracker: Subscribed to enemy: {enemy.GetEnemyId()}");
    }

    /// <summary>
    /// Unsubscribe from một enemy cụ thể
    /// </summary>
    public void UnsubscribeFromEnemy(EnemyStats enemy)
    {
        if (enemy == null) return;

        enemy.OnEnemyKilled -= OnEnemyDied;

        //if (debugMode)
        //    Debug.Log($"EnemyKillTracker: Unsubscribed from enemy: {enemy.GetEnemyId()}");
    }

    private void OnEnemyDied(EnemyStats enemy)
    {
        if (QuestManager.instance != null)
        {
            string enemyId = enemy.GetEnemyId();
            string enemyType = enemy.GetEnemyType().ToString().ToLower();
            QuestManager.instance.OnEnemyKilled(enemyId, enemyType);
        }

        if (debugMode)
            Debug.Log($"EnemyKillTracker: Enemy death tracked: {enemy.GetEnemyId()} (Type: {enemy.GetEnemyType()})");

        // Auto unsubscribe since enemy will be destroyed
        UnsubscribeFromEnemy(enemy);
    }

    /// <summary>
    /// Call này khi spawn enemy mới
    /// </summary>
    public static void RegisterNewEnemy(EnemyStats enemy)
    {
        EnemyKillTracker tracker = FindObjectOfType<EnemyKillTracker>();
        if (tracker != null)
        {
            tracker.SubscribeToEnemy(enemy);
        }
    }

    private void OnDestroy()
    {
        // Cleanup - unsubscribe from all enemies
        EnemyStats[] allEnemies = FindObjectsOfType<EnemyStats>();
        foreach (EnemyStats enemy in allEnemies)
        {
            if (enemy != null)
                UnsubscribeFromEnemy(enemy);
        }
    }
}
