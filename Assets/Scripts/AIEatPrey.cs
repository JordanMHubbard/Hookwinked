using UnityEngine;

public class AIEatPrey : MonoBehaviour
{
    private SoundRandomizer eatSoundComp;

     void Awake()
    {
        eatSoundComp = GetComponent<SoundRandomizer>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            eatSoundComp.PlayRandomSound();
            other.gameObject.SetActive(false);
        }
    }
}
