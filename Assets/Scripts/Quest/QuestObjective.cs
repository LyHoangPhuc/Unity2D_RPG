using UnityEngine;


[System.Serializable]
public class ObjectiveProgressData
{
    public string objectiveId;
    public int currentAmount;
    public bool isCompleted;
}

[System.Serializable]
public class QuestObjective
{
    [Header("Objective Info")]
    public string objectiveId;
    [TextArea(2, 3)]
    public string description;
    public ObjectiveType type;

    [Header("Progress")]
    public bool isCompleted;
    public bool isOptional; // Objective tùy chọn

    [Header("Target Requirements")]
    public string targetId;        // ID của target (enemy type, item id, npc name...)
    public int requiredAmount = 1; // Số lượng cần
    public int currentAmount;      // Số lượng hiện tại

    [Header("Location (if needed)")]
    public Vector2 targetLocation; // Vị trí cần đến (cho ReachLocation)
    public float locationRadius = 2f; // Bán kính để check đã đến

    // Properties
    public bool IsCompleted => currentAmount >= requiredAmount;
    public float Progress => Mathf.Clamp01((float)currentAmount / requiredAmount);

    // Methods
    public void UpdateProgress(int amount)
    {
        if (isCompleted) return;

        currentAmount = Mathf.Min(currentAmount + amount, requiredAmount);

        if (IsCompleted && !isCompleted)
        {
            isCompleted = true;
            Debug.Log($"Objective completed: {description}");
        }
    }

    public void SetCompleted()
    {
        currentAmount = requiredAmount;
        isCompleted = true;
    }

    public void ResetProgress()
    {
        currentAmount = 0;
        isCompleted = false;
    }

    /// <summary>
    /// Get formatted progress text for UI display
    /// </summary>
    public string GetProgressText()
    {
        switch (type.ToString().ToLower())
        {
            case "kill":
                string targetName = GetTargetDisplayName();
                return $"Kill {targetName}: {currentAmount}/{requiredAmount}";

            case "collect":
                return $"Collect {targetId}: {currentAmount}/{requiredAmount}";

            case "interact":
                return $"Interact with {targetId}: {currentAmount}/{requiredAmount}";

            case "reach":
                return $"Reach {targetId}: {(isCompleted ? "Complete" : "Incomplete")}";

            default:
                return $"{description}: {currentAmount}/{requiredAmount}";
        }
    }

    /// <summary>
    /// Get display name for target (convert IDs to readable names)
    /// </summary>
    private string GetTargetDisplayName()
    {
        switch (targetId.ToLower())
        {
            case "skeleton": return "Skeletons";
            case "mushroom": return "Mushrooms";
            case "deathbringer": return "Death Bringers";
            case "any": return "Any Enemies";
            case "normal": return "Normal Enemies";
            case "elite": return "Elite Enemies";
            case "boss": return "Bosses";
            case "undead": return "Undead";
            case "beast": return "Beasts";
            case "demon": return "Demons";
            case "elemental": return "Elementals";
            default: return targetId;
        }
    }

}
public enum ObjectiveType
{
    KillEnemies,     // Giết quái vật
    CollectItems,    // Thu thập vật phẩm
    TalkToNPC,       // Nói chuyện với NPC
    ReachLocation,   // Đến địa điểm
    UseItem,         // Sử dụng vật phẩm
    LevelUp,         // Lên level
    DefeatBoss,      // Đánh bại boss
    Custom           // Tùy chỉnh
}
