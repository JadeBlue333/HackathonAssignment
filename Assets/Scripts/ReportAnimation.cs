using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportAnimation : MonoBehaviour
{
    [Header("정산 항목을 모두 이 리스트에 추가할 것. 순서대로 나오게 된다.")]
    [SerializeField] private List<GameObject> reportItems = new();

    [Header("Next Button")]
    [SerializeField] private GameObject nextButton;

    [Header("나오는 간격 조절")]
    [SerializeField] private float interval = 0.5f;

    private void Start()
    {
        // 처음에는 모두 숨김
        foreach (GameObject item in reportItems)
        {
            if (item != null)
                item.SetActive(false);
        }

        nextButton.SetActive(false);

        StartCoroutine(ShowReport());
    }

    private IEnumerator ShowReport()
    {
        for (int i = 0; i < reportItems.Count; i++)
        {
            if (reportItems[i] != null)
                reportItems[i].SetActive(true);

            // 마지막 항목이면 버튼도 같이 표시
            if (i == reportItems.Count - 1 && nextButton != null)
            {
                nextButton.SetActive(true);
            }

            yield return new WaitForSeconds(interval);
        }
    }
}