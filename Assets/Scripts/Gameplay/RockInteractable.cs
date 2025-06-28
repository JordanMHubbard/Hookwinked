using System.Collections;
using UnityEngine;

public class RockInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip rockInteractSound;
    private Outline outlineComp;
    private bool isCollected;

    private void Awake()
    {
        outlineComp = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        StartCoroutine(Reset());
    }

    public void Interact(GameObject interactor)
    {
        if (isCollected) return;

        isCollected = true;
        Debug.Log("rock has been interacted with!");
        SoundFXManager.Instance.PlaySoundFXClip(rockInteractSound, null, transform.position, 1f, 1f, 0f, 0.1f);
        ProjectileHandler projectileHandler = interactor.GetComponent<ProjectileHandler>();
        if (projectileHandler != null) projectileHandler.AddProjectile(gameObject);
        gameObject.SetActive(false);
    }

    public void ShowOutline()
    {
        outlineComp.OutlineWidth = 4f;
    }

    public void RemoveOutline()
    {
        outlineComp.OutlineWidth = 0f;
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);
        isCollected = false;
    }
    
}
