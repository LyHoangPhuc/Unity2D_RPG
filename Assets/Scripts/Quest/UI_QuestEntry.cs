using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_QuestEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questProgressText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image statusIcon;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image backgroundImage;

    [Header("Status Colors")]
    [SerializeField] private Color inProgressColor = Color.yellow;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color selectedColor = new Color(0.2f, 0.4f, 0.8f, 0.8f);
    [SerializeField] private Color normalColor = new Color(0.16f, 0.16f, 0.16f, 0.8f);

    private QuestData questData;
    private UI_QuestTracker questTracker;
    private bool isSelected = false;

    private void Awake()
    {
        // Auto-assign components if not assigned
        if (questNameText == null)
            questNameText = transform.Find("QuestInfo/QuestName")?.GetComponent<TextMeshProUGUI>();

        if (questProgressText == null)
            questProgressText = transform.Find("QuestInfo/QuestProgress")?.GetComponent<TextMeshProUGUI>();

        if (progressBar == null)
            progressBar = transform.Find("QuestInfo/ProgressBar")?.GetComponent<Slider>();

        if (statusIcon == null)
            statusIcon = transform.Find("StatusIcon")?.GetComponent<Image>();

        if (selectButton == null)
            selectButton = GetComponent<Button>();

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
    }

    public void SetupQuestEntry(QuestData quest, UI_QuestTracker tracker)
    {
        questData = quest;
        questTracker = tracker;

        UpdateDisplay();

        // Setup button click
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => {
                if (questTracker != null && questData != null)
                {
                    questTracker.SelectQuest(questData);
                }
            });
        }
    }

    public void UpdateProgress()
    {
        UpdateDisplay();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBackgroundColor();
    }

    private void UpdateDisplay()
    {
        if (questData == null) return;

        // Update quest name
        if (questNameText != null)
            questNameText.text = questData.questName;

        // Calculate progress
        int totalObjectives = questData.objectives.Count;
        int completedObjectives = 0;
        int totalProgress = 0;
        int currentProgress = 0;

        foreach (var objective in questData.objectives)
        {
            if (objective.isCompleted)
                completedObjectives++;

            totalProgress += objective.requiredAmount;
            currentProgress += objective.currentAmount;
        }

        // Update progress text
        if (questProgressText != null)
        {
            if (questData.status == QuestStatus.Completed)
            {
                questProgressText.text = "HOÀN THÀNH";
                questProgressText.color = completedColor;
            }
            else
            {
                questProgressText.text = $"{currentProgress}/{totalProgress}";
                questProgressText.color = inProgressColor;
            }
        }

        // Update progress bar
        if (progressBar != null)
        {
            float progressPercentage = totalProgress > 0 ? (float)currentProgress / totalProgress : 0f;
            progressBar.value = progressPercentage;

            // Update progress bar colors
            var fillImage = progressBar.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = questData.status == QuestStatus.Completed ? completedColor : inProgressColor;
            }
        }

        // Update status icon
        if (statusIcon != null)
        {
            statusIcon.color = questData.status == QuestStatus.Completed ? completedColor : inProgressColor;
        }

        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            if (isSelected)
            {
                backgroundImage.color = selectedColor;
            }
            else
            {
                backgroundImage.color = normalColor;
            }
        }
    }

    public QuestData GetQuest()
    {
        return questData;
    }

    // For debugging
    private void OnValidate()
    {
        if (Application.isPlaying && questData != null)
        {
            UpdateDisplay();
        }
    }
}
