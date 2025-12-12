using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName;
    [SerializeField] private string initialDialogueID;
    [SerializeField] private string repeatDialogueID;
    [SerializeField] private bool canInteract = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Quest Integration")]
    [SerializeField] private string[] questsToGive;
    [SerializeField] private string[] questsToComplete;

    private bool playerInRange;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        // Chỉ cho phép tương tác khi không bị block input
        if (playerInRange && canInteract &&
            InputManager.instance != null && InputManager.instance.CanReceiveInput() &&
            Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    private void Interact()
    {
        // Debug checks
        Debug.Log("Interact called");

        if (StoryManager.instance == null)
        {
            Debug.LogError("StoryManager.instance is null!");
            return;
        }

        // Check for quest completion first
        CheckQuestCompletion();

        // Check for new quests
        CheckQuestGiving();

        string dialogueToPlay = GetDialogueID();
        Debug.Log($"Dialogue to play: {dialogueToPlay}");

        if (!string.IsNullOrEmpty(dialogueToPlay))
        {
            StoryManager.instance.TriggerStoryEvent(dialogueToPlay);
        }
        else
        {
            Debug.LogWarning("No dialogue ID to play!");
        }
    }

    private void CheckQuestCompletion()
    {
        if (QuestManager.instance == null) return;

        foreach (string questId in questsToComplete)
        {
            if (string.IsNullOrEmpty(questId)) continue;

            // Kiểm tra quest có thể turn in không
            if (QuestManager.instance.CanTurnInQuest(questId))
            {
                bool completed = QuestManager.instance.TryCompleteQuest(questId);
                if (completed)
                {
                    Debug.Log($"Quest completed via NPC: {questId}");
                    // Có thể thêm visual feedback ở đây
                }
            }
        }
    }

    private void CheckQuestGiving()
    {
        if (QuestManager.instance == null) return;

        foreach (string questId in questsToGive)
        {
            if (string.IsNullOrEmpty(questId)) continue;

            if (QuestManager.instance.CanStartQuest(questId))
            {
                bool started = QuestManager.instance.StartQuest(questId);
                if (started)
                {
                    Debug.Log($"Quest started via NPC: {questId}");
                    break; // Only give one quest at a time
                }
            }
        }
    }

    private string GetDialogueID()
    {
        Debug.Log($"Initial Dialogue ID: {initialDialogueID}");
        Debug.Log($"Repeat Dialogue ID: {repeatDialogueID}");

        // If initial dialogue hasn't been completed, play it
        if (!string.IsNullOrEmpty(initialDialogueID) && !StoryManager.instance.IsSequenceCompleted(initialDialogueID))
        {
            return initialDialogueID;
        }
        // Otherwise play repeat dialogue
        else if (!string.IsNullOrEmpty(repeatDialogueID))
        {
            return repeatDialogueID;
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            playerInRange = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            playerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
}
