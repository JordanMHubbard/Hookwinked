using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SpiritText;
    [SerializeField] private CanvasGroup SpiritSpeechBox;
    [SerializeField] private TextMeshProUGUI PlayerText;
    [SerializeField] private CanvasGroup PlayerSpeechBox;
    [SerializeField] private List<DialogueEntry> Dialogue;
    private List<DialogueLine> currentDialogue;
    private int currentDialogueIndex = 0;
    private int currentLineIndex = 0;
    private bool isInputPaused;

    private void Start()
    {
        StartCoroutine(DayOneDialogue());
    }

    private void Update()
    {
        if (InputManager.Instance.SkipInput && !isInputPaused)
        {
           ShowNextDiaolgue(); 
        } 
    }

    private void ShowNextDiaolgue()
    {   
        if (currentLineIndex > 0 && currentDialogue[currentLineIndex-1].isLastLine) HandleLastLine();
        
        if (currentLineIndex >= currentDialogue.Count) return;

        if (currentDialogue[currentLineIndex].speakerName == "SpiritFish")
        {
            SpiritText.text = currentDialogue[currentLineIndex].line;
        }
        else if (currentDialogue[currentLineIndex].speakerName == "Player")
        {
            PlayerText.text = currentDialogue[currentLineIndex].line;
        }
        
        if (currentDialogue[currentLineIndex].shouldPauseAfter) HandleIntermission();

        currentLineIndex++;
    }

    private void HandleLastLine()
    {
        if (currentDialogue[currentLineIndex-1].speakerName == "SpiritFish")
        {
            SpiritSpeechBox.DOFade(0f, 0.75f);
        }
        else if (currentDialogue[currentLineIndex-1].speakerName == "Player")
        {
            PlayerSpeechBox.DOFade(0f, 0.75f);
        }
    }

    private void ClearAllDialogue()
    {
        SpiritText.text = "";
        PlayerText.text = "";
    }

    private void StartCurrentDayDialogue()
    {
        int currentDay = GameManager.Instance.GetCurrentDay();
        switch (currentDay)
        {
            case 1:
                DayOneDialogue(); 
                break;
            
            case 2:
                //DayTwoDialogue();
                break;
        }
    }

    private void HandleIntermission()
    {   
        isInputPaused = true;

        int currentDay = GameManager.Instance.GetCurrentDay();
        switch (currentDay)
        {
            case 0:
                StartCoroutine(DayOnePartTwoDialogue());
                //Play black screen and effects and return player to dialogue
                break;

            case 1:
                StartCoroutine(DayOnePartTwoDialogue());
                //Play black screen and effects and return player to dialogue
                break;
            
            case 2:
                //DayTwoDialogue();
                break;
        }
    }

    private IEnumerator DayOneDialogue()
    {
        currentDialogue = Dialogue[0].dialogueLines;
        PlayerSpeechBox.DOFade(1f, 0.75f);
        yield return new WaitForSeconds(1f);
        
        ShowNextDiaolgue();
        yield return new WaitForSeconds(2f);
        
        SpiritSpeechBox.DOFade(1f, 0.75f);
        yield return new WaitForSeconds(1f);
        
        ShowNextDiaolgue();
        yield return new WaitForSeconds(2f);

        Debug.Log("Press space to skip");
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.Dialogue);
    }

    private IEnumerator DayOnePartTwoDialogue()
    {
        yield return new WaitForSeconds(2f);
        isInputPaused = false;
        Debug.Log("Woke Up");
    }

    
}

[System.Serializable]
public class DialogueEntry
{
    public string entryName;
    public List<DialogueLine> dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string line;
    public bool shouldPauseAfter;
    public bool isLastLine;
}




