using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GainedPerkUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI perkName;
    [SerializeField] TextMeshProUGUI perkDescription;
    [SerializeField] Image perkImage;
    [SerializeField] CanvasGroup titleAnnouncementCanvas;
    [SerializeField] CanvasGroup imageCanvas;
    [SerializeField] CanvasGroup nameCanvas;
    [SerializeField] CanvasGroup descriptionCanvas;
    [SerializeField] AudioClip unlockSound;

    private void Start()
    {
        StartCoroutine(RevealPerk());
    }

    public void InitializePerk(string name, string description, Sprite icon)
    {
        perkName.text = name;
        perkImage.sprite = icon;
        perkDescription.text = description;
    }

    private IEnumerator RevealPerk()
    {
        titleAnnouncementCanvas.DOFade(1f, 0.75f);
        yield return new WaitForSeconds(1f);

        imageCanvas.DOFade(1f, 0.5f);
        nameCanvas.DOFade(1f, 0.5f);
        SoundFXManager.Instance.PlaySoundFXClip(unlockSound, null, transform.position, 1f, 0f);
        yield return new WaitForSeconds(1f);

        descriptionCanvas.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(4f);

        titleAnnouncementCanvas.DOFade(0f, 1f);
        imageCanvas.DOFade(0f, 1f);
        nameCanvas.DOFade(0f, 1f);
        descriptionCanvas.DOFade(0f, 1f);
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }

    

}
