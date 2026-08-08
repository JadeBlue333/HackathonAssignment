using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverImageSwapTarget : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Hover Area")]
    [Tooltip("이 스크립트가 붙은 오브젝트가 호버를 감지합니다.")]
    [SerializeField] private bool useHover = true;


    [Header("Change Target")]
    [Tooltip("실제로 이미지가 변경될 다른 UI Image")]
    [SerializeField] private Image changeTargetImage;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;


    [Header("Hover Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    [Range(0f, 1f)]
    [SerializeField] private float hoverVolume = 1f;


    private void Awake()
    {
        SetNormalImage();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!useHover)
            return;

        SetHoverImage();

        if (audioSource != null &&
            hoverSound != null)
        {
            audioSource.PlayOneShot(
                hoverSound,
                hoverVolume
            );
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!useHover)
            return;

        SetNormalImage();
    }


    private void SetHoverImage()
    {
        if (changeTargetImage == null)
            return;

        if (hoverSprite == null)
            return;

        changeTargetImage.sprite =
            hoverSprite;
    }


    private void SetNormalImage()
    {
        if (changeTargetImage == null)
            return;

        if (normalSprite == null)
            return;

        changeTargetImage.sprite =
            normalSprite;
    }


    private void OnDisable()
    {
        SetNormalImage();
    }
}