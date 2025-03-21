using UnityEngine;

public class SoundRandomizer : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource source;
    [Range(0.1f , 0.5f)]
    public float volumeChange = 0.2f;
    [Range(0.05f , 0.2f)]
    public float pitchChange = 0.1f;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (source == null) return;
        
        source.clip = sounds[Random.Range(0, sounds.Length)];
        source.volume = Random.Range(1-volumeChange, 1);
        source.pitch = Random.Range(1-pitchChange, 1);
        source.PlayOneShot(source.clip);
    }
}
