using UnityEngine;

[CreateAssetMenu(fileName = "MainStoryDialogue", menuName = "Story System/Main Story Dialogue Data")]
public class MainStoryDialogueData : DialogueData
{
    private void OnEnable()
    {
        if (dialogueSequences == null || dialogueSequences.Count == 0)
        {
            InitializeMainStoryDialogues();
        }
    }

    private void InitializeMainStoryDialogues()
    {
        dialogueSequences = new System.Collections.Generic.List<DialogueSequence>
        {
            // Intro sequence - Mở đầu game
            new DialogueSequence
            {
                sequenceID = "game_intro",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Narrator",
                        dialogueText = "Elaria từng là vùng đất rực rỡ ánh sáng, được bảo vệ bởi Ngọn Lửa Thiêng...",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Narrator",
                        dialogueText = "Nhưng Morveth - chiến binh ánh sáng vĩ đại nhất đã sa ngã, trở thành The Withering Flame.",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Narrator",
                        dialogueText = "Giờ đây, chỉ còn Ember - Tia Lửa Cuối Cùng có thể cứu rỗi thế giới.",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = false,
                nextSequenceID = "ember_awakening"
            },

            // Ember's awakening
            new DialogueSequence
            {
                sequenceID = "ember_awakening",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Ember",
                        dialogueText = "Morveth... tại sao? Chúng ta từng là đồng đội, từng chiến đấu cùng nhau...",
                        displayDuration = 3f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Ember",
                        dialogueText = "Nhưng ta sẽ không để anh hủy hoại Elaria. Ta sẽ mang ánh sáng trở lại!",
                        displayDuration = 3f,
                        waitForInput = true
                    }
                },
                isRepeatable = false
            }
        };
    }
}
