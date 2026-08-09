using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverTextColor : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Target Text")]
    [SerializeField] private TMP_Text targetText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    private void Start()
    {
        if (targetText != null)
            targetText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.color = normalColor;
    }
}