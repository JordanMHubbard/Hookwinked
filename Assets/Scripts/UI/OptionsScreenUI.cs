using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class OptionsScreenUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider mouseSensSlider;

    private void Start()
    {
        InitializeSettings();
    }
    private void InitializeSettings()
    {
        mixer.GetFloat("masterVolume", out float vol);
        masterAudioSlider.value = vol;

        mouseSensSlider.value = InputManager.Instance.mouseSensitivity * 10f;
    }
    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("masterVolume", volume);
    }
    public void SetMouseSens(float sensitivity)
    {
        Debug.Log("Slider sens: " + sensitivity);
        float sens = sensitivity / 10f;
        InputManager.Instance.mouseSensitivity = sens;
    }
    public void Back()
    {
        PauseManager.Instance.ShowPausedScreen();
        gameObject.SetActive(false);
    }
}
