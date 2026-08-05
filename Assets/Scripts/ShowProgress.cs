using UnityEngine;
using UnityEngine.UI;

public class ShowProgress : MonoBehaviour
{
    [Header("D-9부터 D-Day 순서대로 넣기")]
    [SerializeField] private Button[] dayButtons;

    private void Start()
    {
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        // 현재 날짜 (9 ~ 0)
        int currentDay = PlayerStatus.Instance.currentDay;

        // 현재 날짜에 따라 보여줄 버튼 개수
        // D-9(9) -> 1개
        // D-8(8) -> 2개
        // ...
        // D-Day(0) -> 10개
        int showCount = 10 - currentDay;

        for (int i = 0; i < dayButtons.Length; i++)
        {
            dayButtons[i].gameObject.SetActive(i < showCount);
        }
    }
}