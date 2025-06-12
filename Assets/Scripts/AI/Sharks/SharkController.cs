using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SharkController : PredatorController
{
    public Animator animator;
    
    protected override void Awake()
    {
        base.Awake();

        SetTargetPosition(GameManager.Instance.GetRandomFishWaypoint());
    }
}
