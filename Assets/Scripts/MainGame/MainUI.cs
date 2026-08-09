using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    // =========================================================
    // Date
    // =========================================================

    [Header("Date")]
    [SerializeField]
    private TMP_Text dayText;


    // =========================================================
    // Clock
    // =========================================================

    [Header("Clock")]
    [SerializeField]
    private TMP_Text timeText;


    // =========================================================
    // Work Time Range
    // =========================================================

    [Header("Work Time Range")]
    [SerializeField]
    private TMP_Text workTimeRangeText;


    // =========================================================
    // Clock Pointer
    // =========================================================

    [Header("Clock Pointer")]

    [SerializeField]
    private RectTransform timerPointer;

    [SerializeField]
    private RectTransform startPoint;

    [SerializeField]
    private RectTransform endPoint;


    // =========================================================
    // Scene
    // =========================================================

    [Header("Scene")]
    [SerializeField]
    private GoToThisScene goToThisScene;


    // =========================================================
    // Money
    // =========================================================

    [Header("Money")]

    [SerializeField]
    private TMP_Text moneyText;

    [SerializeField]
    private TMP_Text earningText;


    // =========================================================
    // Fuel
    // =========================================================

    [Header("Fuel")]
    [SerializeField]
    private TMP_Text fuelText;


    // =========================================================
    // Trust
    // =========================================================

    [Header("Trust")]

    [SerializeField]
    private TMP_Text trustText;

    [SerializeField]
    private TMP_Text trustGradeText;


    // =========================================================
    // Chat Event
    // =========================================================

    [Header("채팅 이벤트 등장")]

    [SerializeField]
    private GameObject randomEventObject;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip randomEventSfx;

    [Range(0f, 1f)]
    [SerializeField]
    private float randomEventVolume = 1f;

    [Tooltip("채팅 이벤트 발생 가능 시작 시간")]
    [SerializeField]
    private float eventStartTime = 11f;

    [Tooltip("채팅 이벤트 발생 가능 종료 시간")]
    [SerializeField]
    private float eventEndTime = 12f;

    [SerializeField]
    private GameObject chatPopUp;


    // =========================================================
    // Skill Panel
    // =========================================================

    [Header("Skill Panel")]
    [SerializeField]
    private GameObject skillPanel;


    // =========================================================
    // Shortcut Buttons
    // =========================================================

    [Header("Shortcut Buttons")]

    [Tooltip("P 키로 실행할 업무 종료 버튼")]
    [SerializeField]
    private Button endWorkButton;

    [Tooltip("Space 키로 실행할 메뉴얼 버튼")]
    [SerializeField]
    private Button manualButton;

    [Tooltip("ESC 키로 실행할 환경설정 버튼")]
    [SerializeField]
    private Button settingsButton;


    // =========================================================
    // Shortcut Panels
    // =========================================================

    [Header("Shortcut Panels")]

    [Tooltip("업무 종료 확인창 전체 오브젝트")]
    [SerializeField]
    private GameObject endWorkPanel;

    [Tooltip("메뉴얼 전체 오브젝트")]
    [SerializeField]
    private GameObject manualPanel;

    [Tooltip("환경설정 전체 오브젝트")]
    [SerializeField]
    private GameObject settingsPanel;


    // =========================================================
    // Time Pause Layer
    // =========================================================

    [Header("Time Pause")]

    [Tooltip(
        "이 Layer에 속한 활성화된 UI가 하나라도 있으면 " +
        "업무 시간이 멈춥니다."
    )]
    [SerializeField]
    private LayerMask timePauseLayer;


    private readonly List<GameObject>
        timePauseObjects =
            new List<GameObject>();


    // =========================================================
    // Runtime
    // =========================================================

    private PlayerStatus player;

    private float elapsedTime;

    private float randomEventTime;

    private bool randomEventTriggered = false;

    private bool chatOpened = false;

    private bool dayEnding = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        player =
            PlayerStatus.Instance;


        elapsedTime =
            0f;


        // =====================================================
        // PauseUI Layer 오브젝트 찾기
        // =====================================================

        CacheTimePauseObjects();


        // =====================================================
        // 랜덤 채팅 이벤트 시간 결정
        // =====================================================

        randomEventTime =
            Random.Range(
                eventStartTime,
                eventEndTime
            );


        // =====================================================
        // 랜덤 이벤트 아이콘 초기화
        // =====================================================

        if (randomEventObject != null)
        {
            randomEventObject.SetActive(
                false
            );
        }


        // =====================================================
        // 채팅 팝업 초기화
        // =====================================================

        if (chatPopUp != null)
        {
            chatPopUp.SetActive(
                false
            );
        }


        chatOpened =
            false;


        // =====================================================
        // 스킬 패널 초기화
        // =====================================================

        if (skillPanel != null)
        {
            skillPanel.SetActive(
                false
            );
        }


        // =====================================================
        // 초기 UI 표시
        //
        // advanceTime = false
        // → 시작 프레임에서 시간이 증가하지 않음
        // =====================================================

        UpdateClock(
            false
        );

        UpdateUI();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (player == null)
            return;


        if (dayEnding)
            return;


        // =====================================================
        // 먼저 입력 처리
        //
        // 이번 프레임에 창을 열면
        // 바로 그 프레임부터 시간이 멈춤
        // =====================================================

        HandleSkillPanelInput();

        HandleChatInput();

        HandleShortcutInput();


        // =====================================================
        // 일반 UI 갱신
        // =====================================================

        UpdateUI();


        // =====================================================
        // PauseUI가 없을 때만 시간 진행
        // =====================================================

        if (!IsTimePausedByUI())
        {
            UpdateClock(
                true
            );
        }
    }


    // =========================================================
    // Pause UI Cache
    // =========================================================

    private void CacheTimePauseObjects()
    {
        timePauseObjects.Clear();


        GameObject[] allObjects =
            FindObjectsByType<GameObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (GameObject obj in allObjects)
        {
            if (obj == null)
                continue;


            // 현재 GameObject의 Layer가
            // timePauseLayer에 포함되어 있는지 확인
            bool isPauseLayer =
                (
                    timePauseLayer.value &
                    (1 << obj.layer)
                ) != 0;


            if (isPauseLayer)
            {
                timePauseObjects.Add(
                    obj
                );
            }
        }


        Debug.Log(
            $"Time Pause UI 등록 완료 : " +
            $"{timePauseObjects.Count}개"
        );
    }


    // =========================================================
    // Refresh Pause UI Cache
    //
    // 런타임 중 새로운 PauseUI 오브젝트가 생성될 경우
    // 필요할 때 호출 가능
    // =========================================================

    public void RefreshTimePauseObjects()
    {
        CacheTimePauseObjects();
    }


    // =========================================================
    // Time Pause Check
    // =========================================================

    private bool IsTimePausedByUI()
    {
        for (
            int i = 0;
            i < timePauseObjects.Count;
            i++
        )
        {
            GameObject obj =
                timePauseObjects[i];


            if (obj == null)
                continue;


            // Hierarchy에서 실제로 활성 상태인지 확인
            if (obj.activeInHierarchy)
            {
                return true;
            }
        }


        return false;
    }


    // =========================================================
    // Shortcut Input
    // =========================================================

    private void HandleShortcutInput()
    {
        if (Keyboard.current == null)
            return;


        // =====================================================
        // P = 업무 종료 확인창
        // =====================================================

        if (
            Keyboard.current.pKey
                .wasPressedThisFrame
        )
        {
            // 이미 열려 있으면 닫기
            if (
                endWorkPanel != null &&
                endWorkPanel.activeSelf
            )
            {
                endWorkPanel.SetActive(
                    false
                );
            }

            // 닫혀 있으면 실제 업무 종료 버튼 클릭
            else if (
                endWorkButton != null &&
                endWorkButton.interactable &&
                endWorkButton.gameObject
                    .activeInHierarchy
            )
            {
                endWorkButton.onClick
                    .Invoke();
            }
        }


        // =====================================================
        // Space = 메뉴얼
        // =====================================================

        if (
            Keyboard.current.spaceKey
                .wasPressedThisFrame
        )
        {
            if (
                manualPanel != null &&
                manualPanel.activeSelf
            )
            {
                manualPanel.SetActive(
                    false
                );
            }

            else if (
                manualButton != null &&
                manualButton.interactable &&
                manualButton.gameObject
                    .activeInHierarchy
            )
            {
                manualButton.onClick
                    .Invoke();
            }
        }


        // =====================================================
        // ESC = 환경설정
        // =====================================================

        if (
            Keyboard.current.escapeKey
                .wasPressedThisFrame
        )
        {
            if (
                settingsPanel != null &&
                settingsPanel.activeSelf
            )
            {
                settingsPanel.SetActive(
                    false
                );
            }

            else if (
                settingsButton != null &&
                settingsButton.interactable &&
                settingsButton.gameObject
                    .activeInHierarchy
            )
            {
                settingsButton.onClick
                    .Invoke();
            }
        }
    }


    // =========================================================
    // Skill Panel Input
    // =========================================================

    private void HandleSkillPanelInput()
    {
        if (Keyboard.current == null)
            return;


        if (
            Keyboard.current.tabKey
                .wasPressedThisFrame
        )
        {
            ToggleSkillPanel();
        }
    }


    // =========================================================
    // Skill Panel Toggle
    // =========================================================

    public void ToggleSkillPanel()
    {
        if (skillPanel == null)
            return;


        skillPanel.SetActive(
            !skillPanel.activeSelf
        );
    }


    public void OpenSkillPanel()
    {
        if (skillPanel == null)
            return;


        skillPanel.SetActive(
            true
        );
    }


    public void CloseSkillPanel()
    {
        if (skillPanel == null)
            return;


        skillPanel.SetActive(
            false
        );
    }


    // =========================================================
    // Chat Input
    // =========================================================

    private void HandleChatInput()
    {
        if (Keyboard.current == null)
            return;


        if (
            Keyboard.current.tKey
                .wasPressedThisFrame
        )
        {
            ToggleChatPopUp();
        }
    }


    // =========================================================
    // Chat Toggle
    // =========================================================

    public void ToggleChatPopUp()
    {
        if (chatPopUp == null)
            return;


        if (!chatOpened)
        {
            if (randomEventObject != null)
            {
                randomEventObject.SetActive(
                    false
                );
            }


            chatPopUp.SetActive(
                true
            );


            chatOpened =
                true;
        }
        else
        {
            chatPopUp.SetActive(
                !chatPopUp.activeSelf
            );
        }
    }


    // =========================================================
    // Clock
    // =========================================================

    private void UpdateClock(
        bool advanceTime
    )
    {
        if (player == null)
            return;


        // =====================================================
        // 실제 시간 진행
        // =====================================================

        if (advanceTime)
        {
            elapsedTime +=
                Time.deltaTime;
        }


        float duration =
            player.dayDuration;


        if (duration <= 0f)
            return;


        // =====================================================
        // Game Time
        //
        // 실제 180초
        // =
        // 게임 내 09:00 ~ 15:00
        //
        // 실제 1초
        // =
        // 게임 내 2분
        // =====================================================

        float gameMinutesPassed =
            elapsedTime * 2f;


        // 09:00
        int startTimeMinutes =
            9 * 60;


        int endTimeMinutes =
            player.GetWorkEndTimeMinutes();


        int totalWorkGameMinutes =
            endTimeMinutes -
            startTimeMinutes;


        // =====================================================
        // Progress
        // =====================================================

        float progress =
            0f;


        if (totalWorkGameMinutes > 0)
        {
            progress =
                Mathf.Clamp01(
                    gameMinutesPassed /
                    totalWorkGameMinutes
                );
        }


        // =====================================================
        // Clock Pointer
        // =====================================================

        if (
            timerPointer != null &&
            startPoint != null &&
            endPoint != null
        )
        {
            timerPointer.anchoredPosition =
                Vector2.Lerp(
                    startPoint.anchoredPosition,
                    endPoint.anchoredPosition,
                    progress
                );
        }


        // =====================================================
        // Current Time
        // =====================================================

        float currentTotalMinutes =
            startTimeMinutes +
            gameMinutesPassed;


        currentTotalMinutes =
            Mathf.Min(
                currentTotalMinutes,
                endTimeMinutes
            );


        int hour24 =
            Mathf.FloorToInt(
                currentTotalMinutes /
                60f
            );


        int minute =
            Mathf.FloorToInt(
                currentTotalMinutes %
                60f
            );


        // 5분 단위 표시
        minute =
            (minute / 5) * 5;


        // =====================================================
        // Colon Blink
        // =====================================================

        float fiveMinuteProgress =
            (gameMinutesPassed % 5f) /
            5f;


        string separator =
            fiveMinuteProgress < 0.7f
                ? " : "
                : "   ";


        if (timeText != null)
        {
            timeText.text =
                $"{hour24:00}" +
                $"{separator}" +
                $"{minute:00}";
        }


        // =====================================================
        // Random Chat Event
        // =====================================================

        float currentHour =
            currentTotalMinutes /
            60f;


        if (
            !randomEventTriggered &&
            currentHour >= randomEventTime
        )
        {
            randomEventTriggered =
                true;


            if (randomEventObject != null)
            {
                randomEventObject.SetActive(
                    true
                );
            }


            if (
                audioSource != null &&
                randomEventSfx != null
            )
            {
                audioSource.PlayOneShot(
                    randomEventSfx,
                    randomEventVolume
                );
            }


            Debug.Log(
                $"★★★ 랜덤 채팅 이벤트 발생! " +
                $"현재 시간: {currentHour:F2}"
            );
        }


        // =====================================================
        // 자동 하루 종료
        // =====================================================

        if (elapsedTime >= duration)
        {
            elapsedTime =
                duration;


            int endHour =
                endTimeMinutes /
                60;


            int endMinute =
                endTimeMinutes %
                60;


            if (timeText != null)
            {
                timeText.text =
                    $"{endHour:00} : " +
                    $"{endMinute:00}";
            }


            if (
                timerPointer != null &&
                endPoint != null
            )
            {
                timerPointer.anchoredPosition =
                    endPoint.anchoredPosition;
            }


            EndWorkDay();
        }
    }


    // =========================================================
    // 업무 중단 / 하루 종료
    // =========================================================

    public void EndWorkDay()
    {
        if (dayEnding)
            return;


        dayEnding =
            true;


        int startTimeMinutes =
            9 * 60;


        float gameMinutesPassed =
            elapsedTime * 2f;


        int currentTimeMinutes =
            Mathf.FloorToInt(
                startTimeMinutes +
                gameMinutesPassed
            );


        int maxEndTimeMinutes =
            player != null
                ? player.GetWorkEndTimeMinutes()
                : 15 * 60;


        currentTimeMinutes =
            Mathf.Min(
                currentTimeMinutes,
                maxEndTimeMinutes
            );


        int hour =
            currentTimeMinutes /
            60;


        int minute =
            currentTimeMinutes %
            60;


        minute =
            (minute / 5) * 5;


        Debug.Log(
            $"업무 종료 - " +
            $"{hour:00}:{minute:00}"
        );


        if (goToThisScene != null)
        {
            goToThisScene
                .nextSceneButton();
        }


        enabled =
            false;
    }


    // =========================================================
    // UI
    // =========================================================

    private void UpdateUI()
    {
        if (player == null)
            return;


        // =====================================================
        // Date
        // =====================================================

        if (dayText != null)
        {
            dayText.text =
                $"D - {player.currentDay}";
        }


        // =====================================================
        // Money
        // =====================================================

        if (moneyText != null)
        {
            moneyText.text =
                $"{player.money}";
        }


        if (earningText != null)
        {
            earningText.text =
                $"+ {player.earnings}";
        }


        // =====================================================
        // Fuel
        // =====================================================

        if (fuelText != null)
        {
            fuelText.text =
                $"{player.fuel} / " +
                $"{PlayerStatus.MaxFuel}";
        }


        // =====================================================
        // Trust
        // =====================================================

        if (trustText != null)
        {
            int displayedTrust =
                Mathf.Clamp(
                    player.trust +
                    player.trustChange,
                    0,
                    PlayerStatus.MaxTrust
                );


            trustText.text =
                $"{displayedTrust} / " +
                $"{PlayerStatus.MaxTrust}";
        }


        if (trustGradeText != null)
        {
            trustGradeText.text =
                GetDisplayedTrustGrade();
        }


        UpdateWorkTimeRangeText();
    }


    // =========================================================
    // Work Time Range
    // =========================================================

    private void UpdateWorkTimeRangeText()
    {
        if (
            workTimeRangeText == null ||
            player == null
        )
        {
            return;
        }


        int startHour =
            9;

        int startMinute =
            0;


        int endTimeMinutes =
            player.GetWorkEndTimeMinutes();


        int endHour =
            endTimeMinutes /
            60;


        int endMinute =
            endTimeMinutes %
            60;


        workTimeRangeText.text =
            $"{startHour:00}:" +
            $"{startMinute:00} - " +
            $"{endHour:00}:" +
            $"{endMinute:00}";
    }


    // =========================================================
    // Displayed Trust Grade
    // =========================================================

    private string GetDisplayedTrustGrade()
    {
        int displayedTrust =
            Mathf.Clamp(
                player.trust +
                player.trustChange,
                0,
                PlayerStatus.MaxTrust
            );


        if (
            displayedTrust >=
            PlayerStatus.TrustGradeA
        )
        {
            return "A";
        }


        if (
            displayedTrust >=
            PlayerStatus.TrustGradeB
        )
        {
            return "B";
        }


        return "C";
    }
}