using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

public class UIHookAnim : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] public PlayerInput playerInput;
    private Vector3 originalPos;
    private InputAction shakeAction;


    private void Awake()
    {
        shakeAction = playerInput.actions["Shake"];
        originalPos = gameObject.transform.position;
    }

    private void Start()
    {
        //StartCoroutine(Bounce());

    }

    private IEnumerator Bounce()
    {
        int count = 0;
        transform.DOMove(originalPos + new Vector3(0.1f, 0, 0), duration);
        yield return new WaitForSeconds(duration);  
        transform.DOMove(originalPos, duration);
        
        // gameObject.SetActive(false); <- USE THIS TO TURN ON/OFF UI!
    } 

    private void Update()
    {
        
        transform.position += new Vector3(shakeAction.ReadValue<Vector2>().x, shakeAction.ReadValue<Vector2>().y, 0f);
    }
}
