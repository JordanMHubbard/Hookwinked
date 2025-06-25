using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance {get; private set;}
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private RectTransform transitionMask;
    [SerializeField] private bool shouldTransitionAtStart;
    private Vector3 originalMaskScale = new Vector3(7f, 7f, 7f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (SceneTransition != null) SceneTransition.SetActive(false);
    }

    private void Start()
    {
        if (shouldTransitionAtStart)
        {
            StartCoroutine(TransitionOut());
        }
    }

    public IEnumerator TransitionIn(Action callback)
    {
        SceneTransition.SetActive(true);
        transitionMask.DOScale(new Vector3(0f, 0f, 0f), 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(3f);
        callback?.Invoke();
    }

    public IEnumerator TransitionOut()
    {
        SceneTransition.SetActive(true);
        transitionMask.localScale = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(0.5f);

        transitionMask.DOScale(originalMaskScale, 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);
        
        SceneTransition.SetActive(false);
    }
}
