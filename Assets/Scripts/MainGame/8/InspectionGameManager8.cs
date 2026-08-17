using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class InspectionGameManager8 : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private BoxMatchManager boxMatchManager;
    [SerializeField] private MotorMatchManager8 motorMatchManager;


    // =========================================================
    // Box / Motor
    // =========================================================

    [Header("Box / Motor")]
    public GameObject boxButtons;
    public GameObject motorButtons;


    // =========================================================
    // Wrong Answer UI
    // =========================================================

    [Header("Wrong Answer UI")]

    [Tooltip("오답 사유를 표시할 UI")]
    [SerializeField]
    private GameObject wrongReasonUI;

    [Tooltip("오답 사유 텍스트")]
    [SerializeField]
    private TMP_Text wrongReasonText;

    [Tooltip("오답 사유를 표시할 시간")]
    [SerializeField]
    private float wrongReasonDuration = 2f;

    [Tooltip("오답 사유가 사라질 때 페이드아웃되는 시간")]
    [SerializeField]
    private float wrongReasonFadeOutDuration = 0.5f;

    private CanvasGroup wrongReasonCanvasGroup;


    // =========================================================
    // Reward
    // =========================================================

    [Header("Reward")]

    [SerializeField] private int unopenedReward = 3;
    [SerializeField] private int aReward = 2;
    [SerializeField] private int bReward = 1;
    [SerializeField] private int cReward = 1;
    [SerializeField] private int discardReward = 0;


    // =========================================================
    // Penalty
    // =========================================================

    [Header("Penalty")]

    [SerializeField] private int trustPenalty = 5;
    [SerializeField] private int fuelCost = 2;


    // =========================================================
    // Combo
    // =========================================================

    [Header("Combo")]

    [Tooltip("이 콤보 이상부터 정답 시 신뢰도 보너스")]
    [SerializeField] private int trustComboThreshold = 3;

    [Tooltip("콤보 보너스로 얻는 신뢰도")]
    [SerializeField] private int comboTrustReward = 2;


    // =========================================================
    // Fail Effect
    // =========================================================

    [Header("Fail Effect")]
    [SerializeField]
    private InspectionFailEffect failEffect;


    // =========================================================
    // Sound
    // =========================================================

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;


    [Header("Spawn Box SFX")]
    [SerializeField] private AudioClip spawnBoxSfx;

    [Range(0f, 1f)]
    [SerializeField] private float spawnBoxVolume = 1f;


    [Header("Open Box SFX")]
    [SerializeField] private AudioClip openBoxSfx;

    [Range(0f, 1f)]
    [SerializeField] private float openBoxVolume = 1f;


    [Header("Correct SFX")]
    [SerializeField] private AudioClip correctSfx;

    [Range(0f, 1f)]
    [SerializeField] private float correctVolume = 1f;


    [Header("Wrong SFX")]
    [SerializeField] private AudioClip wrongSfx;

    [Range(0f, 1f)]
    [SerializeField] private float wrongVolume = 1f;


    // =========================================================
    // Runtime
    // =========================================================

    // 미개봉 박스를 잘못 열었는지
    private bool openedWrongBox = false;

    private InspectionResult currentAnswer;

    private string currentWrongReason = "";

    private Coroutine wrongReasonCoroutine;


    // =========================================================
    // Combo / Statistics
    // =========================================================

    private int correctCount = 0;
    private int wrongCount = 0;

    private int comboCount = 0;


    // =========================================================
    // Start
    // =========================================================

    private IEnumerator Start()
    {
        if (wrongReasonUI != null)
        {
            wrongReasonCanvasGroup =
                wrongReasonUI.GetComponent<CanvasGroup>();

            if (wrongReasonCanvasGroup == null)
            {
                wrongReasonCanvasGroup =
                    wrongReasonUI.AddComponent<CanvasGroup>();
            }


            wrongReasonCanvasGroup.alpha =
                1f;


            wrongReasonUI.SetActive(
                false
            );
        }


        yield return new WaitUntil(
            () =>
                boxMatchManager.IsReady &&
                motorMatchManager.IsReady
        );


        comboCount = PlayerStatus.Instance.comboNumber;

        GenerateQuestion();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;


        // =====================================================
        // 팝업이 떠 있는 동안 검수 단축키 차단
        // =====================================================

        if (
            PopupManager.Instance != null &&
            PopupManager.Instance.HasOpenPopup()
        )
        {
            return;
        }


        // =====================================================
        // Box 선택 단계
        //
        // 1 = 미개봉
        // 2 = 개봉
        // =====================================================

        if (
            boxButtons != null &&
            boxButtons.activeSelf
        )
        {
            if (
                Keyboard.current
                    .digit1Key
                    .wasPressedThisFrame
            )
            {
                SelectUnopened();
            }

            else if (
                Keyboard.current
                    .digit2Key
                    .wasPressedThisFrame
            )
            {
                SelectOpened();
            }


            return;
        }


        // =====================================================
        // Motor 선택 단계
        //
        // 1 = A
        // 2 = B
        // 3 = C
        // 4 = 폐기
        // =====================================================

        if (
            motorButtons != null &&
            motorButtons.activeSelf
        )
        {
            if (
                Keyboard.current
                    .digit1Key
                    .wasPressedThisFrame
            )
            {
                SelectA();
            }

            else if (
                Keyboard.current
                    .digit2Key
                    .wasPressedThisFrame
            )
            {
                SelectB();
            }

            else if (
                Keyboard.current
                    .digit3Key
                    .wasPressedThisFrame
            )
            {
                SelectC();
            }

            else if (
                Keyboard.current
                    .digit4Key
                    .wasPressedThisFrame
            )
            {
                SelectDiscard();
            }
        }
    }


    // =========================================================
    // Sound
    // =========================================================

    private void PlaySfx(
        AudioClip clip,
        float volume
    )
    {
        if (
            audioSource == null ||
            clip == null
        )
        {
            return;
        }


        audioSource.PlayOneShot(
            clip,
            volume
        );
    }


    // =========================================================
    // Wrong Reason
    // =========================================================

    public void SetCurrentWrongReason(
        string reason
    )
    {
        currentWrongReason =
            reason;
    }


    public void AddCurrentWrongReason(
        string reason
    )
    {
        if (string.IsNullOrEmpty(reason))
            return;


        if (string.IsNullOrEmpty(currentWrongReason))
        {
            currentWrongReason =
                reason;
        }
        else
        {
            currentWrongReason +=
                "\n" +
                reason;
        }
    }


    private void ClearCurrentWrongReason()
    {
        currentWrongReason =
            "";
    }


    // =========================================================
    // Box Wrong Reason
    // =========================================================

    private void SetBoxWrongReasons()
    {
        ClearCurrentWrongReason();


        // 박스 손상
        if (boxMatchManager.CurrentBoxDamaged)
        {
            AddCurrentWrongReason(
                "박스가 손상되어 있습니다."
            );
        }


        // 테이프 개봉 흔적
        if (boxMatchManager.CurrentTapeOpened)
        {
            AddCurrentWrongReason(
                "테이프에 개봉 흔적이 있습니다."
            );
        }


        // 정상 박스 + 정상 테이프
        if (
            !boxMatchManager.CurrentBoxDamaged &&
            !boxMatchManager.CurrentTapeOpened
        )
        {
            AddCurrentWrongReason(
                "박스와 테이프에 개봉 흔적이 없습니다."
            );
        }
    }


    // =========================================================
    // Motor Wrong Reason
    // =========================================================

    private void SetMotorWrongReasons()
    {
        ClearCurrentWrongReason();


        // =====================================================
        // 폐기 대상
        //
        // 프로펠러가 누락되어 있으면
        // 다른 조건은 표시하지 않음
        // =====================================================

        if (!motorMatchManager.CurrentIsComplete)
        {
            SetCurrentWrongReason(
                "[폐기 필요] 프로펠러가 누락되어 있습니다."
            );

            return;
        }


        // =====================================================
        // 일반 하자
        //
        // Day8:
        // 색 같음 → A
        // 색 다름 → B
        // =====================================================

        if (motorMatchManager.CurrentSameColor)
        {
            SetCurrentWrongReason(
                "프로펠러 앞뒤 색상이 일치합니다."
            );
        }
        else
        {
            SetCurrentWrongReason(
                "프로펠러 앞뒤 색상이 일치하지 않습니다."
            );
        }
    }


    // =========================================================
    // Wrong Reason UI
    // =========================================================

    private void ShowWrongReason(
        string reason
    )
    {
        if (wrongReasonText != null)
        {
            wrongReasonText.text =
                reason;
        }


        if (wrongReasonUI == null)
            return;


        if (wrongReasonCoroutine != null)
        {
            StopCoroutine(
                wrongReasonCoroutine
            );
        }


        wrongReasonCoroutine =
            StartCoroutine(
                ShowWrongReasonRoutine()
            );
    }


    private IEnumerator ShowWrongReasonRoutine()
    {
        wrongReasonUI.SetActive(
            true
        );


        if (wrongReasonCanvasGroup != null)
        {
            wrongReasonCanvasGroup.alpha =
                1f;
        }


        // =====================================================
        // 일반 표시 시간
        // =====================================================

        yield return new WaitForSecondsRealtime(
            wrongReasonDuration
        );


        // =====================================================
        // Fade Out
        // =====================================================

        if (
            wrongReasonCanvasGroup != null &&
            wrongReasonFadeOutDuration > 0f
        )
        {
            float elapsed =
                0f;


            while (
                elapsed <
                wrongReasonFadeOutDuration
            )
            {
                elapsed +=
                    Time.unscaledDeltaTime;


                float t =
                    Mathf.Clamp01(
                        elapsed /
                        wrongReasonFadeOutDuration
                    );


                wrongReasonCanvasGroup.alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t
                    );


                yield return null;
            }


            wrongReasonCanvasGroup.alpha =
                0f;
        }


        wrongReasonUI.SetActive(
            false
        );


        // 다음 표시를 위해 복구
        if (wrongReasonCanvasGroup != null)
        {
            wrongReasonCanvasGroup.alpha =
                1f;
        }


        wrongReasonCoroutine =
            null;
    }


    // =========================================================
    // Generate Question
    // =========================================================

    private void GenerateQuestion()
    {
        openedWrongBox =
            false;


        ClearCurrentWrongReason();


        currentAnswer =
            boxMatchManager.CreateNextMatch();


        // 현재 박스 상태로 오답 사유 준비
        SetBoxWrongReasons();


        PlaySfx(
            spawnBoxSfx,
            spawnBoxVolume
        );


        StartCoroutine(
            NextQuestionDelay()
        );


        // 디버깅용 정답만 표시
        Debug.Log(
            $"정답 : {currentAnswer}"
        );
    }


    private IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(
            0.5f
        );


        if (boxButtons != null)
        {
            boxButtons.SetActive(
                true
            );
        }
    }


    // =========================================================
    // Box Selection
    // =========================================================

    public void SelectUnopened()
    {
        CheckAnswer(
            InspectionResult.Unopened
        );


        if (boxButtons != null)
        {
            boxButtons.SetActive(
                false
            );
        }
    }


    public void SelectOpened()
    {
        PlaySfx(
            openBoxSfx,
            openBoxVolume
        );


        if (boxButtons != null)
        {
            boxButtons.SetActive(
                false
            );
        }


        // =====================================================
        // 미개봉 박스를 잘못 열었는지 기록
        // =====================================================

        openedWrongBox =
            currentAnswer ==
            InspectionResult.Unopened;


        if (openedWrongBox)
        {
            SetCurrentWrongReason(
                "미개봉 제품을 개봉했습니다."
            );
        }


        // =====================================================
        // 박스 제거
        // =====================================================

        boxMatchManager
            .RemoveCurrentMatch();


        // =====================================================
        // 모터 생성
        // =====================================================

        currentAnswer =
            motorMatchManager
                .CreateNextMatch();


        // =====================================================
        // 모터 오답 사유 준비
        //
        // 미개봉 제품을 잘못 개봉한 경우에는
        // 해당 사유를 그대로 유지
        // =====================================================

        if (!openedWrongBox)
        {
            SetMotorWrongReasons();
        }


        // 디버깅용 내부 정답만 표시
        Debug.Log(
            $"정답 : {currentAnswer}"
        );


        // =====================================================
        // 모터 선택 가능
        // =====================================================

        if (motorButtons != null)
        {
            motorButtons.SetActive(
                true
            );
        }
    }


    // =========================================================
    // Motor Selection
    // =========================================================

    public void SelectA()
    {
        if (motorButtons != null)
        {
            motorButtons.SetActive(
                false
            );
        }


        CheckAnswer(
            InspectionResult.A
        );


        motorMatchManager
            .RemoveCurrentMotor();
    }


    public void SelectB()
    {
        if (motorButtons != null)
        {
            motorButtons.SetActive(
                false
            );
        }


        CheckAnswer(
            InspectionResult.B
        );


        motorMatchManager
            .RemoveCurrentMotor();
    }


    public void SelectC()
    {
        if (motorButtons != null)
        {
            motorButtons.SetActive(
                false
            );
        }


        CheckAnswer(
            InspectionResult.C
        );


        motorMatchManager
            .RemoveCurrentMotor();
    }


    public void SelectDiscard()
    {
        if (motorButtons != null)
        {
            motorButtons.SetActive(
                false
            );
        }


        CheckAnswer(
            InspectionResult.Discard
        );


        motorMatchManager
            .RemoveCurrentMotor();
    }


    // =========================================================
    // Answer Check
    // =========================================================

    private void CheckAnswer(
        InspectionResult playerAnswer
    )
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogError(
                "PlayerStatus.Instance가 없습니다."
            );

            return;
        }


        bool correct;


        // =====================================================
        // 정답 판정
        // =====================================================

        // 미개봉 박스를 잘못 열었다면
        // 내부 선택이 맞아도 최종적으로 오답
        if (openedWrongBox)
        {
            correct =
                false;
        }
        else
        {
            correct =
                playerAnswer ==
                currentAnswer;
        }


        // =====================================================
        // 정답
        // =====================================================

        if (correct)
        {
            PlaySfx(
                correctSfx,
                correctVolume
            );


            // 정답 통계
            correctCount++;


            // 콤보 증가
            comboCount++;
            PlayerStatus.Instance.successNumber++;

            PlayerStatus.Instance.comboNumber =
                comboCount;


            // =================================================
            // 수익
            // =================================================

            int reward =
                GetReward(
                    currentAnswer
                );


            PlayerStatus.Instance
                .AddEarnings(
                    reward
                );


            // =================================================
            // 콤보 신뢰도 보너스
            //
            // 1콤보 → 없음
            // 2콤보 → 없음
            // 3콤보 이상 → 정답마다 +2
            // =================================================

            if (
                comboCount >=
                trustComboThreshold
            )
            {
                PlayerStatus.Instance
                    .AddTrustChanges(
                        comboTrustReward
                    );
            }
        }


        // =====================================================
        // 오답
        // =====================================================

        else
        {
            PlaySfx(
                wrongSfx,
                wrongVolume
            );


            // 오답 통계
            wrongCount++;
            PlayerStatus.Instance.mistakeNumber++;


            // 콤보 초기화
            comboCount =
                0;


            PlayerStatus.Instance.comboNumber =
                0;


            // =================================================
            // 신뢰도 패널티
            // =================================================
            if (PlayerStatus.Instance.humanHead)
            {
                trustPenalty = -2;
            }
            else
            {
                trustPenalty = -5;
            }

            PlayerStatus.Instance
                .AddTrustChanges(
                    trustPenalty
                );


            // =================================================
            // 오답 사유 표시
            // =================================================

            string reason =
                currentWrongReason;


            if (string.IsNullOrEmpty(reason))
            {
                reason =
                    "검수 기준과 일치하지 않습니다.";
            }


            ShowWrongReason(
                reason
            );

            // 폐기를 미폐기 처리했을시 폭발 트리거
            if (currentAnswer == InspectionResult.Discard && playerAnswer != InspectionResult.Discard)
            {
                //trigger explosion event
                if (failEffect != null)
                {
                    failEffect.Play();
                }
            }
        }


        // =====================================================
        // 연료 소비
        // =====================================================

        if (PlayerStatus.Instance.humanBody)
        {
            fuelCost = 1;
        }
        else
        {
            fuelCost = 2;
        }

        PlayerStatus.Instance
            .ReduceFuel(
                fuelCost
            );


        // =====================================================
        // Game Over
        // =====================================================

        if (
            PlayerStatus.Instance.trust <= 0
        )
        {
            return;
        }


        if (
            PlayerStatus.Instance.fuel <= 0
        )
        {
            return;
        }


        // =====================================================
        // 다음 문제
        // =====================================================

        GenerateQuestion();
    }


    // =========================================================
    // Reward
    // =========================================================

    private int GetReward(
        InspectionResult result
    )
    {
        switch (result)
        {
            case InspectionResult.Unopened:

                return unopenedReward;


            case InspectionResult.A:

                return aReward;


            case InspectionResult.B:

                return bReward;


            case InspectionResult.C:

                return cReward;


            case InspectionResult.Discard:

                return discardReward;
        }


        return 0;
    }
}