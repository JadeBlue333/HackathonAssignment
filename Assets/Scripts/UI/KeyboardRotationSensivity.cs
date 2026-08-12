using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardRotationSensitivityUI : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [Tooltip("좌우로 움직일 삼각형")]
    [SerializeField]
    private RectTransform handle;

    [Tooltip("감도 조절 시작 위치")]
    [SerializeField]
    private RectTransform startPoint;

    [Tooltip("감도 조절 끝 위치")]
    [SerializeField]
    private RectTransform endPoint;

    [Tooltip("현재 감도 숫자 표시")]
    [SerializeField]
    private TMP_Text valueText;


    // =========================================================
    // Sensitivity
    // =========================================================

    [Header("방향키 회전 감도")]

    [Tooltip("최소 감도")]
    [SerializeField]
    private float minSensitivity = 0.2f;

    [Tooltip("최대 감도")]
    [SerializeField]
    private float maxSensitivity = 20f;

    [Tooltip("처음 실행했을 때 기본 감도")]
    [SerializeField]
    private float defaultSensitivity = 1f;


    // =========================================================
    // PlayerPrefs
    // =========================================================

    public const string KeyboardRotationSensitivityKey =
        "KeyboardRotationSensitivity";


    // =========================================================
    // Runtime
    // =========================================================

    private RectTransform coordinateRect;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        if (handle == null)
        {
            handle =
                GetComponent<RectTransform>();
        }

        if (handle != null)
        {
            coordinateRect =
                handle.parent as RectTransform;
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        float savedSensitivity =
            PlayerPrefs.GetFloat(
                KeyboardRotationSensitivityKey,
                defaultSensitivity
            );

        SetSensitivity(
            savedSensitivity
        );
    }


    // =========================================================
    // Pointer Down
    // =========================================================

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        UpdateHandleFromPointer(
            eventData
        );
    }


    // =========================================================
    // Drag
    // =========================================================

    public void OnDrag(
        PointerEventData eventData
    )
    {
        UpdateHandleFromPointer(
            eventData
        );
    }


    // =========================================================
    // 마우스 위치 → Handle 위치
    // =========================================================

    private void UpdateHandleFromPointer(
        PointerEventData eventData
    )
    {
        if (handle == null ||
            startPoint == null ||
            endPoint == null ||
            coordinateRect == null)
        {
            return;
        }


        Vector2 localPoint;

        bool success =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                coordinateRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );

        if (!success)
            return;


        // -----------------------------------------------------
        // Start / End 위치
        // -----------------------------------------------------

        float startX =
            startPoint.anchoredPosition.x;

        float endX =
            endPoint.anchoredPosition.x;


        float minX =
            Mathf.Min(
                startX,
                endX
            );

        float maxX =
            Mathf.Max(
                startX,
                endX
            );


        // -----------------------------------------------------
        // 마우스 X 제한
        // -----------------------------------------------------

        float clampedX =
            Mathf.Clamp(
                localPoint.x,
                minX,
                maxX
            );


        // -----------------------------------------------------
        // 삼각형 이동
        // -----------------------------------------------------

        Vector2 handlePosition =
            handle.anchoredPosition;

        handlePosition.x =
            clampedX;

        handle.anchoredPosition =
            handlePosition;


        // -----------------------------------------------------
        // 위치 → 0 ~ 1
        // -----------------------------------------------------

        float t =
            Mathf.InverseLerp(
                startX,
                endX,
                clampedX
            );


        // -----------------------------------------------------
        // 0 ~ 1 → 실제 감도
        // -----------------------------------------------------

        float sensitivity =
            Mathf.Lerp(
                minSensitivity,
                maxSensitivity,
                t
            );


        SetAndSaveSensitivity(
            sensitivity
        );
    }


    // =========================================================
    // 저장된 감도를 Handle 위치로 적용
    // =========================================================

    private void SetSensitivity(
        float sensitivity
    )
    {
        if (handle == null ||
            startPoint == null ||
            endPoint == null)
        {
            return;
        }


        sensitivity =
            Mathf.Clamp(
                sensitivity,
                minSensitivity,
                maxSensitivity
            );


        float t =
            Mathf.InverseLerp(
                minSensitivity,
                maxSensitivity,
                sensitivity
            );


        float targetX =
            Mathf.Lerp(
                startPoint.anchoredPosition.x,
                endPoint.anchoredPosition.x,
                t
            );


        Vector2 handlePosition =
            handle.anchoredPosition;

        handlePosition.x =
            targetX;

        handle.anchoredPosition =
            handlePosition;


        UpdateValueText(
            sensitivity
        );
    }


    // =========================================================
    // 감도 저장
    // =========================================================

    private void SetAndSaveSensitivity(
        float sensitivity
    )
    {
        sensitivity =
            Mathf.Clamp(
                sensitivity,
                minSensitivity,
                maxSensitivity
            );


        PlayerPrefs.SetFloat(
            KeyboardRotationSensitivityKey,
            sensitivity
        );

        PlayerPrefs.Save();


        UpdateValueText(
            sensitivity
        );
    }


    // =========================================================
    // 숫자 표시
    // =========================================================

    private void UpdateValueText(
        float sensitivity
    )
    {
        if (valueText == null)
            return;


        valueText.text =
            sensitivity.ToString(
                "0.00"
            );
    }


    // =========================================================
    // 다른 코드에서 현재 감도 가져오기
    // =========================================================

    public static float GetSensitivity(
        float defaultValue = 1f
    )
    {
        float value =
            PlayerPrefs.GetFloat(
                KeyboardRotationSensitivityKey,
                defaultValue
            );


        return Mathf.Clamp(
            value,
            0.2f,
            20f
        );
    }


    // =========================================================
    // Inspector 보정
    // =========================================================

    private void OnValidate()
    {
        minSensitivity =
            Mathf.Max(
                0.2f,
                minSensitivity
            );


        maxSensitivity =
            Mathf.Max(
                minSensitivity,
                maxSensitivity
            );


        defaultSensitivity =
            Mathf.Clamp(
                defaultSensitivity,
                minSensitivity,
                maxSensitivity
            );
    }
}