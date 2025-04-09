using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GainedPerkUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI perkName;
    [SerializeField] Image perkImage;

    public void InitializePerk(string name, Sprite texture)
    {
        perkName.text = name;
        perkImage.sprite = texture;
    }

}
