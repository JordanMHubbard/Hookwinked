using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string entryName;
    public List<Dialogue> dialogueLines;
}

[System.Serializable]
public class Dialogue
{
    public string speakerName;
    [TextArea(2 , 10)]
    public string line;
    public bool shouldPauseAfter;
    public bool isLastLine;
    public bool isNPC;
}
