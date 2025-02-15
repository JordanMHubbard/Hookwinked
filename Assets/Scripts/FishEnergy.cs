using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [Header("Progress Bar")]
    public Text valueText;
    public Slider energyBar;
    private float currentProgress = 100;
    private float targetProgress;
    private bool shoulUpdate;
    private bool shouldDecrease = true;
    [Tooltip("The rate at which currentProgress is updated to targetProgress")]
    [SerializeField] private float updateRate = 20f;
    [Tooltip("The rate at which currentProgress depreciates to 0")]
    [SerializeField] private float depreciateRate = 5f;

    private void Start()
    {
        if (energyBar == null)
        {
            Debug.LogWarning($"{gameObject.name}: EnergyBar script is missing a Slider reference!");
            enabled = false;
        }
        Debug.Log("CurrentProgress: "+ currentProgress);
    }

    private void Update()
    {
        if (shoulUpdate)
        {
            StartCoroutine(UpdateProgress());
        }
        else if (shouldDecrease) 
        {
            StartCoroutine(DepreciateProgress());
        }

    }

    public void SetTargetProgress(float progressVal)
    {
        targetProgress = Mathf.Clamp(currentProgress + progressVal, 0f, 100f);
        shoulUpdate = true;
    }

    // Updates energy progress when target progress value is changed
    IEnumerator UpdateProgress()
    {
        shoulUpdate = false;
        
        float rateOfChange = currentProgress < targetProgress ? updateRate : -updateRate;
        
        while (Math.Abs(currentProgress - targetProgress) > 0.1)
        {
            currentProgress += rateOfChange * Time.deltaTime;
            Debug.Log("CurrentProgress: "+ currentProgress);
            energyBar.value = currentProgress / 100f;

            yield return null;
        }

        currentProgress = targetProgress;
        
        if (currentProgress > 0f) shouldDecrease = true;
    }

    // Depreciates energy constantly unless energy is being updated
    IEnumerator DepreciateProgress()
    {
        shouldDecrease = false;
        
        while (currentProgress > 0f)
        {
            if (shoulUpdate) yield break;

            currentProgress -= depreciateRate * Time.deltaTime;
            //Debug.Log("CurrentProgress: "+ currentProgress);
            energyBar.value = currentProgress / 100f;

            yield return null;
        }

        currentProgress = 0f;
    }
}
