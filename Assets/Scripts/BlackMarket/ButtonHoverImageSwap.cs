using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverImageSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // =====================================================
    // Image
    // =====================================================

    [Header("버튼 내부에서 변경할 이미지")]
    [SerializeField] private Image targetImage;

    [Header("이미지")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;


    // =====================================================
    // Text
    // =====================================================

    [Header("색상을 변경할 텍스트들")]
    [SerializeField] private TMP_Text[] targetTexts;

    [Header("텍스트 색상")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hoverTextColor = Color.black;


    // =====================================================
    // Mouse Hover
    // =====================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 이미지 변경
        if (targetImage != null && hoverSprite != null)
        {
            targetImage.sprite = hoverSprite;
        }

        // 텍스트 색상 변경
        foreach (TMP_Text text in targetTexts)
        {
            if (text != null)
            {
                text.color = hoverTextColor;
            }
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        // 이미지 원래대로
        if (targetImage != null && normalSprite != null)
        {
            targetImage.sprite = normalSprite;
        }

        // 텍스트 색상 원래대로
        foreach (TMP_Text text in targetTexts)
        {
            if (text != null)
            {
                text.color = normalTextColor;
            }
        }
    }
}