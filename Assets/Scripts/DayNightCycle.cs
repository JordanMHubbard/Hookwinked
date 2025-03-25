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
    }

    private void Update()
    {
        decreaseIntensity();
        lowerSun();
    }

    private void decreaseIntensity()
    {
        if (sunlight.intensity > 0f)
        {
            sunlight.intensity -= Time.deltaTime * daySpeed;
        }
    }

    private void lowerSun()
    {
        if (currentRotation.x > 0f)
        {
            currentRotation.x -= Time.deltaTime * sunSpeed;
            sunlight.transform.rotation = Quaternion.Euler(currentRotation);
        }
    }
}
