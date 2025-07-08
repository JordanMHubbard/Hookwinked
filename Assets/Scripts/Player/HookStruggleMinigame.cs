using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HookStruggleMinigame : MonoBehaviour
{
    [SerializeField] private RectTransform FishHookUI;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float boundsOffset = 5f;
    [SerializeField] private RectTransform fishImage;
    [SerializeField] private RectTransform hookImage;
    [SerializeField] private RectTransform hook2Image;
    [SerializeField] private RectTransform textPrompt;
    [SerializeField] private CanvasGroup leftToolTip;
    [SerializeField] private CanvasGroup rightToolTip;
    [SerializeField] private Slider struggleMeter;

    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip reelingSound;
    [SerializeField] private AudioClip stretchSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioClip swimSound;
    [SerializeField] private AudioSource reelingSource;
    [SerializeField] private AudioSource stretchSource;

    private Vector2 fishPosition;
    private Vector3 hookRotation;
    private Vector2 hookPosition;
    private Vector2 hook2Position;
    private Vector3 textRotation;

    private bool isGameActive;
    private Vector2 originalPos;
    private Vector2 currentInput;
    private Vector2 previousInput;
    private float currentSpeed;
    private bool isAcceptingInput = false;

    private void Awake()
    {
        originalPos = FishHookUI.anchoredPosition;
        fishPosition = fishImage.anchoredPosition;
        hookPosition = hookImage.anchoredPosition;
        hookRotation = hookImage.localRotation.eulerAngles;
        hook2Position = hook2Image.anchoredPosition;
        textRotation = textPrompt.localRotation.eulerAngles;
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
        FishHookUI.anchoredPosition += new Vector2(xOffset, 0f) * 0.06f;

        currentSpeed = (currentInput - previousInput).magnitude;
        float struggleRate = Mathf.Clamp(currentSpeed * 0.2f, 0f, 5f) / 20f;

        //Debug.Log("currentSpeed: " + currentSpeed);
        //Debug.Log("struggleRate: " + struggleRate);
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
                if (FishHookUI.localScale.x < 1.3f) FishHookUI.localScale +=
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
                if (FishHookUI.localScale.x > 1f) FishHookUI.localScale -=
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
        FishHookUI.localScale = new Vector3(1f, 1f, 1f);
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
        hookImage.gameObject.SetActive(false);
        hook2Image.gameObject.SetActive(true);
        SoundFXManager.Instance.PlaySoundFXClip(releaseSound, null, transform.position, 1f, 0f);
        fishImage.DOAnchorPos(fishPosition + new Vector2(-200f, -20f), 0.75f).SetEase(Ease.OutCubic);
        hook2Image.DOLocalRotate(hookRotation + new Vector3(0f, 0f, 30f), 0.5f);
        hook2Image.DOAnchorPos(hook2Position + new Vector2(140f, 180f), 0.5f).SetEase(Ease.OutCubic);
    }

    private void FishSwimAwayAnim()
    {
        SoundFXManager.Instance.PlaySoundFXClip(swimSound, null, transform.position, 1f, 0f);
        fishImage.DOAnchorPos(fishPosition + new Vector2(2600, 0f), 0.75f).SetEase(Ease.OutCubic);
    }

    private IEnumerator textAnim()
    {
        SoundFXManager.Instance.PlaySoundFXClip(alertSound, null, transform.position, 1f, 0f);
        textPrompt.DOLocalRotate(textRotation + new Vector3(0f, 0f, 2.5f), 0.04f);
        textPrompt.DOScale(1.5f, 0.5f);
        yield return new WaitForSeconds(0.04f);

        float count = 0;
        while (count < 6)
        {
            textPrompt.DOLocalRotate(textRotation + new Vector3(0f, 0f, -5f), 0.08f);
            yield return new WaitForSeconds(0.08f);
            textPrompt.DOLocalRotate(textRotation + new Vector3(0f, 0f, 5f), 0.08f);
            yield return new WaitForSeconds(0.08f);
            count++;
        }

        textPrompt.DOScale(1f, 0.5f);
        textPrompt.DOLocalRotate(textRotation, 0.04f);
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
        hook2Image.anchoredPosition = hook2Position;
        hookImage.anchoredPosition = hookPosition;
        hook2Image.localRotation = Quaternion.Euler(hookRotation);
        fishImage.anchoredPosition = fishPosition;
        textPrompt.localRotation = Quaternion.Euler(textRotation);
        struggleMeter.value = 0.7f;
        hookImage.gameObject.SetActive(true);
        hook2Image.gameObject.SetActive(false);
    }
}
