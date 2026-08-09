using UnityEngine;
using UnityEngine.Rendering;

public class LowFuelNoiseEffect : MonoBehaviour
{
    // =========================================================
    // Settings
    // =========================================================

    [Header("Fuel Settings")]

    [Tooltip("이 연료량 이하일 때 노이즈 효과 활성화")]
    [SerializeField]
    private int fuelThreshold = 35;


    // =========================================================
    // Volume
    // =========================================================

    [Header("Volume")]

    [Tooltip("Interferences가 들어있는 Volume")]
    [SerializeField]
    private Volume noiseVolume;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (noiseVolume == null)
        {
            noiseVolume =
                GetComponent<Volume>();
        }

        Refresh();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        Refresh();
    }


    // =========================================================
    // Refresh
    // =========================================================

    private void Refresh()
    {
        if (PlayerStatus.Instance == null)
            return;

        if (noiseVolume == null)
            return;


        bool lowFuel =
            PlayerStatus.Instance.fuel <= fuelThreshold;


        if (lowFuel)
        {
            noiseVolume.weight = 1f;
        }
        else
        {
            noiseVolume.weight = 0f;
        }
    }
}