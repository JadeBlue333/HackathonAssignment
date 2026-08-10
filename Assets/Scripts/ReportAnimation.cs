using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReportAnimation : MonoBehaviour
{
    [Header("순서대로 넣어줄 정산 항목 다 넣기")]
    [SerializeField] private List<GameObject> reportItems = new();

    [Header("Notice")]
    [SerializeField] private GameObject notice;
    [SerializeField] private int blinkCount = 4;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Next Button")]
    [SerializeField] private GameObject nextButton;

    [Header("정산 항목 넘기는 간격")]
    [SerializeField] private float interval = 0.5f;

    [Header("Sound Effect")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reportItemSfx;

    private Coroutine reportCoroutine;
    private bool isShowingReport = true;
    private bool isSkipped = false;

    private void Start()
    {
        // 처음에는 모든 정산 항목 숨기기
        foreach (GameObject item in reportItems)
        {
            if (item != null)
                item.SetActive(false);
        }

        if (notice != null)
            notice.SetActive(false);

        if (nextButton != null)
            nextButton.SetActive(false);

        reportCoroutine = StartCoroutine(ShowReport());
    }

    private void Update()
    {
        // New Input System - 마우스 좌클릭 감지
        if (isShowingReport &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            SkipAnimation();
        }
    }

    private IEnumerator ShowReport()
    {
        // Notice 깜빡이기
        if (notice != null)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                notice.SetActive(!notice.activeSelf);
                yield return new WaitForSeconds(blinkInterval);
            }

            // 마지막에는 켜진 상태
            notice.SetActive(true);
        }

        // 정산 항목 순서대로 등장
        for (int i = 0; i < reportItems.Count; i++)
        {
            if (reportItems[i] != null)
            {
                reportItems[i].SetActive(true);

                // 항목이 나올 때마다 SFX
                PlayReportSfx();
            }

            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(0.2f);

        if (nextButton != null)
            nextButton.SetActive(true);

        isShowingReport = false;
    }

    private void SkipAnimation()
    {
        // 이미 스킵했다면 아무것도 하지 않음
        if (isSkipped)
            return;

        isSkipped = true;
        isShowingReport = false;

        // 현재 코루틴 중단
        if (reportCoroutine != null)
        {
            StopCoroutine(reportCoroutine);
            reportCoroutine = null;
        }

        // Notice는 최종 상태로
        if (notice != null)
            notice.SetActive(true);

        // 모든 정산 항목 한 번에 표시
        foreach (GameObject item in reportItems)
        {
            if (item != null)
                item.SetActive(true);
        }

        // 스킵할 때는 SFX 딱 한 번
        PlayReportSfx();

        // Next Button 표시
        if (nextButton != null)
            nextButton.SetActive(true);
    }

    private void PlayReportSfx()
    {
        if (audioSource != null && reportItemSfx != null)
        {
            audioSource.PlayOneShot(reportItemSfx);
        }
    }
}