using UnityEngine;

public class UISoundFXManager : SoundFXPlayer
{
    public static UISoundFXManager Instance { get; private set; }
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void PlayHoverSound()
    {
        PlaySoundFXClip(hoverSound, null, transform.position, 0.5f, 0f);
    }

    public void PlayClickSound()
    {
        PlaySoundFXClip(clickSound, null, transform.position, 0.5f, 0f);
    }
}
