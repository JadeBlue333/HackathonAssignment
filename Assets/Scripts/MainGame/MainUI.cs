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

    // 하루가 시작된 시각
    private float elapsedTime;

    //private bool dayEnded;

    private void Start()
    {
        player = PlayerStatus.Instance;

        elapsedTime = 0;
        //dayEnded = false;
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

        // 하루 진행률 (0~1)
        float t = Mathf.Clamp01(elapsedTime / duration);

        // 09:00 ~ 15:00 (총 6시간)
        float currentHour = 9f + (6f * t);

        int hour24 = Mathf.FloorToInt(currentHour);
        int minute = Mathf.FloorToInt((currentHour - hour24) * 60f);
        minute = (minute / 10) * 10;

        // 24시간 -> 12시간(AM/PM) 변환
        string period = hour24 < 12 ? "AM" : "PM";

        int hour12 = hour24;
        if (hour12 == 0)
            hour12 = 12;
        else if (hour12 > 12)
            hour12 -= 12;

        string separator = (Mathf.FloorToInt(Time.time) % 2 == 0) ? ":" : " "; // 2초마다 : 깜박거리는 효과
        timeText.text = $"{hour12:00}{separator}{minute:00} {period}";

        // 하루 종료
        if (elapsedTime >= duration)
        {
            Debug.Log("15:00 PM - 하루 종료");

            goToThisScene.nextSceneButton();

            enabled = false;   // 여러 번 호출 방지
        }
    }

    void UpdateUI()
    {
        dayText.text = $"D-{player.currentDay}";

        moneyText.text = $"{player.money} 크레타";
        earningText.text = $"(+{player.earnings})";

        fuelBar.fillAmount = player.fuel / (float)PlayerStatus.MaxFuel;
        fuelText.text = $"{player.fuel} 연료";

        trustBar.fillAmount = player.trust / (float)PlayerStatus.MaxTrust;
        trustText.text = $"{player.trust} 신뢰도";
    }
}