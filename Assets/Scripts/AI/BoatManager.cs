using System.Collections.Generic;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    [SerializeField] private GameObject boatParent;
    [SerializeField] private List<GameObject> damageDecals;
    [SerializeField] private GameObject boatFragment;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip finalDamageSound;
    [SerializeField] private AudioSource motorSource;
    private bool isMotorOn;
    public Color32 boatColor { get; set; }
    private int timesDamaged;
    private bool canSpawnFish;
    private Vector3 currentTarget;
    private Vector3 directionToTarget;

    // Movement
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float turnSpeed = 1f;
    private float currentSpeed = 0.1f;
    float accelerationRate = 0.5f;
    private bool hasTarget;
    private bool shouldRotate;

    // Baited Fish 
    private List<GameObject> BaitedPrey = new List<GameObject>();

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.GetPreySpawner() != null)
            {
                canSpawnFish = true;
            }
        }

        if (boatParent == null)
        {
            Debug.LogWarning("boatparent has not been set!");
        }

        // Spawn random number of bait
        int spawnCount = GameManager.Instance.GetCurrentDaySettings().GetBaitCount();
        BaitedPrey = GameManager.Instance.GetPreySpawner().SpawnLures(spawnCount);
    }

    private void OnEnable()
    {
        PauseManager.Instance.OnPaused += PauseMotor;
        PauseManager.Instance.OnUnpaused += UnpauseMotor;
    }
    
    private void OnDisable()
    {
        PauseManager.Instance.OnPaused -= PauseMotor;
        PauseManager.Instance.OnUnpaused -= UnpauseMotor;
    }

    private void Start()
    {
        SetDamageColor();
    }

    private void Update()
    {
        if (shouldRotate) Rotate();
        if (hasTarget) MoveTowardsTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RockInteractable>() != null)
        {
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 contactNormal = collision.GetContact(0).normal;
            ApplyRockDamage(contactPoint, contactNormal, collision.gameObject);
        }
    }

    public void ApplyRockDamage(Vector3 point, Vector3 normal, GameObject rock)
    {
        if (++timesDamaged > 3) return;

        // Show damage decal and effect
        Vector3 position = point + normal * -0.1f;
        Quaternion rotation = Quaternion.LookRotation(normal);
        Quaternion effectRotation = Quaternion.LookRotation(-normal);
        ShowDamageEffects(position, rotation, effectRotation);
        AudioClip hitSound = timesDamaged == 3 ? finalDamageSound : damageSound;
        SoundFXManager.Instance.PlaySoundFXClip(hitSound, transform, position, 1f, 1f, 0.1f, 0.1f);

        // Drop fragment
        DropFragment(position);

        // Apply damage
        if (timesDamaged == 3)
        {
            Flee();
            GameManager.Instance.DecrementBoatCount();
            // Spawn two new prey
            if (canSpawnFish)
            {
                Debug.Log("Changing bait to prey");
                foreach (GameObject fish in BaitedPrey)
                {
                    PreyController controller = fish.GetComponent<PreyController>();
                    if (controller != null)
                    {
                        controller.SetIsLure(false);
                        controller.SetLureStatus();
                        GameManager.Instance.PreyConsumed(controller.gameObject);
                    }
                }
            }
        }
    }

    private void ShowDamageEffects(Vector3 position, Quaternion rotation, Quaternion effectRotation)
    {
        GameObject damageDecal = damageDecals[timesDamaged - 1];
        Instantiate(damageDecal, position, rotation, boatParent.transform);
        GameObject effect = Instantiate(damageEffect, position, effectRotation);
        ParticleSystemRenderer ps = effect.GetComponent<ParticleSystemRenderer>();
        ps.material.color = boatColor;
    }

    private void DropFragment(Vector3 position)
    {
        if (boatFragment != null)
        {
            MeshRenderer renderer = boatFragment.GetComponent<MeshRenderer>();
            renderer.sharedMaterial.color = boatColor;
            GameObject fragment = Instantiate(boatFragment, position, Quaternion.identity);
        }
    }

    private void Flee()
    {
        Debug.Log("We need to get outta here cap'n!");
        currentTarget = GameManager.Instance.GetRandomBoatWaypoint();

        motorSource.Play();
        isMotorOn = true;
        StartRotate();
        MoveTowardsTarget();
    }

    private void StartRotate()
    {
        directionToTarget = (currentTarget - boatParent.transform.position).normalized;
        turnSpeed = Random.Range(0.5f, 1.5f);
        shouldRotate = true;
        hasTarget = true;
    }

    private void Rotate()
    {
        boatParent.transform.rotation = Quaternion.Slerp(boatParent.transform.rotation, Quaternion.LookRotation(directionToTarget),
            turnSpeed * Time.deltaTime);

        if (Quaternion.Angle(boatParent.transform.rotation, Quaternion.LookRotation(directionToTarget)) < 0.5f)
        {
            shouldRotate = false;
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentSpeed < maxSpeed) currentSpeed *= 1f + (accelerationRate * Time.deltaTime);
        boatParent.transform.position += directionToTarget * currentSpeed * Time.deltaTime;

        float dist = Vector3.Distance(boatParent.transform.position, currentTarget);
        if (dist < 1f)
        {
            hasTarget = false;
            motorSource.Stop();
            isMotorOn = false;
        }
    }

    private void SetDamageColor()
    {
        Transform boat = transform.GetChild(0);
        if (boat == null) return;

        string boatName = boat.name;
        switch (boatName)
        {
            case "RedBoat(Clone)":
                boatColor = new Color32(201, 12, 56, 255);
                break;

            case "YellowBoat(Clone)":
                boatColor = new Color32(222, 208, 80, 255);
                break;

            case "GreenBoat(Clone)":
                boatColor = new Color32(74, 148, 61, 255);
                break;

            case "BlueBoat(Clone)":
                boatColor = new Color32(113, 194, 235, 255);
                break;
        }
    }
    
    private void PauseMotor()
    {
        if (isMotorOn) motorSource.Pause();
    }

    private void UnpauseMotor()
    {
        if (isMotorOn) motorSource.Play();
    }
}
