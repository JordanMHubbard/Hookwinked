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
    private bool isPaused;
    private bool shouldUpdate;
    private bool shouldDepreciate = true;
    [Tooltip("The rate at which currentProgress is updated to targetProgress")]
    [SerializeField] private float updateRate = 50f;
    [Tooltip("The rate at which currentProgress depreciates to 0")]
    [SerializeField] private float depreciateRate = 2f;
    private Coroutine DepreciateCoroutine;

    public void setIsPaused(bool shouldPause) { isPaused = shouldPause; }

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
        else if (shouldDepreciate) 
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
    private IEnumerator UpdateProgress()
    {
        shouldUpdate = false;
        shouldDepreciate = false;
        
        float rateOfChange = currentProgress < targetProgress ? updateRate : -updateRate;
        while (Math.Abs(currentProgress - targetProgress) > 0.1)
        {
            currentProgress += rateOfChange * Time.deltaTime;
            energyBar.value = currentProgress / 100f;
            
            if (rateOfChange > 0)
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
        
        if (currentProgress > 0f) shouldDepreciate = true;
        else { GameManager.Instance.ShowDeathScreen(DeathScreenManager.DeathType.Exhaustion); }
    }

    // Depreciates energy constantly unless energy is being updated
    private IEnumerator DepreciateProgress()
    {
       shouldDepreciate = false;
        
        while (currentProgress > 0f && !shouldUpdate)
        {
            if (isPaused) yield break;

            currentProgress -= depreciateRate * Time.deltaTime;
            //Debug.Log("CurrentProgress: "+ currentProgress);
            energyBar.value = currentProgress / 100f;

            yield return null;
        }

        currentProgress = 0f;

        yield return new WaitForSeconds(1f);
        //GameManager.Instance.ShowDeathScreen(DeathScreenManager.DeathType.Exhaustion);
    }
}
