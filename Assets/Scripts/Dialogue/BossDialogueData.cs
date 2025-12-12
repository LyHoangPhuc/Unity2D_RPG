using UnityEngine;

[CreateAssetMenu(fileName = "BossDialogue", menuName = "Story System/Boss Dialogue Data")]
public class BossDialogueData : DialogueData
{
    private void OnEnable()
    {
        if (dialogueSequences == null || dialogueSequences.Count == 0)
        {
            InitializeBossDialogues();
        }
    }

    private void InitializeBossDialogues()
    {
        dialogueSequences = new System.Collections.Generic.List<DialogueSequence>
        {
            // Morveth encounter
            new DialogueSequence
            {
                sequenceID = "morveth_encounter",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Morveth",
                        dialogueText = "Ember... đồng đội cũ của ta. Ngươi vẫn mang theo ánh sáng yếu ớt đó sao?",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Ember",
                        dialogueText = "Morveth! Anh đã làm gì với chính mình? Đây không phải anh!",
                        displayDuration = 3f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Morveth",
                        dialogueText = "Ta đã thấy được sức mạnh thật sự. Bóng tối mạnh hơn ánh sáng nhiều lần!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Ember",
                        dialogueText = "Không! Ta sẽ giải thoát anh khỏi ma lực này và mang ánh sáng trở lại Elaria!",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = false,
                nextSequenceID = "morveth_battle_start"
            },

            new DialogueSequence
            {
                sequenceID = "morveth_battle_start",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Morveth",
                        dialogueText = "Vậy thì hãy chứng minh xem ánh sáng của ngươi có đủ mạnh không!",
                        displayDuration = 3f,
                        waitForInput = true
                    }
                },
                isRepeatable = false
            },

            // Victory dialogue
            new DialogueSequence
            {
                sequenceID = "morveth_defeat",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Morveth",
                        dialogueText = "Ember... ta... ta nhớ ra rồi... Cảm ơn ngươi đã giải thoát ta...",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Ember",
                        dialogueText = "Morveth... anh đã trở lại. Chúng ta sẽ cùng nhau khôi phục Elaria.",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = false,
                nextSequenceID = "ending_sequence"
            }
        };
    }
}
