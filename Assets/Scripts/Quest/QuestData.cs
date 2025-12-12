using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestProgressData
{
    public string questId;
    public float timeRemaining;
    public List<ObjectiveProgressData> objectiveProgress = new List<ObjectiveProgressData>();
}



[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public string questId;
    public string questName;
    [TextArea(3, 5)]
    public string questDescription;
    public Sprite questIcon;

    [Header("Quest Properties")]
    public QuestType questType = QuestType.Side;
    public QuestPriority priority = QuestPriority.Normal;
    public bool isRepeatable = false;
    public bool autoComplete = false; // Tự động hoàn thành khi đủ objectives

    [Header("Quest Status")]
    public QuestStatus status = QuestStatus.NotStarted;

    [Header("Requirements")]
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public int playerLevelRequired = 1;
    public string[] prerequisiteQuestIds; // Quest cần hoàn thành trước

    [Header("Rewards")]
    public int expReward;
    public int currencyReward;
    public ItemData[] itemRewards;

    [Header("Dialogue Integration")]
    public string startDialogueId;
    public string inProgressDialogueId;
    public string completeDialogueId;
    public string failedDialogueId;

    [Header("Time Limits (Optional)")]
    public bool hasTimeLimit = false;
    public float timeLimitInSeconds = 300f; // 5 phút

    // Runtime properties
    [System.NonSerialized]
    public float timeRemaining;

    private void OnEnable()
    {
        // Reset time limit when quest is loaded
        if (hasTimeLimit)
        {
            timeRemaining = timeLimitInSeconds;
        }
    }

    // Methods
    public bool CanStart()
    {
        // Check player level
        if (PlayerLevel.instance != null && PlayerLevel.instance.currentLevel < playerLevelRequired)
        {
            return false;
        }

        // Check prerequisites
        if (prerequisiteQuestIds != null && prerequisiteQuestIds.Length > 0)
        {
            foreach (string prereqId in prerequisiteQuestIds)
            {
                if (!QuestManager.instance.IsQuestCompleted(prereqId))
                {
                    return false;
                }
            }
        }

        // Check if already completed (and not repeatable)
        if (!isRepeatable && status == QuestStatus.Completed)
        {
            return false;
        }

        return status == QuestStatus.NotStarted || (isRepeatable && status == QuestStatus.Completed);
    }

    public bool IsCompleted()
    {
        if (objectives == null || objectives.Count == 0) return false;

        foreach (var objective in objectives)
        {
            if (!objective.isOptional && !objective.isCompleted)
            {
                return false;
            }
        }
        return true;
    }

    public float GetOverallProgress()
    {
        if (objectives == null || objectives.Count == 0) return 0f;

        float totalProgress = 0f;
        int validObjectives = 0;

        foreach (var objective in objectives)
        {
            if (!objective.isOptional)
            {
                totalProgress += objective.Progress;
                validObjectives++;
            }
        }

        return validObjectives > 0 ? totalProgress / validObjectives : 0f;
    }

    public void ResetQuest()
    {
        status = QuestStatus.NotStarted;
        if (objectives != null)
        {
            foreach (var objective in objectives)
            {
                objective.ResetProgress();
            }
        }

        if (hasTimeLimit)
        {
            timeRemaining = timeLimitInSeconds;
        }
    }

    public QuestObjective GetObjective(string objectiveId)
    {
        if (objectives == null) return null;

        foreach (var objective in objectives)
        {
            if (objective.objectiveId == objectiveId)
            {
                return objective;
            }
        }
        return null;
    }

    public string GetStatusText()
    {
        switch (status)
        {
            case QuestStatus.NotStarted:
                return "Available";
            case QuestStatus.InProgress:
                return hasTimeLimit ? $"In Progress ({Mathf.CeilToInt(timeRemaining)}s)" : "In Progress";
            case QuestStatus.Completed:
                return "Completed";
            case QuestStatus.Failed:
                return "Failed";
            default:
                return "Unknown";
        }
    }
}

public enum QuestType
{
    Main,      // Quest chính
    Side,      // Quest phụ
    Daily,     // Quest hàng ngày
    Tutorial,  // Quest hướng dẫn
    Event      // Quest sự kiện
}

public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

public enum QuestPriority
{
    Low,
    Normal,
    High,
    Critical
}
