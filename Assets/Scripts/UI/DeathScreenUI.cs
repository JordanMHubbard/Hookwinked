using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTMP;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private CanvasGroup restartGroup;
    [SerializeField] private CanvasGroup exitGroup;
    [SerializeField] private AudioClip failSound;
    public enum DeathType { Hooked, Exhaustion }
    public DeathType causeOfDeath {get; private set;}
    private static readonly string[] hookedMessages = new string[] {
        "You were packaged and sold to the local supermarket",
        "You were made into some delicious fish and chips",
        "You joined some old friends at my aunt's fish fry",
        "Hey, at least the fisherman thinks you're a catch",
        "Being the main character won't protect you from yourself. Maybe don't try to eat prey that is obviously plastic next time!",
        "Look on the bright side — you made it to Gordon Ramsay's kitchen! Sure, you were just a mediocre dish on one of his shows, but hey, it's something!"
    };

    private static readonly string[] exhaustionMessages = new string[] {
        "You died..."
    };

    private void OnEnable()
    {
        StartCoroutine(FadeText());
    }

    public void ChooseRandomMessage(DeathType causeOfDeath)
    {
        if (textTMP == null) return;
        int index;

        switch (causeOfDeath)
        {
            case DeathType.Hooked:
                index = Random.Range(0, hookedMessages.Length - 1);
                textTMP.text = hookedMessages[index];
                break;
            
            case DeathType.Exhaustion:
                index = 0;
                textTMP.text = exhaustionMessages[index];
                break;
        }
    }

    private IEnumerator FadeText()
    {
        yield return new WaitForSeconds(1.5f);
        textGroup.DOFade(1f, 2f);
        UISoundFXManager.Instance.PlaySoundFXClip(failSound, null, transform.position, 0.8f, 0f);

        yield return new WaitForSeconds(1f);
        StartCoroutine(ButtonsAnim());
    }

    private IEnumerator ButtonsAnim()
    {
        if (restartGroup != null) restartGroup.DOFade(1f, 1f);
        if (exitGroup != null) exitGroup.DOFade(1f, 1f);
        yield return new WaitForSeconds(1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartDay()
    {
        GameManager.Instance.IncrementNumDayRetries();
        SceneManager.LoadScene("TheReef");
    }
    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
