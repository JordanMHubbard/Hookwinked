using UnityEngine;

public class SoundFXManager : SoundFXPlayer
{
    public static SoundFXManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
