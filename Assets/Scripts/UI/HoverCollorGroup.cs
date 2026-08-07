using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverColorGroup : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Targets")]
    [SerializeField] private Image[] imageTargets;
    [SerializeField] private TMP_Text[] textTargets;


    [Header("Hover Color")]
    [Tooltip("호버 시 섞일 색")]
    [SerializeField] private Color hoverColor =
        new Color(1f, 0.35f, 0.1f);

    [Tooltip("Hover 색이 원래 색에 얼마나 섞일지")]
    [Range(0f, 1f)]
    [SerializeField] private float hoverStrength = 0.35f;

    [Tooltip("색 전환 속도")]
    [SerializeField] private float colorChangeSpeed = 10f;


    [Header("Hover Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    [Range(0f, 1f)]
    [SerializeField] private float hoverVolume = 1f;


    private Color[] originalImageColors;
    private Color[] originalTextColors;

    private bool isHovered;


    private void Awake()
    {
        // 처음 설정되어 있던 색 저장
        originalImageColors =
            new Color[imageTargets.Length];

        for (int i = 0; i < imageTargets.Length; i++)
        {
            if (imageTargets[i] != null)
            {
                originalImageColors[i] =
                    imageTargets[i].color;
            }
        }


        originalTextColors =
            new Color[textTargets.Length];

        for (int i = 0; i < textTargets.Length; i++)
        {
            if (textTargets[i] != null)
            {
                originalTextColors[i] =
                    textTargets[i].color;
            }
        }
    }


    private void Update()
    {
        UpdateColors();
    }


    // =========================================================
    // Hover Start
    // =========================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        if (audioSource != null &&
            hoverSound != null)
        {
            audioSource.PlayOneShot(
                hoverSound,
                hoverVolume
            );
        }
    }


    // =========================================================
    // Hover End
    // =========================================================

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }


    // =========================================================
    // Color Transition
    // =========================================================

    private void UpdateColors()
    {
        // Image
        for (int i = 0; i < imageTargets.Length; i++)
        {
            if (imageTargets[i] == null)
                continue;

            Color targetColor =
                isHovered
                ? Color.Lerp(
                    originalImageColors[i],
                    hoverColor,
                    hoverStrength
                )
                : originalImageColors[i];


            imageTargets[i].color =
                Color.Lerp(
                    imageTargets[i].color,
                    targetColor,
                    colorChangeSpeed *
                    Time.unscaledDeltaTime
                );
        }


        // Text
        for (int i = 0; i < textTargets.Length; i++)
        {
            if (textTargets[i] == null)
                continue;

            Color targetColor =
                isHovered
                ? Color.Lerp(
                    originalTextColors[i],
                    hoverColor,
                    hoverStrength
                )
                : originalTextColors[i];


            textTargets[i].color =
                Color.Lerp(
                    textTargets[i].color,
                    targetColor,
                    colorChangeSpeed *
                    Time.unscaledDeltaTime
                );
        }
    }
}