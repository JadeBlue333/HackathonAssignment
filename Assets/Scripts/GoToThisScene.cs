using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToThisScene : MonoBehaviour
{
    [Header("Scene")]
    public string sceneName;

    [Header("Fade")]
    public Image blackImage;
    public float fadeDuration = 1f;


    // =========================================================
    // Progress
    // =========================================================

    [Header("Progress 화면에서 사용하는 경우 true")]
    public bool progress = false;

    public int buttonDay = 9;


    // =========================================================
    // Report
    // =========================================================

    [Header("Report 화면에서 사용하는 경우 true")]
    public bool report = false;

    public ReportUI reportUI;


    // =========================================================
    // Button
    // =========================================================

    public void nextSceneButton()
    {
        StartCoroutine(nextScene());
    }


    // =========================================================
    // Scene Change
    // =========================================================

    public IEnumerator nextScene()
    {
        // =====================================================
        // Progress
        // =====================================================

        if (progress)
        {
            if (PlayerStatus.Instance != null &&
                PlayerStatus.Instance.currentDay == buttonDay)
            {
                StartCoroutine(FadeIn());

                yield return new WaitForSeconds(
                    fadeDuration
                );

                SceneManager.LoadScene(
                    sceneName
                );
            }
            else
            {
                Debug.Log(
                    "이미 지난 날짜입니다."
                );
            }
        }


        // =====================================================
        // Report
        // =====================================================

        else if (report)
        {
            StartCoroutine(FadeIn());

            yield return new WaitForSeconds(
                fadeDuration
            );


            if (PlayerStatus.Instance == null)
            {
                Debug.LogError(
                    "PlayerStatus.Instance가 없습니다."
                );

                yield break;
            }


            // =================================================
            // 1. 오늘 번 돈 적용
            // =================================================

            PlayerStatus.Instance.ApplyEarnings();


            // =================================================
            // 2. 오늘 신뢰도 변화 적용
            // =================================================

            PlayerStatus.Instance.ApplyTrustChanges();


            // =================================================
            // 3. 연료 구매
            //
            // 연료 +30
            // 돈 -20
            // =================================================

            if (reportUI != null &&
                reportUI.fuelToggle != null &&
                reportUI.fuelToggle.isOn)
            {
                // 돈이 충분할 때만 구매
                if (PlayerStatus.Instance.SpendMoney(20))
                {
                    PlayerStatus.Instance.AddFuel(
                        30
                    );

                    Debug.Log(
                        "연료 구매 완료 / 연료 +30"
                    );
                }
                else
                {
                    Debug.Log(
                        "연료 구매 실패 / 돈 부족"
                    );
                }
            }


            // =================================================
            // 4. 날짜 변경
            // =================================================

            PlayerStatus.Instance.NextDay();


            // =================================================
            // 5. 다음 날 시작 효과 적용
            //
            // Fuel Recovery
            // Trust Recovery
            // Work Time 재계산
            // =================================================

            PlayerStatus.Instance.StartDay();


            // =================================================
            // 6. 다음 씬 이동
            // =================================================

            SceneManager.LoadScene(
                sceneName
            );
        }


        // =====================================================
        // Normal Scene Change
        // =====================================================

        else
        {
            StartCoroutine(FadeIn());

            yield return new WaitForSeconds(
                fadeDuration
            );

            SceneManager.LoadScene(
                sceneName
            );
        }
    }


    // =========================================================
    // Fade
    // =========================================================

    public IEnumerator FadeIn()
    {
        if (blackImage == null)
        {
            yield break;
        }


        float t = 0f;

        Color color =
            blackImage.color;


        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    0f,
                    1f,
                    t / fadeDuration
                );

            blackImage.color =
                color;

            yield return null;
        }


        color.a = 1f;

        blackImage.color =
            color;
    }
}