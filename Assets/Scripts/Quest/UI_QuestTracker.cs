using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UI_QuestTracker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private TextMeshProUGUI questDetailsTitle;
    [SerializeField] private TextMeshProUGUI questDetailsDescription;
    [SerializeField] private Transform objectivesListParent;
    [SerializeField] private GameObject objectiveEntryPrefab;

    [Header("Notification")]
    [SerializeField] private GameObject questNotificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float notificationDuration = 3f;

    [Header("Controls")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;

    private List<UI_QuestEntry> questEntries = new List<UI_QuestEntry>();
    private QuestData selectedQuest;

    private void Start()
    {
        // Subscribe to QuestManager events
        if (QuestManager.instance != null)
        {
            QuestManager.instance.OnQuestStarted += OnQuestStarted;
            QuestManager.instance.OnQuestCompleted += OnQuestCompleted;
            QuestManager.instance.OnQuestProgressUpdated += OnQuestProgressUpdated;
        }

        // Hide panels initially
        if (questPanel != null)
            questPanel.SetActive(false);

        if (questNotificationPanel != null)
            questNotificationPanel.SetActive(false);

        // Clear quest details initially
        ClearQuestDetails();
    }

    private void Update()
    {
        // Toggle quest panel with key
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleQuestPanel();
        }
    }

    public void ToggleQuestPanel()
    {
        if (questPanel != null)
        {
            bool isActive = !questPanel.activeSelf;
            questPanel.SetActive(isActive);

            if (isActive)
            {
                RefreshQuestList();
            }
        }
    }

    public void RefreshQuestList()
    {
        // Clear existing entries
        foreach (var entry in questEntries)
        {
            if (entry != null && entry.gameObject != null)
                Destroy(entry.gameObject);
        }
        questEntries.Clear();

        // Create new entries for active quests
        if (QuestManager.instance != null)
        {
            var activeQuests = QuestManager.instance.GetActiveQuests();
            foreach (var quest in activeQuests)
            {
                CreateQuestEntry(quest);
            }

            // If no quest selected or selected quest is not active, select first one
            if (selectedQuest == null || !activeQuests.Contains(selectedQuest))
            {
                if (activeQuests.Count > 0)
                {
                    SelectQuest(activeQuests[0]);
                }
                else
                {
                    ClearQuestDetails();
                }
            }
        }
    }

    private void CreateQuestEntry(QuestData quest)
    {
        if (questEntryPrefab == null || questListContent == null)
        {
            Debug.LogError("QuestEntryPrefab or QuestListContent is null!");
            return;
        }

        GameObject entryObj = Instantiate(questEntryPrefab, questListContent);
        UI_QuestEntry entry = entryObj.GetComponent<UI_QuestEntry>();

        if (entry != null)
        {
            entry.SetupQuestEntry(quest, this);
            questEntries.Add(entry);
        }
        else
        {
            Debug.LogError("QuestEntryPrefab doesn't have UI_QuestEntry component!");
        }
    }

    public void SelectQuest(QuestData quest)
    {
        selectedQuest = quest;
        UpdateQuestDetails();

        // Update visual selection in quest entries
        foreach (var entry in questEntries)
        {
            entry.SetSelected(entry.GetQuest() == quest);
        }
    }

    private void UpdateQuestDetails()
    {
        if (selectedQuest == null)
        {
            ClearQuestDetails();
            return;
        }

        if (questDetailsTitle != null)
            questDetailsTitle.text = selectedQuest.questName;

        if (questDetailsDescription != null)
            questDetailsDescription.text = selectedQuest.questDescription;

        UpdateObjectivesList();
    }

    private void UpdateObjectivesList()
    {
        if (objectivesListParent == null) return;

        // Clear old objectives
        foreach (Transform child in objectivesListParent)
        {
            Destroy(child.gameObject);
        }

        // Create objective entries
        if (selectedQuest != null)
        {
            foreach (var objective in selectedQuest.objectives)
            {
                CreateObjectiveEntry(objective);
            }
        }
    }

    private void CreateObjectiveEntry(QuestObjective objective)
    {
        if (objectiveEntryPrefab == null) return;

        GameObject objEntry = Instantiate(objectiveEntryPrefab, objectivesListParent);
        TextMeshProUGUI objText = objEntry.GetComponent<TextMeshProUGUI>();

        if (objText != null)
        {
            string status = objective.isCompleted ? "✓" : "○";
            string progress = $"{objective.currentAmount}/{objective.requiredAmount}";
            objText.text = $"{status} {objective.description} ({progress})";

            // Color coding
            objText.color = objective.isCompleted ? Color.green : Color.white;
        }
    }

    private void ClearQuestDetails()
    {
        if (questDetailsTitle != null)
            questDetailsTitle.text = "Chọn một nhiệm vụ";

        if (questDetailsDescription != null)
            questDetailsDescription.text = "Chọn nhiệm vụ từ danh sách để xem chi tiết";

        // Clear objectives
        if (objectivesListParent != null)
        {
            foreach (Transform child in objectivesListParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Event handlers
    private void OnQuestStarted(QuestData quest)
    {
        ShowNotification($"Nhiệm vụ mới: {quest.questName}");
        if (questPanel != null && questPanel.activeSelf)
        {
            RefreshQuestList();
        }
    }

    private void OnQuestCompleted(QuestData quest)
    {
        ShowNotification($"🎉 Hoàn thành: {quest.questName}");
        if (questPanel != null && questPanel.activeSelf)
        {
            RefreshQuestList();
        }
    }

    private void OnQuestProgressUpdated(QuestData quest)
    {
        // Update specific quest entry
        var entry = questEntries.Find(e => e != null && e.GetQuest() == quest);
        if (entry != null)
        {
            entry.UpdateProgress();
        }

        // Update details if this quest is selected
        if (selectedQuest == quest)
        {
            UpdateObjectivesList();
        }
    }

    public void ShowNotification(string message)
    {
        if (questNotificationPanel != null && notificationText != null)
        {
            questNotificationPanel.SetActive(true);
            notificationText.text = message;

            // Cancel previous invoke if exists
            CancelInvoke(nameof(HideNotification));

            // Auto hide after duration
            Invoke(nameof(HideNotification), notificationDuration);
        }
    }

    private void HideNotification()
    {
        if (questNotificationPanel != null)
            questNotificationPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (QuestManager.instance != null)
        {
            QuestManager.instance.OnQuestStarted -= OnQuestStarted;
            QuestManager.instance.OnQuestCompleted -= OnQuestCompleted;
            QuestManager.instance.OnQuestProgressUpdated -= OnQuestProgressUpdated;
        }
    }

    // Public methods for testing
    [ContextMenu("Test Notification")]
    public void TestNotification()
    {
        ShowNotification("Test notification message!");
    }

    [ContextMenu("Refresh Quest List")]
    public void TestRefreshQuestList()
    {
        RefreshQuestList();
    }
}
