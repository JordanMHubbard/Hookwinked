using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PerkSelectionUI : MonoBehaviour
{
    private List<PerkInfo> perkList = new List<PerkInfo>
    {
        { new PerkInfo("NitroFish", "Longer Speed Boost", 3) },
        { new PerkInfo("Ocean's Endurance", "Slower energy depletion", 3) },
        { new PerkInfo("Coral-lateral Damage", "Shoot rocks faster and deal more damage", 3) },
        { new PerkInfo("Silent Assassin", "Prey's detection range gets smaller", 3) }
    };
    [SerializeField] private List<CanvasGroup> perkImages;
    private int totalShipFragments;

    private void Awake()
    {
        GameManager.Instance.PerkUpgrades = this;
        totalShipFragments = GameManager.Instance.GetShipFragmentsCount();
        SaveSystem.Load();

        totalShipFragments = 10;

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
            GameManager.Instance.SetShipFragmentsCount(totalShipFragments);
            SaveSystem.Save();
        }

    }
    public void UnlockPerk(int index)
    {
        perkList[index].isUnlocked = true;
        perkImages[index].DOFade(1f, 0.5f);
    }

    #region Save and Load

    public void Save(ref PerkSaveData data)
    {
        data.perkInfos = perkList;
    }

    public void Load(PerkSaveData data)
    {
        perkList = data.perkInfos;
        for (int i = 0; i < perkList.Count; i++)
        {
            if (perkList[i].isUnlocked)
            {
                UnlockPerk(i);
            }
        }
    }

    #endregion
}

[System.Serializable]
public struct PerkSaveData
{
    public List<PerkInfo> perkInfos;
    
}
