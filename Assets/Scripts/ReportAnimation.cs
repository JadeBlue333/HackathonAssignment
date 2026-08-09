using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportAnimation : MonoBehaviour
{
    [Header("���� �׸��� ��� �� ����Ʈ�� �߰��� ��. ������� ������ �ȴ�.")]
    [SerializeField] private List<GameObject> reportItems = new();

    [Header("Notice")]
    [SerializeField] private GameObject notice;
    [SerializeField] private int blinkCount = 4;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Next Button")]
    [SerializeField] private GameObject nextButton;

    [Header("������ ���� ����")]
    [SerializeField] private float interval = 0.5f;

    private void Start()
    {
        // ó������ ��� ����
        foreach (GameObject item in reportItems)
        {
            if (item != null)
                item.SetActive(false);
        }

        if (notice != null)
            notice.SetActive(false);

        if (nextButton != null)
            nextButton.SetActive(false);

        StartCoroutine(ShowReport());
    }

    private IEnumerator ShowReport()
    {
        // Notice �����̱�
        if (notice != null)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                notice.SetActive(!notice.activeSelf);
                yield return new WaitForSeconds(blinkInterval);
            }

            // ���������� ���� ���� ����
            notice.SetActive(true);
        }

        for (int i = 0; i < reportItems.Count; i++)
        {
            if (reportItems[i] != null)
                reportItems[i].SetActive(true);

            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(0.2f);

        if (nextButton != null)
            nextButton.SetActive(true);
    }
}