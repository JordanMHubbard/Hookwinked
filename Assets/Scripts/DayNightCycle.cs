using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Light sunlight;
    [SerializeField] private float daySpeed = 1f;
    private Vector3 currentRotation;
    private float sunRotationSpeed;
    [SerializeField] private Material fog;
    [SerializeField] private Color darkColor;
    private Color baseColor;
    
    void Start()
    {
        daySpeed /= 75f;
        sunRotationSpeed = daySpeed * 26f;
        sunlight = GetComponent<Light>();
        baseColor = fog.GetColor("_BaseColor");
        currentRotation = sunlight.transform.rotation.eulerAngles;

        StartCoroutine(decreaseIntensity());
        StartCoroutine(lowerSun());
    }

    private IEnumerator decreaseIntensity()
    {
        float sunlightVal = sunlight.intensity;
        float t;

        while (sunlightVal > 0f)
        {
            sunlightVal -= Time.deltaTime * daySpeed;
            sunlight.intensity = sunlightVal;

            t = 1f - sunlightVal/2.5f;
            fog.SetColor("_BaseColor",  Color.Lerp(baseColor, darkColor, t));

            yield return null;
        }

        GameManager.Instance.ShowSurviveScreen();
    }

    private IEnumerator lowerSun()
    {
        while (currentRotation.x > 0f)
        {
            currentRotation.x -= Time.deltaTime * sunRotationSpeed;
            sunlight.transform.rotation = Quaternion.Euler(currentRotation);
            yield return null;
        }
    }

    private void OnDisable()
    {
        fog.SetColor("_BaseColor", baseColor);
    }
}
