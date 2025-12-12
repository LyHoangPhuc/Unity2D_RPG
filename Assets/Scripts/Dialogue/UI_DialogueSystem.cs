using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;

    [Header("Settings")]
    [SerializeField] private float typewriterSpeed = 0.05f;

    private DialogueSequence currentSequence;
    private int currentLineIndex;
    private bool isTyping;
    private bool isDialogueActive;

    public static UI_DialogueSystem instance;
    private Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ UI qua các scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Đảm bảo panel bị tắt ban đầu
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Setup button events
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipDialogue);

        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
        {
            player = PlayerManager.instance.player;
        }
;
    }

    private void Update()
    {
        // Chỉ xử lý input dialogue khi dialogue active
        if (isDialogueActive && InputManager.instance != null && InputManager.instance.CanReceiveDialogueInput())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                    SkipTypewriter();
                else
                    NextLine();
            }
        }
    }

    public void StartDialogue(DialogueSequence sequence)
    {
        if (sequence == null || sequence.dialogueLines.Count == 0)
            return;

        currentSequence = sequence;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);

        // Set player dialogue state và block input
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
        {
            PlayerManager.instance.player.SetDialogueState(true);
        }

        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentLineIndex >= currentSequence.dialogueLines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = currentSequence.dialogueLines[currentLineIndex];

        characterNameText.text = currentLine.characterName;
        characterImage.sprite = currentLine.characterSprite;

        StartCoroutine(TypewriterEffect(currentLine.dialogueText));
    }

    private IEnumerator TypewriterEffect(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
    }

    private void SkipTypewriter()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSequence.dialogueLines[currentLineIndex].dialogueText;
            isTyping = false;
        }
    }

    public void NextLine()
    {
        if (isTyping)
        {
            SkipTypewriter();
            return;
        }

        currentLineIndex++;
        DisplayCurrentLine();
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Release player from dialogue state và unblock input
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
        {
            PlayerManager.instance.player.SetDialogueState(false);
        }

        // Check if there's a next sequence
        if (!string.IsNullOrEmpty(currentSequence.nextSequenceID))
        {
            StoryManager.instance.TriggerStoryEvent(currentSequence.nextSequenceID);
        }

        StoryManager.instance.MarkSequenceAsCompleted(currentSequence.sequenceID);
    }
}
