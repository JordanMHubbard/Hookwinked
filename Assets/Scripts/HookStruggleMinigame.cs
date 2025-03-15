using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class HookStruggleMinigame : MonoBehaviour
{
    [SerializeField] private GameObject FishHookUI;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float boundsOffset = 5f;
    [SerializeField] private GameObject fishImage;
    [SerializeField] private GameObject hookImage;
    [SerializeField] private GameObject hook2Image;
    
    private bool isGrowing;
    private Vector3 originalPos;
    private RectTransform rectTransform;
    private Vector2 currentInput;
    private Vector2 previousInput;
    private float currentSpeed;
    private bool isAcceptingInput = false;


    private void Awake()
    {
        originalPos = FishHookUI.transform.position;
        rectTransform = FishHookUI.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        isAcceptingInput = true;
    }

    private IEnumerator Bounce()
    {
        transform.DOMove(originalPos + new Vector3(0.1f, 0, 0), duration);
        yield return new WaitForSeconds(duration);  
        transform.DOMove(originalPos, duration);
    } 

    private void Update()
    {
        if (isAcceptingInput) CalculateStruggle();
    }

    private void CalculateStruggle()
    {
        currentInput = InputManager.instance.ShakeInput;
        float xOffset = Mathf.Clamp(currentInput.x, -boundsOffset, boundsOffset);
        float yOffset = Mathf.Clamp(currentInput.y, -boundsOffset, boundsOffset);
        FishHookUI.transform.position += new Vector3(xOffset, yOffset, 0f) * 0.05f;

        currentSpeed = (currentInput - previousInput).magnitude;
        float struggleRate = Mathf.Clamp(currentSpeed * 0.1f, 0f, 5f) / 20f;
        //Debug.Log("currentSpeed: " + currentSpeed);
        //Debug.Log("struggleRate: " + struggleRate);
        
        if (struggleRate > 0.1f && !isGrowing) StartCoroutine(Grow(struggleRate));
    }

    private IEnumerator Grow(float rate)
    {
        isGrowing = true;
        while (rectTransform.localScale.x < 1.3f && isGrowing)
        {
            rectTransform.localScale +=  Time.deltaTime * new Vector3(rate, rate, 0f);
            yield return null;
        }
        
        // Play animation of fish freeing from hook HERE
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        BreakFreeHookAnim();
        isGrowing = false;
        isAcceptingInput = false;
        StartCoroutine(EndMinigame()); // End game
    }

    private IEnumerator EndMinigame()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ExitHookedMinigame();
    }

    private void BreakFreeAnim()
    {

    }

    private void BreakFreeHookAnim()
    {
        hookImage.SetActive(false);
        hook2Image.SetActive(true);
        hook2Image.transform.DORotate(hookImage.transform.rotation.eulerAngles + new Vector3(0f, 0f, 50f), 0.5f);
        hook2Image.transform.DOMove(hookImage.transform.position + new Vector3(90f, 125f, 0f), 0.5f).SetEase(Ease.OutCubic);
    }

    private void BreakFreeFishAnim()
    {
        
    }
}
