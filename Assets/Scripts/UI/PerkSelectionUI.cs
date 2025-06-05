using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PerkSelectionUI : MonoBehaviour
{
    private List<PerkInfo> perkList;
    [SerializeField] private List<CanvasGroup> perkImages;
    private int totalShipFragments;

    private void Awake()
    {
        GameManager.Instance.PerkUpgrades = this;
        totalShipFragments = GameManager.Instance.GetBoatFragmentsCount();
        perkList = GameManager.Instance.GetPerkList();
        InitalizePerks();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
            }
        }
    }
}
