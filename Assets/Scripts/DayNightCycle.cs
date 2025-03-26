using System.Collections;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Light sunlight;
    [SerializeField] private float daySpeed = 1f;
    private Vector3 currentRotation;
    private float sunSpeed;
    
    void Start()
    {
        daySpeed /= 75f;
        sunSpeed = daySpeed * 26f;
        sunlight = GetComponent<Light>();
        currentRotation = sunlight.transform.rotation.eulerAngles;

        StartCoroutine(decreaseIntensity());
        StartCoroutine(lowerSun());
    }

    private IEnumerator decreaseIntensity()
    {
        while (sunlight.intensity > 0f)
        {
            sunlight.intensity -= Time.deltaTime * daySpeed;
            yield return null;
        }
        GameManager.Instance.ShowSurviveScreen();
    }

    private IEnumerator lowerSun()
    {
        while (currentRotation.x > 0f)
        {
            currentRotation.x -= Time.deltaTime * sunSpeed;
            sunlight.transform.rotation = Quaternion.Euler(currentRotation);
            yield return null;
        }
    }
}
