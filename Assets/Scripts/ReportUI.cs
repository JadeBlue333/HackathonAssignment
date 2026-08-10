using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportUI : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] private TMP_Text originalMoneyText;
    [SerializeField] private TMP_Text earnedMoneyText;
    [SerializeField] private TMP_Text fuelLeftText;
    [SerializeField] private TMP_Text finalMoneyText;

    [Header("Trust")]
    [SerializeField] private TMP_Text comboNum;
    [SerializeField] private TMP_Text mistakeNum;
    [SerializeField] private TMP_Text finalTrustText;

    [Header("Fuel Purchase")]
    public Toggle fuelToggle;

    private const int FuelPrice = 20;
    private const int FuelAmount = 30;

    private void Start()
    {
        fuelToggle.onValueChanged.AddListener(OnFuelToggleChanged);
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (fuelToggle != null)
            fuelToggle.onValueChanged.RemoveListener(OnFuelToggleChanged);
    }

    private void OnFuelToggleChanged(bool isOn)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        PlayerStatus ps = PlayerStatus.Instance;

        int fuel = ps.fuel;
        int totalMoney = ps.money + ps.earnings;

        // 현재 보유 가능한 돈
        int availableMoney = ps.money + ps.earnings;

        // 돈이 FuelPrice보다 많을 때만 토글 가능
        if (fuelToggle != null)
        {
            fuelToggle.interactable = availableMoney >= FuelPrice;

            /*
            if (availableMoney < FuelPrice && fuelToggle.isOn)
            {
                fuelToggle.isOn = false;
            }
            */
        }

        // 연료 구매 선택
        if (fuelToggle != null && fuelToggle.isOn)
        {
            fuel += FuelAmount;
            totalMoney -= FuelPrice;
        }

        fuelLeftText.text = $"남은 연료: {fuel - 10}";

        originalMoneyText.text = $"{ps.money}C";
        earnedMoneyText.text = $"{ps.earnings}C";
        finalMoneyText.text = $"{totalMoney}C";

        comboNum.text = $"{ps.comboNumber * 2}";
        mistakeNum.text = $"{ps.mistakeNumber * -5}";
        finalTrustText.text = $"{ps.trust + ps.trustChange}";
    }
}