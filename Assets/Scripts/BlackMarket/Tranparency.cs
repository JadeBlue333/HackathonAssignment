using UnityEngine;

public class Tranparency : MonoBehaviour
{
    [Header("투명하게 만들 패널")]
    [SerializeField] private GameObject targetPanel;

    [Header("투명도 (%)")]
    [Range(0f, 100f)]
    [SerializeField] private float transparencyPercent = 50f;

    // 버튼 OnClick에 연결
    public void ApplyTransparency()
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("Target Panel이 지정되지 않았습니다.");
            return;
        }

        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();

        // 없으면 자동으로 추가
        if (canvasGroup == null)
        {
            canvasGroup = targetPanel.AddComponent<CanvasGroup>();
        }

        // 0% 투명 = 완전 불투명
        // 100% 투명 = 완전 투명
        canvasGroup.alpha = 1f - (transparencyPercent / 100f);
    }

    // 다시 완전 불투명하게
    public void ResetTransparency()
    {
        if (targetPanel == null)
            return;

        CanvasGroup canvasGroup = targetPanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }
}