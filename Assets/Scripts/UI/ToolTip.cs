using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string perkName;
    [SerializeField] private string perkDescription;
    [SerializeField] private Image perkImage;
    [SerializeField] private Slider unlockSlider;
    [SerializeField] private PerkSelectionUI perkSelectionUI;
    [SerializeField] private int perkIndex;
    [SerializeField] private AudioClip riserSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip unlockSound;
    private GameObject riserObject;
    private float unlockSpeed = 0.4f;
    private bool isHeld = false;
    private bool isUnlocked = false;
    private bool isToolTipActive;

    private void UnlockPerk()
    {
        if (unlockSlider.value < 1)
        {
            unlockSlider.value += unlockSpeed * Time.deltaTime;
            Debug.Log("slider val: " + unlockSlider.value);
        }
        else if (unlockSlider.value >= 1 && !isUnlocked)
        {
            if (!perkSelectionUI.CanBuyPerk(perkIndex)) return;

            Debug.Log("Is unlocked");
            isUnlocked = true;
            perkImage.DOColor(Color.white, 0.1f);
            Destroy(riserObject);
            UISoundFXManager.Instance.PlayClickSound();
            SoundFXManager.Instance.PlaySoundFXClip(unlockSound, transform.position, 1f, 0f);
            perkSelectionUI.BuyPerk(perkIndex);
        }
    }

    private void Awake()
    {
        if (GameManager.Instance.GetPerkList()[perkIndex].isUnlocked)
        {
            isUnlocked = true;
        }
    }

    private void Update()
    {
        if (isHeld)
        {
            UnlockPerk();
        }
        else
        {
            StopUnlockingPerk();
        }
    }

    private void StopUnlockingPerk()
    {
        if (!isUnlocked && unlockSlider.value > 0) unlockSlider.value -= unlockSpeed * 2 * Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!isToolTipActive)
        {
            ToolTipManager.Instance.SetAndShowToolTip(perkName, perkDescription);
            if  (!isUnlocked) perkImage.DOColor(Color.gray, 0.1f);
            isToolTipActive = true;
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (isToolTipActive)
        {
            ToolTipManager.Instance.HideToolTip();
            if  (!isUnlocked) perkImage.DOColor(Color.white, 0.1f);
            isToolTipActive = false;
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            if (!perkSelectionUI.CanBuyPerk(perkIndex))
            {
                SoundFXManager.Instance.PlaySoundFXClip(errorSound, transform.position, 0.75f, 0f);
                return;
            }

            Debug.Log("Left mouse button pressed down.");
            isHeld = true;
            riserObject = SoundFXManager.Instance.LoopSoundFXClip(riserSound, transform.position, 0.5f, 0f);
        }
    }
    
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            if (!isHeld) return;

            isHeld = false;
            Debug.Log("Left mouse button released.");
            Destroy(riserObject);
        }
    }
}
