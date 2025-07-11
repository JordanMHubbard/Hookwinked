using System.Collections;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance {get; private set;}
    private Light sunlight;
    [SerializeField] private float daySpeed = 1f;
    private Vector3 currentRotation;
    private float sunRotationSpeed;
    [SerializeField] private Material fog;
    [SerializeField] private Color darkColor;
    private Color baseColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        daySpeed = GameManager.Instance.GetCurrentDaySettings().daySpeed;
    }

    private void Start()
    {
        daySpeed /= 75f;
        //daySpeed *= 10f;
        sunRotationSpeed = daySpeed * 26f;
        sunlight = GetComponent<Light>();
        baseColor = fog.GetColor("_BaseColor");
        currentRotation = sunlight.transform.rotation.eulerAngles;
    }

    public void StartDay()
    {
        StartCoroutine(DecreaseIntensity());
        StartCoroutine(LowerSun());
    }

    private IEnumerator DecreaseIntensity()
    {
        float sunlightVal = sunlight.intensity;
        float t;

        while (sunlightVal > 1f)
        {
            sunlightVal -= Time.deltaTime * daySpeed;
            sunlight.intensity = sunlightVal;

            t = 1f - sunlightVal / 2.8f;
            fog.SetColor("_BaseColor", Color.Lerp(baseColor, darkColor, t));

            yield return null;
        }
    }

    private IEnumerator LowerSun()
    {
        while (currentRotation.x > 0f)
        {
            currentRotation.x -= Time.deltaTime * sunRotationSpeed;
            sunlight.transform.rotation = Quaternion.Euler(currentRotation);
            yield return null;
        }

        if (GameManager.Instance.GetCurrentDay() != 4) HomeManager.Instance.StartDayEnd();
    }

    private void OnDisable()
    {
        fog.SetColor("_BaseColor", baseColor);
    }
}
