using System.Collections;
using UnityEngine;

public class FragmentInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip fragmentInteractSound;
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

    private void Start()
    {
        transform.localScale = transform.localScale +
                new Vector3(Random.Range(-0.1f, 0.1f), 0f, Random.Range(-0.1f, 0.1f));
    }

    public void Interact(GameObject interactor)
    {
        if (isCollected) return;

        isCollected = true;
        Debug.Log("fragment has been collected!");
        SoundFXManager.Instance.PlaySoundFXClip(fragmentInteractSound, null, transform.position, 1f, 1f, 0f, 0.1f);
        GameManager.Instance.IncrementCurrentDayFragments();
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
    
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);
        isCollected = false;
    }
    
}
