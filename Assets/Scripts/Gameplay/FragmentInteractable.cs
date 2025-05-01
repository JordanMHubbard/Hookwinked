using UnityEngine;

public class FragmentInteractable : MonoBehaviour, IInteractable
{
    private Outline outlineComp;

    private void Awake()
    {
        outlineComp = GetComponent<Outline>();
    }

    private void Start()
    {
        transform.localScale = transform.localScale + 
                new Vector3 (Random.Range(-0.1f, 0.1f), 0f, Random.Range(-0.1f, 0.1f));
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("fragment has been collected!");
        GameManager.Instance.SetBoatFragmentsCount(GameManager.Instance.GetBoatFragmentsCount() + 1);
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
