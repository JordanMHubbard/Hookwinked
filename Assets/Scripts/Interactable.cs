using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact ()
    {
        Debug.Log(gameObject.name + " was interacted with!");
    }

    public virtual void Use ()
    {
        
    }
}
