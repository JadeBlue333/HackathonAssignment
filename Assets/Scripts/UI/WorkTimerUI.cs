using UnityEngine;
using TMPro;

public class WorkTimerUI : MonoBehaviour
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [SerializeField]
    private RectTransform timerPointer;

    [SerializeField]
    private RectTransform startPoint;

    [SerializeField]
    private RectTransform endPoint;

    [SerializeField]
    private TMP_Text timeText;


    // =========================================================
    // Work Time
    // =========================================================

    [Header("Work Time")]

    [SerializeField]
    private int startHour = 9;

    [Tooltip("PlayerStatus가 없을 때 사용할 기본 작업 시간")]
    [SerializeField]
    private float fallbackDuration = 180f;


    // =========================================================
    // Runtime
    // =========================================================

    private float elapsedTime = 0f;

    private float duration;

    // 게임 내 시작 시각 (분 단위)
    private int startTimeMinutes;

    // 게임 내 종료 시각 (분 단위)
    private int endTimeMinutes;

    private bool isRunning = true;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // 시작 시각
        // 09:00 = 540분
        startTimeMinutes =
            startHour * 60;


        // PlayerStatus가 있을 경우
        if (PlayerStatus.Instance != null)
        {
            // 실제 플레이 시간
            duration =
                PlayerStatus.Instance.dayDuration;


            // 작업 연장 스킬이 적용된
            // 게임 내 종료 시각
            endTimeMinutes =
                PlayerStatus.Instance
                    .GetWorkEndTimeMinutes();
        }
        else
        {
            // PlayerStatus가 없을 때 기본값
            duration =
                fallbackDuration;

            endTimeMinutes =
                15 * 60;
        }


        elapsedTime = 0f;

        isRunning = true;


        UpdateTimerUI();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (!isRunning)
            return;


        elapsedTime +=
            Time.deltaTime;


        // 작업시간 종료
        if (elapsedTime >= duration)
        {
            elapsedTime =
                duration;

            isRunning =
                false;


            UpdateTimerUI();

            OnTimerEnd();

            return;
        }


        UpdateTimerUI();
    }


    // =========================================================
    // Timer UI
    // =========================================================

    private void UpdateTimerUI()
    {
        if (duration <= 0f)
            return;


        // 0 = 시작
        // 1 = 종료
        float progress =
            Mathf.Clamp01(
                elapsedTime / duration
            );


        // -----------------------------------------------------
        // Pointer
        // -----------------------------------------------------

        if (timerPointer != null &&
            startPoint != null &&
            endPoint != null)
        {
            timerPointer.anchoredPosition =
                Vector2.Lerp(
                    startPoint.anchoredPosition,
                    endPoint.anchoredPosition,
                    progress
                );
        }


        // -----------------------------------------------------
        // Time Text
        // -----------------------------------------------------

        UpdateTimeText(
            progress
        );
    }


    // =========================================================
    // Time Text
    // =========================================================

    private void UpdateTimeText(
        float progress
    )
    {
        if (timeText == null)
            return;


        // 전체 게임 내 작업시간
        //
        // 기본:
        // 09:00 ~ 15:00
        // = 360분
        //
        // Lv.1:
        // 09:00 ~ 15:30
        // = 390분
        //
        // Lv.2:
        // 09:00 ~ 16:00
        // = 420분
        //
        // Lv.3:
        // 09:00 ~ 16:30
        // = 450분

        float totalGameMinutes =
            endTimeMinutes -
            startTimeMinutes;


        float passedGameMinutes =
            totalGameMinutes *
            progress;


        float currentTotalMinutes =
            startTimeMinutes +
            passedGameMinutes;


        int hour =
            Mathf.FloorToInt(
                currentTotalMinutes / 60f
            );


        int minute =
            Mathf.FloorToInt(
                currentTotalMinutes % 60f
            );


        // -----------------------------------------------------
        // 5분 단위 표시
        // -----------------------------------------------------

        minute =
            (minute / 5) * 5;


        // -----------------------------------------------------
        // 종료 순간에는 정확한 종료시각 강제
        // -----------------------------------------------------

        if (!isRunning)
        {
            hour =
                endTimeMinutes / 60;

            minute =
                endTimeMinutes % 60;
        }


        timeText.text =
            $"{hour:00} : {minute:00}";
    }


    // =========================================================
    // Timer End
    // =========================================================

    private void OnTimerEnd()
    {
        int endHour =
            endTimeMinutes / 60;

        int endMinute =
            endTimeMinutes % 60;


        Debug.Log(
            $"작업 시간 종료! " +
            $"{endHour:00}:{endMinute:00}"
        );
    }
}