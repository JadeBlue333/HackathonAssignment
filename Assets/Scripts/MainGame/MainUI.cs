using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("Date")]
    [SerializeField] private TMP_Text dayText;

    [Header("Clock")]
    [SerializeField] private TMP_Text timeText;
    public GoToThisScene goToThisScene;

    [Header("Money")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text earningText;

    [Header("Fuel")]
    [SerializeField] private Image fuelBar;
    [SerializeField] private TMP_Text fuelText;

    [Header("Trust")]
    [SerializeField] private Image trustBar;
    [SerializeField] private TMP_Text trustText;

    private PlayerStatus player;

    private float elapsedTime;

    private void Start()
    {
        player = PlayerStatus.Instance;
        elapsedTime = 0;
    }

    private void Update()
    {
        if (player == null)
            return;

        UpdateClock();
        UpdateUI();
    }

    void UpdateClock()
    {
        elapsedTime += Time.deltaTime;

        float duration = player.dayDuration;

        // 하루 진행률 (0 ~ 1)
        float t = Mathf.Clamp01(elapsedTime / duration);

        // 09:00 ~ 15:00
        float currentHour = 9f + (6f * t);

        int hour24 = Mathf.FloorToInt(currentHour);
        int minute = Mathf.FloorToInt((currentHour - hour24) * 60f);

        // 5분 단위 표시
        minute = (minute / 5) * 5;

        // 게임 내 총 진행 분
        // 09:00 ~ 15:00 = 총 360분
        float totalGameMinutes = 360f * t;

        // 현재 5분 구간 안에서의 진행률
        // 0 ~ 1
        float fiveMinuteProgress = (totalGameMinutes % 5f) / 5f;

        // 5분에 한 번 깜빡임
        // 앞 70%는 콜론 표시
        // 뒤 30%는 콜론 숨김
        // 다음 5분으로 넘어가면 다시 표시
        string separator = fiveMinuteProgress < 0.7f
            ? " : "
            : "   ";

        timeText.text = $"{hour24:00}{separator}{minute:00}";

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
        dayText.text = $"D-{player.currentDay}";

        moneyText.text = $"{player.money}";
        earningText.text = $"(+{player.earnings})";

        fuelBar.fillAmount = player.fuel / (float)PlayerStatus.MaxFuel;
        fuelText.text = $"{player.fuel} 연료";

        trustBar.fillAmount = player.trust / (float)PlayerStatus.MaxTrust;
        trustText.text = $"{player.trust} 신뢰도";
    }
}