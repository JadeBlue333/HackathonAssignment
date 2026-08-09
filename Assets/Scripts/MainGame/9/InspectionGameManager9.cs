using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InspectionGameManager9 : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private BoxMatchManager boxMatchManager;
    [SerializeField] private MotorMatchManager9 motorMatchManager;


    // =========================================================
    // Box / Motor
    // =========================================================

    [Header("Box / Motor")]
    public GameObject boxButtons;
    public GameObject motorButtons;


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

    // 정상 박스를 잘못 열었는지
    private bool openedWrongBox = false;

    private InspectionResult currentAnswer;


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
        yield return new WaitUntil(
            () =>
                boxMatchManager.IsReady &&
                motorMatchManager.IsReady
        );


        // PlayerStatus에 기존 콤보가 있다면 가져오기
        if (PlayerStatus.Instance != null)
        {
            comboCount =
                PlayerStatus.Instance.comboNumber;
        }


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
        // Box 선택 단계
        // =====================================================

        if (boxButtons != null &&
            boxButtons.activeSelf)
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
        // =====================================================

        if (motorButtons != null &&
            motorButtons.activeSelf)
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
        if (audioSource == null ||
            clip == null)
        {
            return;
        }


        audioSource.PlayOneShot(
            clip,
            volume
        );
    }


    // =========================================================
    // Generate Question
    // =========================================================

    private void GenerateQuestion()
    {
        openedWrongBox =
            false;


        currentAnswer =
            boxMatchManager.CreateNextMatch();


        PlaySfx(
            spawnBoxSfx,
            spawnBoxVolume
        );


        StartCoroutine(
            NextQuestionDelay()
        );


        Debug.Log(
            "=========================="
        );

        Debug.Log(
            "새 문제 생성"
        );

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


        // 미개봉 박스를 잘못 열었는지 기록
        openedWrongBox =
            currentAnswer ==
            InspectionResult.Unopened;


        // 박스 제거
        boxMatchManager
            .RemoveCurrentMatch();


        // 모터 생성
        currentAnswer =
            motorMatchManager
                .CreateNextMatch();


        Debug.Log(
            $"박스 개봉 완료 / 잘못 연 박스: {openedWrongBox}"
        );


        Debug.Log(
            $"내부 정답: {currentAnswer}"
        );


        // 모터 선택 가능
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


            Debug.Log(
                "미개봉 박스를 개봉함 → " +
                "모터 선택과 관계없이 오답"
            );
        }

        else
        {
            correct =
                playerAnswer ==
                currentAnswer;
        }


        Debug.Log(
            $"선택 : {playerAnswer}"
        );

        Debug.Log(
            $"정답 : {currentAnswer}"
        );


        // =====================================================
        // 정답
        // =====================================================

        if (correct)
        {
            PlaySfx(
                correctSfx,
                correctVolume
            );


            // =================================================
            // 정답 통계
            // =================================================

            correctCount++;


            // =================================================
            // 콤보 증가
            // =================================================

            comboCount++;


            // PlayerStatus에도 현재 연속 성공 수 저장
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


            Debug.Log(
                $"정답! +{reward}"
            );


            Debug.Log(
                $"현재 콤보 : {comboCount}"
            );


            // =================================================
            // 콤보 신뢰도 보너스
            //
            // 1콤보 → 없음
            // 2콤보 → 없음
            // 3콤보 → +2
            // 4콤보 → +2
            // 5콤보 → +2
            // ...
            //
            // 실패할 때까지 계속 적용
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


                Debug.Log(
                    $"★★★ {comboCount} COMBO! " +
                    $"신뢰도 +{comboTrustReward} ★★★"
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


            // =================================================
            // 오답 통계
            // =================================================

            wrongCount++;


            PlayerStatus.Instance
                .mistakeNumber++;


            // =================================================
            // 콤보 초기화
            // =================================================

            comboCount =
                0;


            PlayerStatus.Instance.comboNumber =
                0;


            // =================================================
            // 신뢰도 패널티
            // =================================================

            PlayerStatus.Instance
                .AddTrustChanges(
                    -trustPenalty
                );


            Debug.Log(
                $"오답! 신뢰도 -{trustPenalty}"
            );


            Debug.Log(
                "콤보가 0으로 초기화되었습니다."
            );
        }


        // =====================================================
        // 연료 소비
        // =====================================================

        PlayerStatus.Instance
            .ReduceFuel(
                fuelCost
            );


        Debug.Log(
            $"연료 -{fuelCost}"
        );


        // =====================================================
        // 통계
        // =====================================================

        Debug.Log(
            $"[검사 통계] " +
            $"정답: {correctCount} / " +
            $"오답: {wrongCount} / " +
            $"현재 콤보: {comboCount}"
        );


        // =====================================================
        // Game Over
        // =====================================================

        if (
            PlayerStatus.Instance.trust <= 0
        )
        {
            Debug.Log(
                "GAME OVER - 신뢰도 부족"
            );

            return;
        }


        if (
            PlayerStatus.Instance.fuel <= 0
        )
        {
            Debug.Log(
                "GAME OVER - 연료 부족"
            );

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