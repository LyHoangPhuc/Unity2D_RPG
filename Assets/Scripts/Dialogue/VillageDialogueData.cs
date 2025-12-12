using UnityEngine;

[CreateAssetMenu(fileName = "VillageDialogue", menuName = "Story System/Village Dialogue Data")]
public class VillageDialogueData : DialogueData
{
    // Constructor để setup sẵn các dialogue cho làng
    private void OnEnable()
    {
        if (dialogueSequences == null || dialogueSequences.Count == 0)
        {
            InitializeVillageDialogues();
        }
    }

    private void InitializeVillageDialogues()
    {
        dialogueSequences = new System.Collections.Generic.List<DialogueSequence>
        {
            // Village Elder - Trưởng làng
            new DialogueSequence
            {
                sequenceID = "elder_intro",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Trưởng làng Aldric",
                        dialogueText = "Ember! Con đã trở về... Làng ta cần con lắm. Từ khi Morveth sa ngã, bóng tối đã bao trùm mọi nơi.",
                        displayDuration = 3f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Trưởng làng Aldric",
                        dialogueText = "Con là tia hy vọng cuối cùng của chúng ta. Ngọn Lửa Thiêng phải được khôi phục!",
                        displayDuration = 3f,
                        waitForInput = true
                    }
                },
                isRepeatable = false,
                nextSequenceID = "elder_quest"
            },

            new DialogueSequence
            {
                sequenceID = "elder_quest",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Trưởng làng Aldric",
                        dialogueText = "Hãy bảo vệ làng khỏi những quái vật và tìm cách đến được tháp Elaria. Đó là nơi Morveth đang giữ Ngọn Lửa Thiêng.",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = true
            }, 

            // Hướng dẫn cơ bản
            new DialogueSequence
            {
                sequenceID = "tutorial_basic_controls",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Chào mừng đến với Elaria, chiến binh trẻ! Ta sẽ hướng dẫn con những kỹ năng cơ bản.",
                        displayDuration = 3f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Sử dụng phím A/D để di chuyển. SPACE để nhảy.",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = true,
            },

            // Hướng dẫn kỹ năng nâng cao
            new DialogueSequence
            {
                sequenceID = "tutorial_wall_jump",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "ta sẽ dạy con những kỹ năng nâng cao hơn.",
                        displayDuration = 3f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Khi ở gần địa hình cao, SPACE để wall jump. Hữu ích để leo lên cao!",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = true,
            },

            // Hướng dẫn chiến đấu
            new DialogueSequence
            {
                sequenceID = "tutorial_combat_tips",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = " Mouse trái hoặc nút J để đánh. Con có thể combo các đòn đánh cách liên tiếp!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Không phải lúc nào cũng đánh kẻ dịch  mù quáng. Hãy quan sát kẻ thù!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Mỗi loại quái vật có pattern đánh khác nhau. Học cách né tránh!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Sử dụng combo của bạn để gây sát thương tối đa. Timing là chìa khóa!",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = true,
            },
            // Lời khuyên chung
            new DialogueSequence
            {
                sequenceID = "tutorial_tips",
                dialogueLines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Một số lời khuyên cuối: Luôn chuẩn bị đầy đủ trước khi vào dungeon!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Checkpoint sẽ lưu tiến độ của con. Tìm và kích hoạt chúng!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Đừng ngại thử nghiệm các combo đòn đánh và kỹ năng khác nhau!",
                        displayDuration = 4f,
                        waitForInput = true
                    },
                    new DialogueLine
                    {
                        characterName = "Anna",
                        dialogueText = "Chúc con may mắn trong hành trình cứu rỗi Elaria, chiến binh trẻ!",
                        displayDuration = 4f,
                        waitForInput = true
                    }
                },
                isRepeatable = true
            }

        };
    }
}
