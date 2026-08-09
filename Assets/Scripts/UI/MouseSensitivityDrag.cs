using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseSensitivityDrag : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;
    [SerializeField] private TMP_Text valueText;

    [Header("Sensitivity")]
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 20f;
    [SerializeField] private float defaultSensitivity = 2f;

    private RectTransform parentRect;

    private const string SensitivityKey = "MouseSensitivity";

    private void Awake()
    {
        if (handle == null)
            handle = GetComponent<RectTransform>();

        parentRect = handle.parent as RectTransform;
    }

    private void Start()
    {
        float savedSensitivity =
            PlayerPrefs.GetFloat(
                SensitivityKey,
                defaultSensitivity
            );

        SetSensitivity(savedSensitivity);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateHandlePosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateHandlePosition(eventData);
    }

    private void UpdateHandlePosition(
        PointerEventData eventData)
    {
        if (parentRect == null ||
            startPoint == null ||
            endPoint == null)
            return;

        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            return;
        }

        float startX =
            startPoint.anchoredPosition.x;

        float endX =
            endPoint.anchoredPosition.x;

        float clampedX =
            Mathf.Clamp(
                localPoint.x,
                Mathf.Min(startX, endX),
                Mathf.Max(startX, endX)
            );

        Vector2 handlePosition =
            handle.anchoredPosition;

        handlePosition.x =
            clampedX;

        handle.anchoredPosition =
            handlePosition;

        // 위치를 0 ~ 1 값으로 변환
        float t =
            Mathf.InverseLerp(
                startX,
                endX,
                clampedX
            );

        // 실제 마우스 감도로 변환
        float sensitivity =
            Mathf.Lerp(
                minSensitivity,
                maxSensitivity,
                t
            );

        SaveSensitivity(
            sensitivity
        );
    }

    private void SetSensitivity(float sensitivity)
    {
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

        float x =
            Mathf.Lerp(
                startPoint.anchoredPosition.x,
                endPoint.anchoredPosition.x,
                t
            );

        Vector2 position =
            handle.anchoredPosition;

        position.x =
            x;

        handle.anchoredPosition =
            position;

        UpdateValueText(
            sensitivity
        );
    }

    private void SaveSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(
            SensitivityKey,
            sensitivity
        );

        PlayerPrefs.Save();

        UpdateValueText(
            sensitivity
        );
    }

    private void UpdateValueText(float sensitivity)
    {
        if (valueText == null)
            return;

        valueText.text =
            sensitivity.ToString("0.00");
    }

    public static float GetSensitivity(
        float defaultValue = 2f)
    {
        return PlayerPrefs.GetFloat(
            SensitivityKey,
            defaultValue
        );
    }
}