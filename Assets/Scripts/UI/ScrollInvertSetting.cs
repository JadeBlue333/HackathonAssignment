using UnityEngine;
using UnityEngine.UI;

public class ScrollInvertSetting : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Toggle invertToggle;

    public const string ScrollInvertKey = "ScrollInvert";

    private void Start()
    {
        if (invertToggle == null)
            return;

        // 저장된 값 불러오기
        bool isInverted =
            PlayerPrefs.GetInt(
                ScrollInvertKey,
                0
            ) == 1;

        // 이벤트 발생 없이 초기값 적용
        invertToggle.SetIsOnWithoutNotify(
            isInverted
        );

        invertToggle.onValueChanged.AddListener(
            OnToggleChanged
        );
    }

    private void OnDestroy()
    {
        if (invertToggle != null)
        {
            invertToggle.onValueChanged.RemoveListener(
                OnToggleChanged
            );
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(
            ScrollInvertKey,
            isOn ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    // 다른 스크립트에서 사용
    public static bool IsScrollInverted()
    {
        return PlayerPrefs.GetInt(
            ScrollInvertKey,
            0
        ) == 1;
    }
}