using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainUI : MonoBehaviour
{
    // =========================================================
    // Date
    // =========================================================

    [Header("Date")]
    [SerializeField] private TMP_Text dayText;


    // =========================================================
    // Clock
    // =========================================================

    [Header("Clock")]
    [SerializeField] private TMP_Text timeText;


    // =========================================================
    // Clock Pointer
    // =========================================================

    [Header("Clock Pointer")]
    [SerializeField] private RectTransform timerPointer;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;


    // =========================================================
    // Scene
    // =========================================================

    [Header("Scene")]
    [SerializeField] private GoToThisScene goToThisScene;


    // =========================================================
    // Money
    // =========================================================

    [Header("Money")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text earningText;


    // =========================================================
    // Fuel
    // =========================================================

    [Header("Fuel")]
    [SerializeField] private TMP_Text fuelText;


    // =========================================================
    // Trust
    // =========================================================

    [Header("Trust")]
    [SerializeField] private TMP_Text trustText;
    [SerializeField] private TMP_Text trustGradeText;


    // =========================================================
    // Chat Event
    // =========================================================

    [Header("채팅 이벤트 등장")]

    [SerializeField] private GameObject randomEventObject;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip randomEventSfx;

    [Range(0f, 1f)]
    [SerializeField] private float randomEventVolume = 1f;


    [Tooltip("채팅 이벤트 발생 가능 시작 시간")]
    [SerializeField] private float eventStartTime = 11f;

    [Tooltip("채팅 이벤트 발생 가능 종료 시간")]
    [SerializeField] private float eventEndTime = 12f;


    [SerializeField] private GameObject chatPopUp;


    // =========================================================
    // Skill Panel
    // =========================================================

    [Header("Skill Panel")]

    [SerializeField] private GameObject skillPanel;


    // =========================================================
    // Runtime
    // =========================================================

    private PlayerStatus player;

    private float elapsedTime;

    private float randomEventTime;

    private bool randomEventTriggered = false;

    private bool chatOpened = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        player = PlayerStatus.Instance;

        elapsedTime = 0f;


        // -----------------------------------------------------
        // 랜덤 채팅 이벤트 시간 결정
        // -----------------------------------------------------

        randomEventTime = Random.Range(
            eventStartTime,
            eventEndTime
        );


        // -----------------------------------------------------
        // 랜덤 이벤트 아이콘 초기화
        // -----------------------------------------------------

        if (randomEventObject != null)
        {
            randomEventObject.SetActive(false);
        }


        // -----------------------------------------------------
        // 채팅 팝업 초기화
        // -----------------------------------------------------

        if (chatPopUp != null)
        {
            chatPopUp.SetActive(false);
        }


        chatOpened = false;


        // -----------------------------------------------------
        // 스킬 패널 초기화
        // -----------------------------------------------------

        if (skillPanel != null)
        {
            skillPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // 처음 UI 갱신
        // -----------------------------------------------------

        UpdateClock();
        UpdateUI();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (player == null)
            return;


        UpdateClock();

        UpdateUI();

        HandleSkillPanelInput();

        HandleChatInput();
    }


    // =========================================================
    // Skill Panel Input
    // =========================================================

    private void HandleSkillPanelInput()
    {
        if (Keyboard.current == null)
            return;


        // Tab을 한 번 누를 때마다
        // 스킬 패널 열기 / 닫기
        if (Keyboard.current.tabKey.wasPressedThisFrame)
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


    // =========================================================
    // Skill Panel Open
    // =========================================================

    public void OpenSkillPanel()
    {
        if (skillPanel == null)
            return;


        skillPanel.SetActive(true);
    }


    // =========================================================
    // Skill Panel Close
    // =========================================================

    public void CloseSkillPanel()
    {
        if (skillPanel == null)
            return;


        skillPanel.SetActive(false);
    }


    // =========================================================
    // Chat Input
    // =========================================================

    private void HandleChatInput()
    {
        if (Keyboard.current == null)
            return;


        if (Keyboard.current.tKey.wasPressedThisFrame)
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


        // 처음 열 때
        // 랜덤 이벤트 알림 아이콘 끄기
        if (!chatOpened)
        {
            if (randomEventObject != null)
            {
                randomEventObject.SetActive(false);
            }


            chatPopUp.SetActive(true);

            chatOpened = true;
        }
        else
        {
            bool newState =
                !chatPopUp.activeSelf;


            chatPopUp.SetActive(
                newState
            );
        }
    }


    // =========================================================
    // Clock
    // =========================================================

    private void UpdateClock()
    {
        if (player == null)
            return;


        elapsedTime +=
            Time.deltaTime;


        float duration =
            player.dayDuration;


        if (duration <= 0f)
            return;


        // =====================================================
        // Game Time
        //
        // 기본:
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


        // 작업 연장 스킬 적용 종료시간
        //
        // Lv.0 = 15:00
        // Lv.1 = 15:30
        // Lv.2 = 16:00
        // Lv.3 = 16:30

        int endTimeMinutes =
            player.GetWorkEndTimeMinutes();


        int totalWorkGameMinutes =
            endTimeMinutes -
            startTimeMinutes;


        // =====================================================
        // Progress
        // =====================================================

        float progress = 0f;


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


        // =====================================================
        // Current Game Time
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
                currentTotalMinutes / 60f
            );


        int minute =
            Mathf.FloorToInt(
                currentTotalMinutes % 60f
            );


        // =====================================================
        // 5분 단위 표시
        // =====================================================

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
                $"{hour24:00}{separator}{minute:00}";
        }


        // =====================================================
        // Random Chat Event
        // =====================================================

        float currentHour =
            currentTotalMinutes /
            60f;


        if (!randomEventTriggered &&
            currentHour >= randomEventTime)
        {
            randomEventTriggered = true;


            if (randomEventObject != null)
            {
                randomEventObject.SetActive(true);
            }


            if (audioSource != null &&
                randomEventSfx != null)
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
        // Day End
        // =====================================================

        if (elapsedTime >= duration)
        {
            elapsedTime =
                duration;


            int endHour =
                endTimeMinutes / 60;


            int endMinute =
                endTimeMinutes % 60;


            // 종료 시각 정확히 표시
            if (timeText != null)
            {
                timeText.text =
                    $"{endHour:00} : {endMinute:00}";
            }


            // 포인터 정확히 끝점
            if (timerPointer != null &&
                endPoint != null)
            {
                timerPointer.anchoredPosition =
                    endPoint.anchoredPosition;
            }


            Debug.Log(
                $"{endHour:00}:{endMinute:00} - 하루 종료"
            );


            if (goToThisScene != null)
            {
                goToThisScene.nextSceneButton();
            }


            enabled = false;
        }
    }


    // =========================================================
    // UI
    // =========================================================

    private void UpdateUI()
    {
        if (player == null)
            return;


        // -----------------------------------------------------
        // Date
        // -----------------------------------------------------

        if (dayText != null)
        {
            dayText.text =
                $"D - {player.currentDay}";
        }


        // -----------------------------------------------------
        // Money
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // Fuel
        // -----------------------------------------------------

        if (fuelText != null)
        {
            fuelText.text =
                $"{player.fuel} / " +
                $"{PlayerStatus.MaxFuel}";
        }


        // -----------------------------------------------------
        // Trust
        // -----------------------------------------------------

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


        if (displayedTrust >=
            PlayerStatus.TrustGradeA)
        {
            return "A";
        }


        if (displayedTrust >=
            PlayerStatus.TrustGradeB)
        {
            return "B";
        }


        return "C";
    }
}