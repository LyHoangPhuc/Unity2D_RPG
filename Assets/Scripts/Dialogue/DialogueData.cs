using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(3, 5)]
    public string dialogueText;
    public Sprite characterSprite;
    public float displayDuration = 2f;
    public bool waitForInput = true;
}

[System.Serializable]
public class DialogueSequence
{
    public string sequenceID;
    public List<DialogueLine> dialogueLines;
    public bool isRepeatable = true;
    public string nextSequenceID; // Để chain các dialogue
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Story System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueSequence> dialogueSequences;

    public DialogueSequence GetSequence(string sequenceID)
    {
        return dialogueSequences.Find(seq => seq.sequenceID == sequenceID);
    }
}
