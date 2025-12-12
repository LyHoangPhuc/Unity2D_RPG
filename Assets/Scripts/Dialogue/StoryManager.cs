using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour, ISaveManager
{
    [Header("Story Data")]
    [SerializeField] private List<DialogueData> allDialogueData;

    private HashSet<string> completedSequences = new HashSet<string>();
    private Dictionary<string, DialogueData> dialogueDatabase = new Dictionary<string, DialogueData>();

    public static StoryManager instance;

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
        }
    }

    private void Start()
    {
        BuildDialogueDatabase();
    }

    private void BuildDialogueDatabase()
    {
        dialogueDatabase.Clear();

        foreach (DialogueData data in allDialogueData)
        {
            foreach (DialogueSequence sequence in data.dialogueSequences)
            {
                if (!dialogueDatabase.ContainsKey(sequence.sequenceID))
                {
                    dialogueDatabase.Add(sequence.sequenceID, data);
                }
            }
        }
    }

    public void TriggerStoryEvent(string sequenceID)
    {
        // Kiểm tra UI_DialogueSystem instance
        if (UI_DialogueSystem.instance == null)
        {
            Debug.LogError("UI_DialogueSystem instance is null! Make sure UI_DialogueSystem is in the scene and properly initialized.");
            return;
        }

        if (dialogueDatabase.ContainsKey(sequenceID))
        {
            DialogueData data = dialogueDatabase[sequenceID];
            DialogueSequence sequence = data.GetSequence(sequenceID);

            if (sequence != null)
            {
                // Check if sequence is repeatable or not completed yet
                if (sequence.isRepeatable || !completedSequences.Contains(sequenceID))
                {
                    UI_DialogueSystem.instance.StartDialogue(sequence);
                }
                else
                {
                    Debug.Log($"Sequence '{sequenceID}' has already been completed and is not repeatable.");
                }
            }
            else
            {
                Debug.LogError($"Sequence '{sequenceID}' not found in DialogueData!");
            }
        }
        else
        {
            Debug.LogWarning($"Sequence ID '{sequenceID}' not found in dialogue database!");
        }
    }

    public void MarkSequenceAsCompleted(string sequenceID)
    {
        completedSequences.Add(sequenceID);
    }

    public bool IsSequenceCompleted(string sequenceID)
    {
        return completedSequences.Contains(sequenceID);
    }

    // Save/Load System Integration
    public void LoadData(GameData _data)
    {
        completedSequences.Clear();
        foreach (string sequenceID in _data.completedStorySequences)
        {
            completedSequences.Add(sequenceID);
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.completedStorySequences.Clear();
        foreach (string sequenceID in completedSequences)
        {
            _data.completedStorySequences.Add(sequenceID);
        }
    }
}
