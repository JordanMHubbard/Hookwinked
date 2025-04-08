using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NpcText;
    [SerializeField] private CanvasGroup NpcSpeechBox;
    [SerializeField] private TextMeshProUGUI PlayerText;
    [SerializeField] private CanvasGroup PlayerSpeechBox;
    [SerializeField] private List<DialogueEntry> AllDialogue;
    [SerializeField] private CanvasGroup ScreenFade;
    private List<Dialogue> currentDialogue;
    private int currentLineIndex = 0;
    private bool isInputPaused;

    private void Start()
    {
        Debug.Log("Today: " +GameManager.Instance.GetCurrentDay());
        if (AllDialogue.Count > GameManager.Instance.GetCurrentDay()) 
        {
            StartCoroutine(PlayCurrentDayDialogue());
        }
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

        if (currentDialogue[currentLineIndex].shouldPauseAfter) 
        {
            StartCoroutine(HandleIntermission(3f));
        }
        else
        {
            if (currentDialogue[currentLineIndex].isNPC)
            {
                StartCoroutine(TypeSentence(NpcText, currentDialogue[currentLineIndex].line));
            }
            else if (!currentDialogue[currentLineIndex].isNPC)
            {
                StartCoroutine(TypeSentence(PlayerText, currentDialogue[currentLineIndex].line));
            }
        }
    }

    private IEnumerator TypeSentence (TextMeshProUGUI textBox, string sentence)
    {
        isInputPaused = true;
        textBox.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            textBox.text += letter;
            yield return new WaitForSeconds(0.02f);
        }

        isInputPaused = false;
        currentLineIndex++;
    }

    private void HandleLastLine()
    {
        if (currentDialogue[currentLineIndex-1].speakerName == "SpiritFish")
        {
            NpcSpeechBox.DOFade(0f, 0.75f);
        }
        else if (currentDialogue[currentLineIndex-1].speakerName == "Player")
        {
            PlayerSpeechBox.DOFade(0f, 0.75f);
        }
    }


    private IEnumerator HandleIntermission(float intermissionTime)
    {   
        isInputPaused = true;
        currentLineIndex++;
        ScreenFade.DOFade(1f, 1f);
        NpcText.text = "";
        PlayerText.text = "";
        yield return new WaitForSeconds(intermissionTime);

        isInputPaused = false;
        ScreenFade.DOFade(0f, 1f);
        Debug.Log("Intermission Over");
    }

    private IEnumerator PlayCurrentDayDialogue()
    {
        currentDialogue = AllDialogue[GameManager.Instance.GetCurrentDay()].dialogueLines; 
        PlayerSpeechBox.DOFade(1f, 0.75f);
        yield return new WaitForSeconds(1f);
        
        ShowNextDiaolgue();
        yield return new WaitForSeconds(2f);
        
        NpcSpeechBox.DOFade(1f, 0.75f);
        yield return new WaitForSeconds(1f);
        
        ShowNextDiaolgue();
        yield return new WaitForSeconds(2f);

        Debug.Log("Press space to skip");
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.Dialogue);
    }   

    
}



