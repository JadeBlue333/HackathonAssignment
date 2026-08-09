using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToThisScene : MonoBehaviour
{
    [Header("Scene")]
    public string sceneName;


    // =========================================================
    // Fade
    // =========================================================

    [Header("Fade")]
    public Image blackImage;
    public float fadeDuration = 1f;
    public float blackHoldTime = 0.25f;


    // =========================================================
    // BGM
    // =========================================================

    [Header("BGM (선택사항)")]
    [Tooltip("연결되어 있으면 씬 전환 시 BGM이 페이드아웃됩니다.")]
    public AudioSource bgmSource;

    [Tooltip("BGM 페이드아웃 시간")]
    public float bgmFadeDuration = 1f;


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

                // BGM 페이드아웃 시작
                StartBGMFadeOut();

                yield return new WaitForSeconds(
                    fadeDuration
                );

                // 검은 화면 유지
                yield return new WaitForSeconds(
                    blackHoldTime
                );

                // 저장된 날짜와 현재 날짜가 다르면 Progress Snapshot 저장
                if (PlayerStatus.Instance.IsProgressSnapshotDifferentDay())
                {
                    PlayerStatus.Instance.SaveProgressSnapshot();
                }
                // 저장된 날짜와 현재 날짜가 같으면 Progress Snapshot 불러옴
                else
                {
                    PlayerStatus.Instance.LoadProgressSnapshot();
                }

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

            // BGM 페이드아웃 시작
            StartBGMFadeOut();

            yield return new WaitForSeconds(
                fadeDuration
            );

            // 검은 화면 유지
            yield return new WaitForSeconds(
                blackHoldTime
            );


            // =================================================
            // PlayerStatus 확인
            // =================================================

            if (PlayerStatus.Instance == null)
            {
                Debug.LogError(
                    "PlayerStatus.Instance가 없습니다."
                );

                yield break;
            }


            // =================================================
            // 정산 전 날짜 확인
            // =================================================

            // D-Day인지 미리 저장
            // NextDay()를 실행하면 currentDay가 변경되기 때문
            bool isFinalDay =
                PlayerStatus.Instance.currentDay == 0;


            // 블랙마켓 날짜인지 확인
            bool goToBlackMarket =
                PlayerStatus.Instance.currentDay == 7 ||
                PlayerStatus.Instance.currentDay == 5 ||
                PlayerStatus.Instance.currentDay == 1;


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
                    PlayerStatus.Instance.AddFuel(30);

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
            // 5. 다음 날 시작 효과
            //
            // Fuel Recovery
            // Trust Recovery
            // Work Time 재계산
            // =================================================

            PlayerStatus.Instance.StartDay();


            // =================================================
            // 6. 다음 씬 이동
            // =================================================

            // -------------------------------------------------
            // D-Day 정산
            // -------------------------------------------------

            if (isFinalDay)
            {
                // 인간화된 경우
                if (PlayerStatus.Instance.isHuman)
                {
                    Debug.Log(
                        "D-Day 정산 완료 → DATE"
                    );

                    SceneManager.LoadScene(
                        "DATE"
                    );
                }

                // 인간화되지 않았고
                // 최종 신뢰도가 70 이상인 경우
                else if (PlayerStatus.Instance.trust >= 70)
                {
                    Debug.Log(
                        "D-Day 정산 완료 → PromoteTransition"
                    );

                    SceneManager.LoadScene(
                        "PromoteTransition"
                    );
                }

                // 인간화도 아니고
                // 신뢰도도 70 미만인 경우
                else
                {
                    Debug.Log(
                        "D-Day 정산 완료 → Ending2"
                    );

                    SceneManager.LoadScene(
                        "Ending2"
                    );
                }
            }


            // -------------------------------------------------
            // Black Market 날짜
            // -------------------------------------------------

            else if (goToBlackMarket)
            {
                Debug.Log(
                    "Black Market 날짜 → BlackMarketTransition"
                );

                SceneManager.LoadScene(
                    "BlackMarketTransition"
                );
            }


            // -------------------------------------------------
            // 일반 날짜
            // -------------------------------------------------

            else
            {
                SceneManager.LoadScene(
                    sceneName
                );
            }
        }


        // =====================================================
        // Normal Scene Change
        // =====================================================

        else
        {
            StartCoroutine(FadeIn());

            // BGM 페이드아웃 시작
            StartBGMFadeOut();

            yield return new WaitForSeconds(
                fadeDuration
            );

            // 검은 화면 유지
            yield return new WaitForSeconds(
                blackHoldTime
            );

            SceneManager.LoadScene(
                sceneName
            );
        }
    }


    // =========================================================
    // BGM Fade Out
    // =========================================================

    private void StartBGMFadeOut()
    {
        // BGM이 연결되어 있지 않으면 아무것도 하지 않음
        if (bgmSource == null)
        {
            return;
        }

        // 재생 중이 아니면 아무것도 하지 않음
        if (!bgmSource.isPlaying)
        {
            return;
        }

        StartCoroutine(
            FadeOutBGM()
        );
    }


    private IEnumerator FadeOutBGM()
    {
        float startVolume =
            bgmSource.volume;

        float t = 0f;


        // 페이드 시간에 0을 넣어도 오류 없이 처리
        if (bgmFadeDuration <= 0f)
        {
            bgmSource.volume = 0f;

            bgmSource.Stop();

            bgmSource.volume =
                startVolume;

            yield break;
        }


        while (t < bgmFadeDuration)
        {
            // AudioSource가 삭제되었으면 종료
            if (bgmSource == null)
            {
                yield break;
            }

            t += Time.deltaTime;

            bgmSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t / bgmFadeDuration
                );

            yield return null;
        }


        bgmSource.volume = 0f;

        bgmSource.Stop();

        // 혹시 같은 AudioSource를 다시 사용할 경우를 위해
        // 원래 볼륨으로 복구
        bgmSource.volume =
            startVolume;
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