using UnityEngine;
using UnityEngine.Audio;

public class SoundFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeCategory;
    [SerializeField] private AudioSource soundFXobject;

    public void Mute()
    {
        Debug.Log("muted category: " + volumeCategory);
        audioMixer.SetFloat(volumeCategory, -80f);
    }
    public void Unmute() { audioMixer.SetFloat(volumeCategory, 0f); }
    
    public void PlaySoundFXClip(AudioClip audioClip, Transform parent, Vector3 position, float volume, float spatialBlend = 1f,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (!audioClip)
        {
            Debug.LogWarning("audioClip does not exist!");
            return;
        }

        AudioSource audioSource;
        if (parent != null) { audioSource = Instantiate(soundFXobject, position, Quaternion.identity, parent); }
        else { audioSource = Instantiate(soundFXobject, position, Quaternion.identity); }
        audioSource.spatialBlend = spatialBlend;
        audioSource.clip = audioClip;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(volume - volumeChange, volume);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform parent, Vector3 position, float volume, float spatialBlend = 1f,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (audioClips.Length < 1)
        {
            Debug.LogWarning("audioClips does not exist!");
            return;
        }

        AudioSource audioSource;
        if (parent != null) { audioSource = Instantiate(soundFXobject, position, Quaternion.identity, parent); }
        else { audioSource = Instantiate(soundFXobject, position, Quaternion.identity); }
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(volume - volumeChange, volume);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public GameObject LoopSoundFXClip(AudioClip audioClip, Transform parent, Vector3 position, float volume, float spatialBlend = 1f,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (!audioClip)
        {
            Debug.LogWarning("audioClip does not exist!");
            return null;
        }
        
        AudioSource audioSource;
        if (parent != null) { audioSource = Instantiate(soundFXobject, position, Quaternion.identity, parent); }
        else { audioSource = Instantiate(soundFXobject, position, Quaternion.identity); }
        audioSource.clip = audioClip;
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = true;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(volume - volumeChange, volume);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        return audioSource.gameObject;
    }

    public GameObject LoopRandomSoundFXClip(AudioClip[] audioClips, Transform parent, Vector3 position, float volume, float spatialBlend = 1f,
        float volumeChange = 0f, float pitchChange = 0f)
    {
        if (audioClips.Length < 1)
        {
            Debug.LogWarning("audioClips does not exist!");
            return null;
        }

        AudioSource audioSource;
        if (parent != null) { audioSource = Instantiate(soundFXobject, position, Quaternion.identity, parent); }
        else { audioSource = Instantiate(soundFXobject, position, Quaternion.identity); }
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = true;
        audioSource.volume = volume;

        if (volumeChange > 0f) audioSource.volume = Random.Range(volume - volumeChange, volume);
        if (pitchChange > 0f) audioSource.pitch = Random.Range(1 - pitchChange, 1);

        audioSource.Play();
        return audioSource.gameObject;
    }
}