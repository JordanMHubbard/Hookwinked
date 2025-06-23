using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PerkSelectionUI : MonoBehaviour
{
    private List<PerkInfo> perkList;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<CanvasGroup> perkImages;
    [SerializeField] private List<Slider> unlockSliders;
    private int totalShipFragments;

    private void OnEnable()
    {
        GameManager.Instance.PerkUpgrades = this;
        totalShipFragments = GameManager.Instance.GetBoatFragmentsCount();
        perkList = GameManager.Instance.GetPerkList();
        InitalizePerks();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(FadeIn());
    }

    private void Start()
    {
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.UI);
    }

    // Perk Unlocks
    public void BuyPerk(int index)
    {
        if (perkList[index].fragmentCost < totalShipFragments && !perkList[index].isUnlocked)
        {
            UnlockPerk(index);
            totalShipFragments -= perkList[index].fragmentCost;
            GameManager.Instance.SetBoatFragmentsCount(totalShipFragments);
            SaveSystem.Save();
        }

    }
    public void UnlockPerk(int index)
    {
        perkList[index].isUnlocked = true;
        perkImages[index].alpha = 1f;
    }

    private void InitalizePerks()
    {
        for (int i = 0; i < perkList.Count; i++)
        {
            if (perkList[i].isUnlocked)
            {
                perkImages[i].alpha = 1f;
                unlockSliders[i].value = 1f;
            }
        }
    }

    public void Continue()
    {
        GameManager.Instance.IncrementCurrentDay();
        SceneManager.LoadScene("TheReef");
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(2.5f);

        if (canvasGroup) canvasGroup.DOFade(1f, 1f);
    }
}
