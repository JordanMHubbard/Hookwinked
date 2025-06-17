using System.Collections;
using UnityEngine;

public class ShockGrenade : MonoBehaviour
{
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private AudioClip buzzSound;
    [SerializeField] private AudioClip shockSound;
    [SerializeField] private ParticleSystem electricty;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject grenade;
    [SerializeField] private float minDropTime = 0.5f;
    [SerializeField] private float maxDropTime = 1.01f;
    private bool isOnCooldown = true;

    private void Awake()
    {
        InitializeGrenadeRotation();
    }

    private void Start()
    {
        StartCoroutine(EnableGrenade());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOnCooldown) return;

            SoundFXManager.Instance.PlaySoundFXClip(shockSound, transform, 1f, 1f, 0, 0.1f);
            FishEnergy energyComp = other.GetComponent<FishEnergy>();
            if (energyComp) energyComp.AddProgress(-10f);
            GameManager.Instance.ShowShockedEffect();
            StartCoroutine(StartCooldown());
            Debug.Log("Player has been shocked");
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(2f);

        isOnCooldown = false;
    }

    private IEnumerator EnableGrenade()
    {
        yield return new WaitForSeconds(Random.Range(minDropTime, maxDropTime));
        rb.useGravity = false;

        SoundFXManager.Instance.PlaySoundFXClip(beepSound, transform, 1f);
        yield return new WaitForSeconds(1.3f);

        GameObject audio = SoundFXManager.Instance.LoopSoundFXClip(buzzSound, transform, 1f);
        electricty.Play();
        isOnCooldown = false;
        yield return new WaitForSeconds(10f);

        isOnCooldown = true;
        Destroy(audio);

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void InitializeGrenadeRotation()
    {
        float maxAngle = 15;

        Vector3 randomOffset = new Vector3(
            Random.Range(-maxAngle, maxAngle),
            Random.Range(-maxAngle, maxAngle),
            Random.Range(-maxAngle, maxAngle)
        );

        grenade.transform.rotation *= Quaternion.Euler(randomOffset);
    }


}
