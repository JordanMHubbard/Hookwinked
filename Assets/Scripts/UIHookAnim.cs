using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHookAnim : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private float boundsOffset = 5f;
    [SerializeField] public PlayerInput playerInput;
    private bool isGrowing;
    private Vector3 originalPos;
    private Vector2 currentInput;
    private Vector2 previousInput;
    private float currentSpeed;
    private RectTransform rectTransform;
    private InputAction shakeAction;


    private void Awake()
    {
        shakeAction = playerInput.actions["Shake"];
        originalPos = transform.position;
        rectTransform = GetComponent<RectTransform>();
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
        CalculatePosition();
        //transform.position += new Vector3(shakeAction.ReadValue<Vector2>().x, shakeAction.ReadValue<Vector2>().y, 0f) * 0.05f;
    }

    private void CalculatePosition()
    {
        currentInput = shakeAction.ReadValue<Vector2>();
        float xOffset = Mathf.Clamp(currentInput.x, -boundsOffset, boundsOffset);
        float yOffset = Mathf.Clamp(currentInput.y, -boundsOffset, boundsOffset);
        transform.position += new Vector3(xOffset, yOffset, 0f) * 0.05f;

        CalculateSpeed(currentInput);
    }

    private void CalculateSpeed(Vector2 input)
    {
        currentSpeed = (input - previousInput).magnitude;
        //Debug.Log("currentSpeed: " + currentSpeed);
        float struggleRate = Mathf.Clamp(currentSpeed * 0.1f, 0f, 5f) / 20f;
        Debug.Log("struggleRate: " + struggleRate);
        
        if (struggleRate > 0.1f && !isGrowing) StartCoroutine(Grow(struggleRate));
        else if (struggleRate < 0.1f) isGrowing = false;
    }

    private IEnumerator Grow(float rate)
    {
        isGrowing = true;
        while (rectTransform.localScale.x < 1.3f && isGrowing)
        {
            rectTransform.localScale +=  Time.deltaTime * new Vector3(rate, rate, 0f);
            yield return null;
        }
        
        isGrowing = false;
    }
}
