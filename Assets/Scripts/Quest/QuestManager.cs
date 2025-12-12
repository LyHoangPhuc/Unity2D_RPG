using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour, ISaveManager
{
    [Header("Quest Database")]
    public List<QuestData> allQuests = new List<QuestData>();

    [Header("Debug")]
    public bool enableDebugLogs = true;

    // Runtime quest lists
    [SerializeField] private List<QuestData> activeQuests = new List<QuestData>();
    [SerializeField] private List<QuestData> availableQuests = new List<QuestData>();
    [SerializeField] private List<QuestData> completedQuests = new List<QuestData>();
    [SerializeField] private List<QuestData> failedQuests = new List<QuestData>();

    // Events
    public System.Action<QuestData> OnQuestStarted;
    public System.Action<QuestData> OnQuestCompleted;
    public System.Action<QuestData> OnQuestFailed;
    public System.Action<QuestObjective, QuestData> OnObjectiveCompleted;
    public System.Action<QuestData> OnQuestProgressUpdated;

    // Singleton
    public static QuestManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadQuestDatabase();
    }

    private void Update()
    {
        UpdateActiveQuests();
    }

    private void LoadQuestDatabase()
    {
        // Load all quest assets from Resources folder
        QuestData[] questAssets = Resources.LoadAll<QuestData>("Quests");

        foreach (var quest in questAssets)
        {
            if (!allQuests.Contains(quest))
            {
                allQuests.Add(quest);
            }
        }

        DebugLog($"Loaded {allQuests.Count} quests from database");
    }

    private void UpdateActiveQuests()
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestData quest = activeQuests[i];

            // Update time limit
            if (quest.hasTimeLimit && quest.timeRemaining > 0)
            {
                quest.timeRemaining -= Time.deltaTime;

                if (quest.timeRemaining <= 0)
                {
                    FailQuest(quest.questId, "Time limit exceeded");
                    continue;
                }
            }

            // Auto complete check
            if (quest.autoComplete && quest.IsCompleted() && quest.status == QuestStatus.InProgress)
            {
                CompleteQuest(quest.questId);
            }
        }
    }

    #region Quest Management

    public bool StartQuest(string questId)
    {
        var questToStart = availableQuests.Find(q => q.questId == questId);
        if (questToStart != null && !activeQuests.Contains(questToStart))
        {
            questToStart.status = QuestStatus.InProgress;
            activeQuests.Add(questToStart);
            DebugLog($"Quest started: {questToStart.questName}");

            // Trigger event
            OnQuestStarted?.Invoke(questToStart);

            return true;
        }
        return false;
    }

    public bool CompleteQuest(string questId)
    {
        QuestData quest = GetActiveQuest(questId);

        if (quest == null)
        {
            DebugLog($"Active quest not found: {questId}", true);
            return false;
        }

        if (!quest.IsCompleted())
        {
            DebugLog($"Quest objectives not completed: {questId}", true);
            return false;
        }

        quest.status = QuestStatus.Completed;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        // Give rewards
        GiveRewards(quest);

        DebugLog($"Quest completed: {quest.questName}");

        // Trigger events and dialogue
        OnQuestCompleted?.Invoke(quest);

        if (!string.IsNullOrEmpty(quest.completeDialogueId))
        {
            StoryManager.instance?.TriggerStoryEvent(quest.completeDialogueId);
        }

        return true;
    }

    public bool FailQuest(string questId, string reason = "")
    {
        QuestData quest = GetActiveQuest(questId);

        if (quest == null)
        {
            DebugLog($"Active quest not found: {questId}", true);
            return false;
        }

        quest.status = QuestStatus.Failed;
        activeQuests.Remove(quest);
        failedQuests.Add(quest);

        DebugLog($"Quest failed: {quest.questName}. Reason: {reason}");

        // Trigger events and dialogue
        OnQuestFailed?.Invoke(quest);

        if (!string.IsNullOrEmpty(quest.failedDialogueId))
        {
            StoryManager.instance?.TriggerStoryEvent(quest.failedDialogueId);
        }

        return true;
    }

    #endregion

    #region Objective Management

    public bool UpdateObjective(string questId, string objectiveId, int amount = 1)
    {
        QuestData quest = GetActiveQuest(questId);

        if (quest == null) return false;

        QuestObjective objective = quest.GetObjective(objectiveId);

        if (objective == null || objective.isCompleted) return false;

        bool wasCompleted = objective.isCompleted;
        objective.UpdateProgress(amount);

        DebugLog($"Objective updated: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");

        // Check if objective was just completed
        if (!wasCompleted && objective.isCompleted)
        {
            OnObjectiveCompleted?.Invoke(objective, quest);
        }

        // Check if quest should be completed
        if (quest.autoComplete && quest.IsCompleted())
        {
            CompleteQuest(questId);
        }
        else
        {
            OnQuestProgressUpdated?.Invoke(quest);
        }

        return true;
    }

    public bool UpdateObjectiveByType(ObjectiveType type, string targetId, int amount = 1)
    {
        bool anyUpdated = false;

        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.objectives)
            {
                if (objective.type == type &&
                    (string.IsNullOrEmpty(targetId) || objective.targetId == targetId) &&
                    !objective.isCompleted)
                {
                    UpdateObjective(quest.questId, objective.objectiveId, amount);
                    anyUpdated = true;
                }
            }
        }

        return anyUpdated;
    }

    #endregion

    #region Quest Queries

    public QuestData GetQuestById(string questId)
    {
        return allQuests.FirstOrDefault(q => q.questId == questId);
    }

    public QuestData GetActiveQuest(string questId)
    {
        return activeQuests.FirstOrDefault(q => q.questId == questId);
    }

    public List<QuestData> GetActiveQuests()
    {
        return activeQuests.FindAll(q => q.status == QuestStatus.InProgress);
    }

    public List<QuestData> GetCompletedQuests()
    {
        return new List<QuestData>(completedQuests);
    }

    public List<QuestData> GetAvailableQuests()
    {
        return allQuests.Where(q => q.CanStart()).ToList();
    }

    public bool IsQuestActive(string questId)
    {
        return activeQuests.Any(q => q.questId == questId);
    }

    public bool IsQuestCompleted(string questId)
    {
        return completedQuests.Any(q => q.questId == questId);
    }

    public bool HasActiveQuestWithObjective(ObjectiveType type, string targetId = "")
    {
        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.objectives)
            {
                if (objective.type == type &&
                    (string.IsNullOrEmpty(targetId) || objective.targetId == targetId) &&
                    !objective.isCompleted)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Rewards

    private void GiveRewards(QuestData quest)
    {
        // EXP reward
        if (quest.expReward > 0 && PlayerLevel.instance != null)
        {
            PlayerLevel.instance.GainExp(quest.expReward);
            DebugLog($"Gained {quest.expReward} EXP from quest: {quest.questName}");
        }

        // Currency reward
        if (quest.currencyReward > 0 && PlayerManager.instance != null)
        {
            PlayerManager.instance.currency += quest.currencyReward;
            DebugLog($"Gained {quest.currencyReward} currency from quest: {quest.questName}");
        }

        // Item rewards
        if (quest.itemRewards != null && quest.itemRewards.Length > 0 && Inventory.instance != null)
        {
            foreach (var item in quest.itemRewards)
            {
                if (item != null)
                {
                    Inventory.instance.AddItem(item);
                    DebugLog($"Received item: {item.itemName} from quest: {quest.questName}");
                }
            }
        }
    }

    #endregion

    #region Save/Load System

    public void LoadData(GameData _data)
    {
        // Clear current lists
        activeQuests.Clear();
        completedQuests.Clear();
        failedQuests.Clear();

        // Load active quests
        foreach (string questId in _data.activeQuestIds)
        {
            QuestData quest = GetQuestById(questId);
            if (quest != null)
            {
                quest.status = QuestStatus.InProgress;
                activeQuests.Add(quest);
            }
        }

        // Load completed quests
        foreach (string questId in _data.completedQuestIds)
        {
            QuestData quest = GetQuestById(questId);
            if (quest != null)
            {
                quest.status = QuestStatus.Completed;
                completedQuests.Add(quest);
            }
        }

        // Load quest progress data
        foreach (var progressData in _data.questProgressData)
        {
            QuestData quest = GetQuestById(progressData.questId);
            if (quest != null)
            {
                foreach (var objProgress in progressData.objectiveProgress)
                {
                    QuestObjective objective = quest.GetObjective(objProgress.objectiveId);
                    if (objective != null)
                    {
                        objective.currentAmount = objProgress.currentAmount;
                        objective.isCompleted = objProgress.isCompleted;
                    }
                }

                if (quest.hasTimeLimit)
                {
                    quest.timeRemaining = progressData.timeRemaining;
                }
            }
        }

        DebugLog($"Loaded quest data: {activeQuests.Count} active, {completedQuests.Count} completed");
    }

    public void SaveData(ref GameData _data)
    {
        // Save active quest IDs
        _data.activeQuestIds.Clear();
        foreach (var quest in activeQuests)
        {
            _data.activeQuestIds.Add(quest.questId);
        }

        // Save completed quest IDs
        _data.completedQuestIds.Clear();
        foreach (var quest in completedQuests)
        {
            _data.completedQuestIds.Add(quest.questId);
        }

        // Save quest progress data
        _data.questProgressData.Clear();
        foreach (var quest in activeQuests)
        {
            var progressData = new QuestProgressData
            {
                questId = quest.questId,
                timeRemaining = quest.timeRemaining
            };

            foreach (var objective in quest.objectives)
            {
                progressData.objectiveProgress.Add(new ObjectiveProgressData
                {
                    objectiveId = objective.objectiveId,
                    currentAmount = objective.currentAmount,
                    isCompleted = objective.isCompleted
                });
            }

            _data.questProgressData.Add(progressData);
        }

        DebugLog($"Saved quest data: {activeQuests.Count} active, {completedQuests.Count} completed");
    }

    #endregion

    #region Debug

    private void DebugLog(string message, bool isWarning = false)
    {
        if (!enableDebugLogs) return;

        if (isWarning)
            Debug.LogWarning($"[QuestManager] {message}");
        else
            Debug.Log($"[QuestManager] {message}");
    }

    // Debug methods for testing
    [ContextMenu("Debug: List Active Quests")]
    private void DebugListActiveQuests()
    {
        Debug.Log($"Active Quests ({activeQuests.Count}):");
        foreach (var quest in activeQuests)
        {
            Debug.Log($"- {quest.questName} ({quest.GetOverallProgress():P})");
        }
    }

    [ContextMenu("Debug: List Available Quests")]
    private void DebugListAvailableQuests()
    {
        var available = GetAvailableQuests();
        Debug.Log($"Available Quests ({available.Count}):");
        foreach (var quest in available)
        {
            Debug.Log($"- {quest.questName}");
        }
    }

    #endregion

    #region Additional Quest Methods (for NPCInteraction)

    /// <summary>
    /// Kiểm tra xem có thể bắt đầu quest không (wrapper cho CanStart)
    /// </summary>
    public bool CanStartQuest(string questId)
    {
        QuestData quest = GetQuestById(questId);
        return quest != null && quest.CanStart();
    }

    /// <summary>
    /// Thử hoàn thành quest nếu tất cả objectives đã xong
    /// </summary>
    public bool TryCompleteQuest(string questId)
    {
        QuestData quest = GetActiveQuest(questId);

        if (quest == null)
        {
            DebugLog($"Cannot complete quest '{questId}': Quest not active", true);
            return false;
        }

        if (!quest.IsCompleted())
        {
            DebugLog($"Cannot complete quest '{questId}': Objectives not completed");
            return false;
        }

        return CompleteQuest(questId);
    }

    /// <summary>
    /// Kiểm tra xem quest có thể được submit/turn in không
    /// </summary>
    public bool CanTurnInQuest(string questId)
    {
        QuestData quest = GetActiveQuest(questId);
        return quest != null && quest.IsCompleted();
    }

    /// <summary>
    /// Lấy danh sách quest có thể turn in với NPC này
    /// </summary>
    public List<QuestData> GetTurnInableQuests(string[] questIds)
    {
        List<QuestData> turnInableQuests = new List<QuestData>();

        foreach (string questId in questIds)
        {
            if (CanTurnInQuest(questId))
            {
                QuestData quest = GetActiveQuest(questId);
                if (quest != null)
                    turnInableQuests.Add(quest);
            }
        }

        return turnInableQuests;
    }

    /// <summary>
    /// Lấy danh sách quest có thể nhận từ NPC này
    /// </summary>
    public List<QuestData> GetAvailableQuestsFromNPC(string[] questIds)
    {
        

        foreach (string questId in questIds)
        {
            if (CanStartQuest(questId))
            {
                QuestData quest = GetQuestById(questId);
                if (quest != null)
                    availableQuests.Add(quest);
            }
        }

        return availableQuests;
    }

    /// <summary>
    /// Abandon/hủy bỏ quest
    /// </summary>
    public bool AbandonQuest(string questId)
    {
        QuestData quest = GetActiveQuest(questId);

        if (quest == null)
        {
            DebugLog($"Cannot abandon quest '{questId}': Quest not active", true);
            return false;
        }

        activeQuests.Remove(quest);
        quest.ResetQuest();

        DebugLog($"Quest abandoned: {quest.questName}");

        // Trigger event nếu cần
        OnQuestFailed?.Invoke(quest);

        return true;
    }

    /// <summary>
    /// Restart quest (cho repeatable quests)
    /// </summary>
    public bool RestartQuest(string questId)
    {
        QuestData quest = GetQuestById(questId);

        if (quest == null || !quest.isRepeatable)
        {
            DebugLog($"Cannot restart quest '{questId}': Quest not found or not repeatable", true);
            return false;
        }

        // Remove from completed list if present
        completedQuests.Remove(quest);

        // Reset and start
        quest.ResetQuest();
        return StartQuest(questId);
    }

    #endregion

    #region Debug Methods

    [ContextMenu("Debug: Force Complete Quest")]
    private void DebugForceCompleteQuest()
    {
        if (activeQuests.Count > 0)
        {
            string questId = activeQuests[0].questId;
            var quest = activeQuests[0];

            // Force complete all objectives
            foreach (var objective in quest.objectives)
            {
                objective.SetCompleted();
            }

            TryCompleteQuest(questId);
            Debug.Log($"Force completed quest: {quest.questName}");
        }
    }

    [ContextMenu("Debug: Reset All Quests")]
    private void DebugResetAllQuests()
    {
        foreach (var quest in allQuests)
        {
            quest.ResetQuest();
        }

        activeQuests.Clear();
        completedQuests.Clear();
        failedQuests.Clear();

        Debug.Log("All quests reset!");
    }

    /// <summary>
    /// Debug method để kiểm tra trạng thái quest
    /// </summary>
    
    public void DebugQuestStatus(string questId)
    {
        QuestData quest = GetQuestById(questId);
        if (quest == null)
        {
            Debug.Log($"Quest '{questId}' not found!");
            return;
        }

        Debug.Log($"=== Quest Status: {quest.questName} ===");
        Debug.Log($"Status: {quest.status}");
        Debug.Log($"Can Start: {quest.CanStart()}");
        Debug.Log($"Is Completed: {quest.IsCompleted()}");
        Debug.Log($"Progress: {quest.GetOverallProgress():P}");

        foreach (var objective in quest.objectives)
        {
            Debug.Log($"  - {objective.description}: {objective.currentAmount}/{objective.requiredAmount} ({objective.isCompleted})");
        }
    }

    #endregion

    #region Enemy Kill Tracking

    /// <summary>
    /// Update quest objectives when enemy is killed
    /// </summary>
    public void UpdateObjectiveProgress(string objectiveType, string targetId, int amount = 1)
    {
        bool anyUpdated = false;

        foreach (QuestData quest in activeQuests)
        {
            if (quest.objectives == null) continue;

            foreach (QuestObjective objective in quest.objectives)
            {
                if (objective.isCompleted) continue;

                // Check if this objective matches the kill
                if (DoesObjectiveMatch(objective, objectiveType, targetId))
                {
                    int oldAmount = objective.currentAmount;
                    objective.UpdateProgress(amount);

                    DebugLog($"Updated objective: {objective.description} ({oldAmount} -> {objective.currentAmount})");

                    // Check if objective completed
                    if (objective.isCompleted && oldAmount < objective.requiredAmount)
                    {
                        DebugLog($"Objective completed: {objective.description}");
                        OnObjectiveCompleted?.Invoke(objective, quest);
                    }

                    anyUpdated = true;
                }
            }

            // Check if quest completed after objective update
            if (quest.IsCompleted() && quest.status == QuestStatus.InProgress)
            {
                quest.status = QuestStatus.Completed;
                DebugLog($"Quest ready to complete: {quest.questName}");
            }

            // Trigger progress update
            if (anyUpdated)
            {
                OnQuestProgressUpdated?.Invoke(quest);
            }
        }
    }

    /// <summary>
    /// Check if objective matches the kill criteria
    /// </summary>
    private bool DoesObjectiveMatch(QuestObjective objective, string objectiveType, string targetId)
    {
        // Must be kill objective
        if (objective.type.ToString().ToLower() != objectiveType.ToLower())
            return false;

        // Check target ID match
        if (objective.targetId.ToLower() == targetId.ToLower())
            return true;

        // Check if objective accepts "any" enemy
        if (objective.targetId.ToLower() == "any")
            return true;

        return false;
    }

    /// <summary>
    /// Track specific enemy kill by enemy component
    /// </summary>
    public void OnEnemyKilled(string enemyId, string enemyType)
    {
        DebugLog($"Enemy killed: {enemyId} (Type: {enemyType})");

        foreach (var quest in activeQuests)
        {
            if (quest.status != QuestStatus.InProgress) continue;

            bool questUpdated = false;

            foreach (var objective in quest.objectives)
            {
                if (DoesObjectiveMatch(objective, "kill", enemyId))
                {
                    objective.currentAmount++;
                    questUpdated = true;
                    DebugLog($"Quest progress: {quest.questName} - {objective.description}: {objective.currentAmount}/{objective.requiredAmount}");

                    if (objective.currentAmount >= objective.requiredAmount)
                    {
                        objective.isCompleted = true;
                        DebugLog($"Objective completed: {objective.description}");
                    }
                }
            }

            if (questUpdated)
            {
                // Trigger progress update event
                OnQuestProgressUpdated?.Invoke(quest);

                // Check if quest completed
                if (quest.IsCompleted() && quest.status == QuestStatus.InProgress)
                {
                    quest.status = QuestStatus.Completed;
                    DebugLog($"Quest completed: {quest.questName}");

                    // Trigger completion event
                    OnQuestCompleted?.Invoke(quest);
                }
            }
        }
    }

    #endregion
}
