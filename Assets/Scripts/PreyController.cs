using System.Collections;
using UnityEngine;

public class PreyController : AIFishController
{
    private bool isBait;
    public void SetIsBait(bool isFishBait) { isBait = isFishBait; }
    public bool GetIsBait() { return isBait; }
}
