using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("Date")]
    [SerializeField] private TMP_Text dayText;

    [Header("Clock")]
    [SerializeField] private TMP_Text timeText;

    [Header("Clock Pointer")]
    [SerializeField] private RectTransform timerPointer;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;

    public GoToThisScene goToThisScene;

    [Header("Money")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text earningText;

    [Header("Fuel")]
    //[SerializeField] private Image fuelBar;
    [SerializeField] private TMP_Text fuelText;

    [Header("Trust")]
    //[SerializeField] private Image trustBar;
    [SerializeField] private TMP_Text trustText;
    [SerializeField] private TMP_Text trustGradeText;

    [Header("채팅 이벤트 등장")]
    [SerializeField] private GameObject randomEventObject;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip randomEventSfx;

    [Range(0f, 1f)]
    [SerializeField] private float randomEventVolume = 1f;

    // 랜덤 이벤트 시간 범위
    [SerializeField] private float eventStartTime = 11f;
    [SerializeField] private float eventEndTime = 12f;

    [SerializeField] private GameObject chatPopUp;

    [Header("Skill Panel")]
    [SerializeField] private GameObject skillPanel;

    private bool chatOpened = false;

    private float randomEventTime;
    private bool randomEventTriggered = false;

    private PlayerStatus player;

    private float elapsedTime;

    private void Start()
    {
        player = PlayerStatus.Instance;
        elapsedTime = 0;

        // 설정된 시간 범위에서 랜덤한 이벤트 발생 시간 결정
        randomEventTime = Random.Range(
            eventStartTime,
            eventEndTime
        );

        // 시작할 때 비활성화
        if (randomEventObject != null)
        {
            randomEventObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        UpdateClock();
        UpdateUI();

        // Tab 홀딩 중 Skill Panel 표시
        if (Keyboard.current != null)
        {
            if (Keyboard.current.tabKey.isPressed)
            {
                skillPanel.SetActive(true);
            }
            else
            {
                skillPanel.SetActive(false);
            }
        }

        // T키로 팝업 열기
        if (Keyboard.current != null &&
            Keyboard.current.tKey.wasPressedThisFrame)
        {
            //열기 전이면 팝업을 생성
            if (!chatOpened)
            {
                randomEventObject.SetActive(false);
                chatPopUp.SetActive(true);
                chatOpened = true;
            }
            //열고난 후면 팝업을 닫기
            else
            {
                chatPopUp.SetActive(false);
            }
        }
    }

    void UpdateClock()
    {
        elapsedTime += Time.deltaTime;

        float duration = player.dayDuration;

        // 하루 진행률 (0 ~ 1)
        float t = Mathf.Clamp01(elapsedTime / duration);

        // 포인터 이동
        timerPointer.anchoredPosition = Vector2.Lerp(
            startPoint.anchoredPosition,
            endPoint.anchoredPosition,
            t
        );

        // 09:00 ~ 15:00
        float currentHour = 9f + (6f * t);

        int hour24 = Mathf.FloorToInt(currentHour);
        int minute = Mathf.FloorToInt((currentHour - hour24) * 60f);

        // 5분 단위 표시
        minute = (minute / 5) * 5;

        // 게임 내 총 진행 분
        float totalGameMinutes = 360f * t;

        // 현재 5분 구간 안에서의 진행률
        float fiveMinuteProgress = (totalGameMinutes % 5f) / 5f;

        // 5분에 한 번 깜빡임
        string separator = fiveMinuteProgress < 0.7f
            ? " : "
            : "   ";

        timeText.text = $"{hour24:00}{separator}{minute:00}";

        // 랜덤 채팅 이벤트
        if (!randomEventTriggered &&
            currentHour >= randomEventTime)
        {
            randomEventTriggered = true;

            if (randomEventObject != null)
            {
                randomEventObject.SetActive(true);
            }

            if (randomEventSfx != null)
            {
                audioSource.PlayOneShot(
    randomEventSfx,
    randomEventVolume
);
            }

            Debug.Log(
                $"★★★ 랜덤 채팅 이벤트 발생! 현재 시간: {currentHour:F2}"
            );
        }

        // 하루 종료
        if (elapsedTime >= duration)
        {
            Debug.Log("15:00 - 하루 종료");

            goToThisScene.nextSceneButton();

            enabled = false;
        }
    }

    void UpdateUI()
    {
        // Date
        dayText.text = $"D - {player.currentDay}";

        // Money
        moneyText.text = $"{player.money}";
        earningText.text = $"+ {player.earnings}";

        // Fuel
        //fuelBar.fillAmount = player.fuel / (float)PlayerStatus.MaxFuel;
        fuelText.text = $"{player.fuel} / {PlayerStatus.MaxFuel}";

        // Trust
        //trustBar.fillAmount = player.trust / (float)PlayerStatus.MaxTrust;
        trustText.text = $"{player.trust + player.trustChange} / {PlayerStatus.MaxTrust}";
        trustGradeText.text = player.GetTrustGrade();
    }
}