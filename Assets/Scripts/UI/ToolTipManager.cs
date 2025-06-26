using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI toolTipName;
    [SerializeField] private TextMeshProUGUI toolTipDesc;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowToolTip(string name, string description)
    {
        toolTipName.text = name;
        toolTipDesc.text = description;
        canvasGroup.DOFade(1f, 0.15f);
    }
    
    public void HideToolTip()
    {
        canvasGroup.DOFade(0f, 0.15f);
    }
}
