using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTMP;
    [SerializeField] private CanvasGroup textGroup;
    public enum DeathType {Hooked, Exhaustion}
    public DeathType causeOfDeath {get; private set;}
    private static readonly string[] hookedMessages = new string[] {
        "YOU WERE PACKAGED AND SOLD TO THE LOCAL SUPERMARKET",
        "YOU WERE MADE INTO SOME DELICIOUS FISH AND CHIPS",
        "YOU JOINED SOME OLD FRIENDS AT MY AUNT'S FISH FRY",
        "HEY, AT LEAST THE FISHERMAN THINKS YOU'RE A CATCH",
        "IF THIS WERE A PIXAR MOVIE, YOU'D HAVE PLOT ARMOR. UNFORTUNATELY, IT'S NOT",
        "YOU ENDED UP AS AN UNDERCOOKED DISASTER ON A GORDON RAMSAY REALITY SHOW"
    };

    private static readonly string[] exhaustionMessages = new string[] {
        "YOU RAN OUT OF ENERGY..."
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
    }
}
