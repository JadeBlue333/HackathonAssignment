using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverChangeOtherTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("다른 버튼 내부의 텍스트")]
    [SerializeField] private TMP_Text targetText;

    [Header("호버했을 때 색")]
    [SerializeField] private Color hoverColor = Color.red;

    private Color originalColor;

    private void Start()
    {
        // B 버튼 텍스트의 원래 색 저장
        originalColor = targetText.color;
    }

    // A 버튼에 마우스가 들어왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetText.color = hoverColor;
    }

    // A 버튼에서 마우스가 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        targetText.color = originalColor;
    }
}