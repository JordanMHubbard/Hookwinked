using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string perkName;
    [SerializeField] private string perkDescription;
    [SerializeField] private Image perkImage;
    [SerializeField] private Slider unlockSlider;
    [SerializeField] private PerkSelectionUI perkSelectionUI;
    [SerializeField] private int perkIndex;
    private float unlockSpeed = 0.3f;
    private bool isHeld = false;
    private bool isUnlocked = false;

    private void UnlockPerk()
    {
        if (unlockSlider.value < 1)
        {
            unlockSlider.value += unlockSpeed * Time.deltaTime;
            Debug.Log("slider val: " + unlockSlider.value);
        }
        else if (unlockSlider.value >= 1 && !isUnlocked)
        {
            Debug.Log("Is unlocked");
            isUnlocked = true;
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
        if (!isUnlocked && unlockSlider.value > 0) unlockSlider.value -= unlockSpeed * Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ToolTipManager.Instance.gameObject.SetActive(true);
        ToolTipManager.Instance.SetAndShowToolTip(perkName, perkDescription);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ToolTipManager.Instance.gameObject.SetActive(false);
        ToolTipManager.Instance.HideToolTip();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            isHeld = true;
            Debug.Log("Left mouse button pressed down.");
        }
    }
    
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            isHeld = false;
            Debug.Log("Left mouse button released.");
        }
    }
}
