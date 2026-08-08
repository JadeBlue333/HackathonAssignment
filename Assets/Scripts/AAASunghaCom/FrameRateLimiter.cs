using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [Header("Frame Rate")]
    [SerializeField] private int targetFrameRate = 60;

    private void Awake()
    {
        // VSync 비활성화
        QualitySettings.vSyncCount = 0;

        // 목표 FPS 제한
        Application.targetFrameRate = targetFrameRate;
    }
}