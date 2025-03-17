using TMPro;
using UnityEngine;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;
    private static readonly string[] messages = new string[] {
        "YOU WERE PACKAGED AND SOLD TO THE LOCAL SUPERMARKET",
        "YOU WERE MADE INTO SOME DELICIOUS FISH AND CHIPS",
        "YOU JOINED SOME OLD FRIENDS AT MY AUNT'S FISH FRY",
        "HEY, AT LEAST THE FISHERMAN THINKS YOU'RE A CATCH",
        "IF THIS WERE A PIXAR MOVIE, YOU'D HAVE PLOT ARMOR. UNFORTUNATELY, IT'S NOT",
        "YOU ENDED UP AS AN UNDERCOOKED DISASTER ON A GORDON RAMSAY REALITY SHOW"
    };

    private void OnEnable()
    {
        if (Text == null) return;
        Text.text = ChooseRandomMessage();
    }

    private string ChooseRandomMessage()
    {
        int index = Random.Range(0, messages.Length - 1);
        return messages[index];
    }
}
