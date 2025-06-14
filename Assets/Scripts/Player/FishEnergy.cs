using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] private float DashDepreciateRate = 6f;
    private Coroutine DepreciateCoroutine;
    [SerializeField] private Image energyBarFill;
    [SerializeField] private Image energyBarCover;
    [SerializeField] private AudioClip nearDeathSound;
    private bool isNearDeath;

    // Getters
    public float GetDepreciationRate() { return depreciateRate; }
    public bool GetIsPaused() { return isPaused; }
    // Setters
    public void SetDepreciationRate(float rate) { depreciateRate = rate; }
    public void SetIsPaused(bool shouldPause) { isPaused = shouldPause; }
    public void UnsetNearDeath() { currentProgress = 100; }

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetIsPerkUnlocked(1))
        {
            depreciateRate = 1.5f;
            DashDepreciateRate = 4.5f;
            Debug.Log("Energy Depreciation is now 1.5 and 4.5 for dashing!");
        }
    }
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
        if (isPaused) return;

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
        else { GameManager.Instance.ShowDeathScreen(DeathScreenUI.DeathType.Exhaustion); }
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

            CheckIfNearDeath(currentProgress);

            yield return null;
        }

        currentProgress = 0f;
        isNearDeath = false;
        yield return new WaitForSeconds(1f);
        //GameManager.Instance.ShowDeathScreen(DeathScreenUI.DeathType.Exhaustion);
    }

    public void OnDash()
    {
        depreciateRate = DashDepreciateRate;
    }

    private void CheckIfNearDeath(float progress)
    {
        //Debug.Log("progress:" + progress);
        if (progress < 30 && !isNearDeath)
        {
            StartCoroutine(NearDeath());
        }
        else if (progress > 30 && isNearDeath)
        {
            isNearDeath = false;
        }
    }

    private IEnumerator NearDeath()
    {
        isNearDeath = true;

        while (isNearDeath)
        {
            energyBarFill.DOColor(Color.red, 1f);
            energyBarCover.DOColor(Color.red, 1f);
            SoundFXManager.Instance.PlaySoundFXClip(nearDeathSound, transform, 1f, 1f, 0.1f, 0.05f);
            yield return new WaitForSeconds(1f);

            energyBarFill.DOColor(Color.white, 1f);
            energyBarCover.DOColor(Color.white, 1f);
            yield return new WaitForSeconds(1f);
        }
    }
}
