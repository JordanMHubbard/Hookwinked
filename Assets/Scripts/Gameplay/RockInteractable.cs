using UnityEngine;

public class RockInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip rockInteractSound;
    private Outline outlineComp;

    private void Awake()
    {
        outlineComp = GetComponent<Outline>();
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("rock has been interacted with!");
        SoundFXManager.Instance.PlaySoundFXClip(rockInteractSound, transform.position, 1f, 1f, 0f, 0.1f);
        gameObject.SetActive(false);
        ProjectileHandler projectileHandler = interactor.GetComponent<ProjectileHandler>();
        if (projectileHandler != null) projectileHandler.AddProjectile(gameObject);
    }

    public void ShowOutline()
    {
        outlineComp.OutlineWidth = 4f;
    }

    public void RemoveOutline()
    {
        outlineComp.OutlineWidth = 0f;
    }
    
}
