using UnityEngine;
using TMPro;

public class WorkTimerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform timerPointer;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;

    [SerializeField] private TMP_Text timeText;

    [Header("Work Time")]
    [SerializeField] private int startHour = 9;
    [SerializeField] private int endHour = 15;

    private float elapsedTime = 0f;
    private float duration;

    private bool isRunning = true;

    private void Start()
    {
        if (PlayerStatus.Instance != null)
        {
            duration = PlayerStatus.Instance.dayDuration;
        }
        else
        {
            duration = 180f;
        }

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
        {
            elapsedTime = duration;
            isRunning = false;

            OnTimerEnd();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        // 0 = 시작 / 1 = 종료
        float progress = elapsedTime / duration;

        // 포인터 위치 이동
        timerPointer.anchoredPosition = Vector2.Lerp(
            startPoint.anchoredPosition,
            endPoint.anchoredPosition,
            progress
        );

        UpdateTimeText(progress);
    }

    private void UpdateTimeText(float progress)
    {
        // 09:00 ~ 15:00
        float totalGameMinutes = (endHour - startHour) * 60f;
        float currentGameMinutes = totalGameMinutes * progress;

        int hour = startHour + Mathf.FloorToInt(currentGameMinutes / 60f);
        int minute = Mathf.FloorToInt(currentGameMinutes % 60f);

        timeText.text = $"{hour:00} : {minute:00}";
    }

    private void OnTimerEnd()
    {
        Debug.Log("작업 시간 종료!");
    }
}