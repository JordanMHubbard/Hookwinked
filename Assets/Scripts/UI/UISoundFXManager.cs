using UnityEngine;

public class UISoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    public void PlayHoverSound()
    {
        SoundFXManager.Instance.PlaySoundFXClip(hoverSound, transform, transform.position, 0.5f, 0f);
    }
    public void PlayClickSound()
    {
        SoundFXManager.Instance.PlaySoundFXClip(clickSound, transform, transform.position, 0.5f, 0f);
    }
}
