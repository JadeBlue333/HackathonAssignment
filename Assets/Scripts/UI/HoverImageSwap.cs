using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverImageSwap : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Image")]
    [SerializeField] private Image targetImage;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;


    [Header("Hover Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    [Range(0f, 1f)]
    [SerializeField] private float hoverVolume = 1f;


    private void Awake()
    {
        if (targetImage != null && normalSprite != null)
        {
            targetImage.sprite = normalSprite;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover 이미지로 변경
        if (targetImage != null && hoverSprite != null)
        {
            targetImage.sprite = hoverSprite;
        }

        // Hover 효과음 1회
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(
                hoverSound,
                hoverVolume
            );
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        // 기본 이미지로 복귀
        if (targetImage != null && normalSprite != null)
        {
            targetImage.sprite = normalSprite;
        }
    }


    private void OnDisable()
    {
        // UI가 꺼질 때 기본 이미지로 초기화
        if (targetImage != null && normalSprite != null)
        {
            targetImage.sprite = normalSprite;
        }
    }
}