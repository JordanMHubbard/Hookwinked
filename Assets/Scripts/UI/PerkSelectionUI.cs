using System.Collections.Generic;
using UnityEngine;

public class PerkSelectionUI : MonoBehaviour
{
    private Dictionary<string, bool> perksAcquired = new Dictionary<string, bool>
    {
        { "NF", false },
        { "OE", false },
        { "OE2", false },
        { "CD", false },
        { "SS", false }
    };

    private int totalShipFragments;

    private void Awake()
    {
        GameManager.Instance.PerkUpgrades = this;
        totalShipFragments = GameManager.Instance.GetShipFragmentsCount();
        //SaveSystem.SaveData.PlayerDa
    }

    // Perk Unlocks

    public void UnlockNF()
    {
        if (totalShipFragments < 3) return;

        perksAcquired["NF"] = true;
        GameManager.Instance.SetShipFragmentsCount(totalShipFragments);
        //Enable NitroFish script
    }

    public void UnlockOE(bool isOE2Unlocked)
    {
        if (isOE2Unlocked) 
        {
            if (totalShipFragments < 5) return;
            
            perksAcquired["OE2"] = true;
            GameManager.Instance.SetShipFragmentsCount(totalShipFragments);
            //Enable OE script
        }
        else
        {
            if (totalShipFragments < 3) return;
            
            perksAcquired["OE"] = true;  
            GameManager.Instance.SetShipFragmentsCount(totalShipFragments);
        }
    }

    public void UnlockCD()
    {
        if (totalShipFragments < 3) return;

        perksAcquired["CD"] = true;
        GameManager.Instance.SetShipFragmentsCount(totalShipFragments);
        //Enable CD script
    }

    public void UnlockSS()
    {
        if (totalShipFragments < 3) return;

        perksAcquired["SS"] = true;
        GameManager.Instance.SetShipFragmentsCount(totalShipFragments);
        //Enable SS script
    }

    #region Save and Load

    public void Save(ref PerkSaveData data)
    {
        data.perksUnlocked = perksAcquired;
    }

    public void Load(PerkSaveData data)
    {
        perksAcquired = data.perksUnlocked;

        if (perksAcquired["NF"])
        {
            UnlockNF();
        }

        if (perksAcquired["OE"])
        {
            UnlockOE(false);
        }
        else if (perksAcquired["OE2"])
        {
            UnlockOE(true);
        }

        if (perksAcquired["CD"])
        {
            UnlockCD();
        }

        if (perksAcquired["SS"])
        {
            UnlockSS();
        }
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveSystem.ResetDays();
    }
}

[System.Serializable]
public struct PerkSaveData
{
    public Dictionary<string, bool> perksUnlocked;
}
