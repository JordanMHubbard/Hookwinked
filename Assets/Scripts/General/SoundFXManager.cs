using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXobject;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform parent, float volume,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (!audioClip)
        {
            Debug.LogWarning("audioClip does not exist!");
            return;
        }

        AudioSource audioSource = Instantiate(soundFXobject, parent.position, Quaternion.identity, parent);
        audioSource.clip = audioClip;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(1 - volumeChange, 1);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform parent, float volume,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (audioClips.Length < 1)
        {
            Debug.LogWarning("audioClips does not exist!");
            return;
        }

        AudioSource audioSource = Instantiate(soundFXobject, parent.position, Quaternion.identity, parent);
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(1 - volumeChange, 1);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public GameObject LoopSoundFXClip(AudioClip audioClip, Transform parent, float volume,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (!audioClip)
        {
            Debug.LogWarning("audioClip does not exist!");
            return null;
        }

        AudioSource audioSource = Instantiate(soundFXobject, parent.position, Quaternion.identity, parent);
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(1 - volumeChange, 1);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        return audioSource.gameObject;
    }

    public GameObject LoopRandomSoundFXClip(AudioClip[] audioClips, Transform parent, float volume,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (audioClips.Length < 1)
        {
            Debug.LogWarning("audioClips does not exist!");
            return null;
        }

        AudioSource audioSource = Instantiate(soundFXobject, parent.position, Quaternion.identity, parent);
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.loop = true;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(1 - volumeChange, 1);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        return audioSource.gameObject;
    }

}
