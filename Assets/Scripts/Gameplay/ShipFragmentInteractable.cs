using UnityEngine;

public class ShipFragmentInteractable : MonoBehaviour, IInteractable
{
    private Outline outlineComp;

    private void Awake()
    {
        outlineComp = GetComponent<Outline>();
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("fragment has been collected!");
        GameManager.Instance.IncrementShipFragments();
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
