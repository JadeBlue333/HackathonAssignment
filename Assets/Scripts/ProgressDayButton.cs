using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ProgressDayButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Day")]
    [Tooltip("D-9 버튼이면 9, D-8이면 8, D-0이면 0")]
    [SerializeField] private int buttonDay;

    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Image")]
    [SerializeField] private Image buttonImage;

    [Tooltip("기본 버튼 이미지")]
    [SerializeField] private Sprite normalSprite;

    [Tooltip("현재 날짜 버튼에 마우스를 올렸을 때 이미지")]
    [SerializeField] private Sprite hoverSprite;

    [Tooltip("이미 지나간 날짜의 이미지")]
    [SerializeField] private Sprite completedSprite;

    [Tooltip("아직 오지 않은 날짜 이미지")]
    [SerializeField] private Sprite lockedSprite;

    [Header("Click Effect")]
    [SerializeField]
    private Color pressedColor =
        new Color(0.75f, 0.75f, 0.75f, 1f);

    [SerializeField] private float pressedDuration = 0.15f;


    private bool isCurrentDay = false;
    private bool isCompletedDay = false;

    private Coroutine clickCoroutine;


    private void Start()
    {
        Refresh();
    }


    // =========================================================
    // 현재 날짜 확인
    // =========================================================

    public void Refresh()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning(
                $"ProgressDayButton : PlayerStatus.Instance가 없습니다. / {gameObject.name}"
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
            isCompletedDay = false;

            button.interactable = true;

            if (buttonImage != null &&
                normalSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }
        }


        // =====================================================
        // 이미 지난 날짜
        //
        // D-9 → D-8 → D-7 순서이므로
        // 현재가 D-6이면 7,8,9는 이미 지난 날짜
        // =====================================================

        else if (buttonDay > currentDay)
        {
            isCurrentDay = false;
            isCompletedDay = true;

            button.interactable = false;

            if (buttonImage != null &&
                completedSprite != null)
            {
                buttonImage.sprite = completedSprite;
            }
        }


        // =====================================================
        // 미래 날짜
        // =====================================================

        else
        {
            isCurrentDay = false;
            isCompletedDay = false;

            button.interactable = false;

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
    // Hover
    // =========================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 현재 날짜 버튼만 Hover 가능
        if (!isCurrentDay)
            return;


        if (buttonImage != null &&
            hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCurrentDay)
            return;


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


        if (clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
        }

        clickCoroutine =
            StartCoroutine(ClickEffectCoroutine());
    }


    private IEnumerator ClickEffectCoroutine()
    {
        if (buttonImage == null)
            yield break;


        Color originalColor =
            buttonImage.color;


        // 살짝 어둡게
        buttonImage.color =
            pressedColor;


        yield return new WaitForSeconds(
            pressedDuration
        );


        // 원래 색으로
        buttonImage.color =
            originalColor;


        clickCoroutine = null;
    }
}