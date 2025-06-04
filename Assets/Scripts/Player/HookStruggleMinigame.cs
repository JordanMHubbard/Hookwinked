using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HookStruggleMinigame : MonoBehaviour
{
    [SerializeField] private GameObject FishHookUI;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float boundsOffset = 5f;
    [SerializeField] private GameObject fishImage;
    [SerializeField] private GameObject hookImage;
    [SerializeField] private GameObject hook2Image;
    [SerializeField] private GameObject textPrompt;
    [SerializeField] private CanvasGroup leftToolTip;
    [SerializeField] private CanvasGroup rightToolTip;
    [SerializeField] private Slider struggleMeter;

    [SerializeField] private AudioClip reelingSound;
    [SerializeField] private AudioClip stretchSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioSource reelingSource;
    [SerializeField] private AudioSource stretchSource;

    private Vector3 fishPosition;
    private Vector3 hookRotation;
    private Vector3 hookPosition;
    private Vector3 hook2Position;
    private Vector3 textRotation;

    private bool isGameActive;
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
        fishPosition = fishImage.transform.position;
        hookPosition = hookImage.transform.position;
        hookRotation = hookImage.transform.rotation.eulerAngles;
        hook2Position = hook2Image.transform.position;
        textRotation = textPrompt.transform.rotation.eulerAngles;
        SetupAudio();
    }

    private void OnEnable()
    {
        isGameActive = true;
        StartCoroutine(textAnim());
    }

    private void Update()
    {
        if (isAcceptingInput) CalculateStruggle();
    }

    private void SetupAudio()
    {
        if (reelingSource)
        {
            reelingSource.loop = true;
            reelingSource.clip = reelingSound;
        }
        if (stretchSource)
        {
            stretchSource.loop = true;
            stretchSource.clip = stretchSound;
        }
    }

    private void CalculateStruggle()
    {
        if (!reelingSource.isPlaying) reelingSource.Play();

        currentInput = InputManager.Instance.ShakeInput;
        float xOffset = Mathf.Clamp(currentInput.x, -boundsOffset, boundsOffset);
        //float yOffset = Mathf.Clamp(currentInput.y, -boundsOffset, boundsOffset);
        FishHookUI.transform.position += new Vector3(xOffset, 0f, 0f) * 0.05f;

        currentSpeed = (currentInput - previousInput).magnitude;
        float struggleRate = Mathf.Clamp(currentSpeed * 0.2f, 0f, 5f) / 20f;

        //Debug.Log("currentSpeed: " + currentSpeed);
        Debug.Log("struggleRate: " + struggleRate);
        ApplyStruggle(struggleRate);
    }

    private void ApplyStruggle(float rate)
    {
        if (rate > 0.1f)
        {
            if (!stretchSource.isPlaying) stretchSource.Play();

            if (struggleMeter.value > 0)
            {
                struggleMeter.value -= rate * Time.deltaTime;
                rate *= 0.2f;
                if (rectTransform.localScale.x < 1.3f) rectTransform.localScale +=
                    Time.deltaTime * new Vector3(rate, rate, 0f);
            }
            else
            {
                StartCoroutine(EndMinigame());
            }
        }
        else
        {
            if (stretchSource.isPlaying) stretchSource.Pause();

            if (struggleMeter.value < 1)
            {
                struggleMeter.value += 0.2f * Time.deltaTime;
                if (rectTransform.localScale.x > 1f) rectTransform.localScale -=
                    Time.deltaTime * new Vector3(0.04f, 0.04f, 0f);
            }
            
            if (struggleMeter.value >= 1)
            {
                reelingSource.Stop();
                StartCoroutine(GameManager.Instance.PlayerController.DeathSimple());
                GameManager.Instance.ExitHookedMinigame(false);
            }
        }
    }

    private IEnumerator EndMinigame()
    {
        reelingSource.Stop();
        stretchSource.Stop();
        //GameManager.Instance.PlayerController.EndShake();
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        isGameActive = false;
        isAcceptingInput = false;
        BreakFreeHookAnim();
        yield return new WaitForSeconds(0.75f);

        FishSwimAwayAnim();
        yield return new WaitForSeconds(0.75f);

        Reset();
        GameManager.Instance.ExitHookedMinigame(true);
    }

    private void BreakFreeHookAnim()
    {
        hookImage.SetActive(false);
        hook2Image.SetActive(true);
        SoundFXManager.Instance.PlaySoundFXClip(releaseSound, transform, 1f, 0f);
        fishImage.transform.DOMove(fishPosition + new Vector3(-150f, -20f, 0f), 0.75f).SetEase(Ease.OutCubic);
        hook2Image.transform.DORotate(hookRotation + new Vector3(0f, 0f, 50f), 0.5f);
        hook2Image.transform.DOMove(hook2Position + new Vector3(90f, 125f, 0f), 0.5f).SetEase(Ease.OutCubic);
    }

    private void FishSwimAwayAnim()
    {
        fishImage.transform.DOMove(fishPosition + new Vector3(1200, 0f, 0f), 0.75f).SetEase(Ease.OutCubic);
    }

    private IEnumerator textAnim()
    {
        textPrompt.transform.DORotate(textRotation + new Vector3(0f, 0f, 2.5f), 0.04f);
        textPrompt.transform.DOScale(1.5f, 0.5f);
        yield return new WaitForSeconds(0.04f);

        float count = 0;
        while (count < 6)
        {
            textPrompt.transform.DORotate(textRotation + new Vector3(0f, 0f, -5f), 0.08f);
            yield return new WaitForSeconds(0.08f);
            textPrompt.transform.DORotate(textRotation + new Vector3(0f, 0f, 5f), 0.08f);
            yield return new WaitForSeconds(0.08f);
            count++;
        }

        textPrompt.transform.DOScale(1f, 0.5f);
        textPrompt.transform.DORotate(textRotation, 0.04f);
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(toolTipAnim());
        isAcceptingInput = true;
        //GameManager.Instance.PlayerController.StartShake();

    }

    private IEnumerator toolTipAnim()
    {
        while(isGameActive)
        {
            leftToolTip.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            leftToolTip.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.5f); 
            rightToolTip.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f); 
            rightToolTip.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.5f); 
        }
    }

    private void Reset()
    {
        hook2Image.transform.position = hook2Position;
        hookImage.transform.position = hookPosition;
        hook2Image.transform.rotation = Quaternion.Euler(hookRotation);
        fishImage.transform.position = fishPosition;
        textPrompt.transform.rotation = Quaternion.Euler(textRotation);
        struggleMeter.value = 0.7f;
        hookImage.SetActive(true);
        hook2Image.SetActive(false);
    }
}
