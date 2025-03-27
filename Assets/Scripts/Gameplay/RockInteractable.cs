using UnityEngine;

public class RockInteractable : MonoBehaviour, IInteractable
{
    private Outline outlineComp;

    private void Awake()
    {
        outlineComp = GetComponent<Outline>();
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("rock has been interacted with!");
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

    
}
