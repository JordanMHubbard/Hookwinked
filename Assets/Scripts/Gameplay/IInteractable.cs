using UnityEngine;

public interface IInteractable
{
    public void Interact(GameObject interactor);

    public void ShowOutline();

    public void RemoveOutline();
}
