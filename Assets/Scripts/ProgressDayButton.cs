using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class ProgressDayButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Day")]
    [Tooltip("D-9 버튼이면 9, D-8이면 8, D-Day면 0")]
    [SerializeField] private int buttonDay;


    [Header("Button")]
    [SerializeField] private Button button;


    [Header("Image")]
    [SerializeField] private Image buttonImage;

    [Tooltip("현재 날짜 기본 이미지")]
    [SerializeField] private Sprite normalSprite;

    [Tooltip("현재 날짜 호버 이미지")]
    [SerializeField] private Sprite hoverSprite;

    [Tooltip("이미 완료한 날짜 이미지")]
    [SerializeField] private Sprite completedSprite;

    [Tooltip("아직 진행 전인 날짜 이미지")]
    [SerializeField] private Sprite lockedSprite;


    [Header("Text")]
    [SerializeField] private TMP_Text dayText;

    [Tooltip("현재 진행해야 하는 날짜 텍스트 투명도")]
    [Range(0f, 1f)]
    [SerializeField] private float currentTextAlpha = 1f;

    [Tooltip("완료했거나 아직 진행 전인 날짜 텍스트 투명도")]
    [Range(0f, 1f)]
    [SerializeField] private float inactiveTextAlpha = 0.2f;


    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("마우스 호버 시 사운드")]
    [SerializeField] private AudioClip hoverSound;

    [Tooltip("버튼 클릭 시 사운드")]
    [SerializeField] private AudioClip clickSound;


    [Header("Click Effect")]
    [SerializeField]
    private Color pressedColor =
        new Color(0.75f, 0.75f, 0.75f, 1f);

    [SerializeField] private float pressedDuration = 0.15f;


    private bool isCurrentDay = false;

    private Coroutine clickCoroutine;


    private void Start()
    {
        Refresh();
    }


    // =========================================================
    // 날짜 상태 갱신
    // =========================================================

    public void Refresh()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning(
                $"PlayerStatus.Instance가 존재하지 않습니다. / {gameObject.name}"
            );

            return;
        }

        int currentDay = PlayerStatus.Instance.currentDay;


        // =====================================================
        // 현재 날짜
        // =====================================================

        if (buttonDay == currentDay)
        {
            isCurrentDay = true;

            button.interactable = true;

            SetTextAlpha(currentTextAlpha);

            if (buttonImage != null &&
                normalSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }
        }


        // =====================================================
        // 이미 완료한 날짜
        //
        // 예:
        // 현재 D-6이면
        // D-9, D-8, D-7은 이미 완료한 날짜
        // =====================================================

        else if (buttonDay > currentDay)
        {
            isCurrentDay = false;

            button.interactable = false;

            SetTextAlpha(inactiveTextAlpha);

            if (buttonImage != null &&
                completedSprite != null)
            {
                buttonImage.sprite = completedSprite;
            }
        }


        // =====================================================
        // 미래 날짜
        //
        // 예:
        // 현재 D-6이면
        // D-5 ~ D-Day는 미래 날짜
        // =====================================================

        else
        {
            isCurrentDay = false;

            button.interactable = false;

            SetTextAlpha(inactiveTextAlpha);

            if (buttonImage != null)
            {
                if (lockedSprite != null)
                {
                    buttonImage.sprite = lockedSprite;
                }
                else if (normalSprite != null)
                {
                    buttonImage.sprite = normalSprite;
                }
            }
        }
    }


    // =========================================================
    // Text Alpha
    // =========================================================

    private void SetTextAlpha(float alpha)
    {
        if (dayText == null)
            return;

        Color color = dayText.color;

        color.r = 1f;
        color.g = 1f;
        color.b = 1f;
        color.a = alpha;

        dayText.color = color;
    }


    // =========================================================
    // Mouse Hover
    // =========================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 현재 날짜 버튼만 작동
        if (!isCurrentDay)
            return;


        // 호버 이미지 변경
        if (buttonImage != null &&
            hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite;
        }


        // 호버 사운드
        if (audioSource != null &&
            hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }


    // =========================================================
    // Mouse Exit
    // =========================================================

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCurrentDay)
            return;


        // 기본 이미지로 복귀
        if (buttonImage != null &&
            normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }


    // =========================================================
    // Click
    // =========================================================

    public void ClickEffect()
    {
        // 현재 날짜 버튼만 클릭 효과
        if (!isCurrentDay)
            return;


        // 클릭 사운드
        if (audioSource != null &&
            clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }


        // 기존 클릭 효과가 실행 중이면 정지
        if (clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
        }


        clickCoroutine =
            StartCoroutine(ClickEffectCoroutine());
    }


    // =========================================================
    // Click Color Effect
    // =========================================================

    private IEnumerator ClickEffectCoroutine()
    {
        if (buttonImage == null)
            yield break;


        Color originalColor =
            buttonImage.color;


        // 클릭 순간 잠깐 어둡게
        buttonImage.color =
            pressedColor;


        yield return new WaitForSeconds(
            pressedDuration
        );


        // 원래 색상으로 복귀
        buttonImage.color =
            originalColor;


        clickCoroutine = null;
    }
}