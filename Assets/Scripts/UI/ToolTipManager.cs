using UnityEngine;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance {get; private set;}
    [SerializeField] private TextMeshProUGUI toolTipName;
    [SerializeField] private TextMeshProUGUI toolTipDesc;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowToolTip(string name, string description)
    {
        toolTipName.text = name;
        toolTipDesc.text = description;
    }

    public void HideToolTip()
    {
        toolTipName.text = string.Empty;
        toolTipDesc.text = string.Empty;
    }
}
