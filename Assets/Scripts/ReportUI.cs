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

        if (fuelToggle != null && fuelToggle.isOn)
        {
            fuel += FuelAmount;
            totalMoney -= FuelPrice;
        }

        fuelLeftText.text = $"남은 연료: {fuel-10}";

        originalMoneyText.text = $"{ps.money}C";
        earnedMoneyText.text = $"{ps.earnings}C";
        finalMoneyText.text = $"{totalMoney}C";

        comboNum.text = $"{ps.comboNumber * 2}";
        mistakeNum.text = $"{ps.mistakeNumber * -5}";
        finalTrustText.text = $"{ps.trust + ps.trustChange}";
    }
}