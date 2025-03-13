using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishEnergy : MonoBehaviour
{
    [Header("Progress Bar")]
    public Text valueText;
    public Slider energyBar;
    private float currentProgress = 100;
    private float targetProgress;
    private bool shouldUpdate;
    private bool shouldDecrease = true;
    [Tooltip("The rate at which currentProgress is updated to targetProgress")]
    [SerializeField] private float updateRate = 50f;
    [Tooltip("The rate at which currentProgress depreciates to 0")]
    [SerializeField] private float depreciateRate = 2f;
    private Coroutine DepreciateCoroutine;

    private void Start()
    {
        if (energyBar == null)
        {
            Debug.LogWarning($"{gameObject.name}: EnergyBar script is missing a Slider reference!");
            enabled = false;
        }
        //Debug.Log("CurrentProgress: "+ currentProgress);
    }

    private void Update()
    {
        if (shouldUpdate)
        {
            StopCoroutine(DepreciateCoroutine);
            StartCoroutine(UpdateProgress());
        }
        else if (shouldDecrease) 
        {
            DepreciateCoroutine = StartCoroutine(DepreciateProgress());
        }

    }

    public void AddProgress(float progressVal)
    {
        targetProgress = Mathf.Clamp(currentProgress + progressVal, 0f, 100f);
        shouldUpdate = true;
    }

    // Updates energy progress when target progress value is changed
    IEnumerator UpdateProgress()
    {
        shouldUpdate = false;
        shouldDecrease = false;
        
        float rateOfChange = currentProgress < targetProgress ? updateRate : -updateRate;
        while (Math.Abs(currentProgress - targetProgress) > 0.1)
        {
            currentProgress += rateOfChange * Time.deltaTime;
            //Debug.Log("rate of change: "+ rateOfChange);
            //Debug.Log("current-target: "+ Math.Abs(currentProgress - targetProgress));
            //Debug.Log("TargetProgress: "+ targetProgress);
            //Debug.Log("CurrentProgress: "+ currentProgress);
            energyBar.value = currentProgress / 100f;
            
            if (updateRate > 0)
            {
                if (currentProgress > targetProgress) break;
            }
            else 
            {
                if (currentProgress < targetProgress) break;
            }

            yield return null;
        }

        currentProgress = targetProgress;
        
        if (currentProgress > 0f) shouldDecrease = true;
    }

    // Depreciates energy constantly unless energy is being updated
    IEnumerator DepreciateProgress()
    {
        shouldDecrease = false;
        
        while (currentProgress > 0f && !shouldUpdate)
        {
            currentProgress -= depreciateRate * Time.deltaTime;
            //Debug.Log("CurrentProgress: "+ currentProgress);
            energyBar.value = currentProgress / 100f;

            yield return null;
        }

        currentProgress = 0f;
    }
}
