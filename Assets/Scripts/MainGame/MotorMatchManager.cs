using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMatchManager : MonoBehaviour
{
    // =====================================================
    // Resources
    // =====================================================

    [Header("Resources")]

    [SerializeField]
    private string motorResourcePath =
        "Motor_new/Motor_Random";


    // =====================================================
    // Display
    // =====================================================

    [Header("Display")]

    [SerializeField]
    private Transform displayAnchor;


    // =====================================================
    // Transform
    // =====================================================

    [Header("Transform")]

    [SerializeField]
    private Vector3 motorLocalPosition =
        Vector3.zero;

    [SerializeField]
    private Vector3 motorLocalRotation =
        Vector3.zero;

    [SerializeField]
    private Vector3 motorScale =
        Vector3.one;


    // =====================================================
    // Rotation Sensitivity
    // =====================================================

    public const float MinRotationSensitivity = 0.2f;
    public const float MaxRotationSensitivity = 20f;

    public const string ObjectRotationSensitivityKey =
        "ObjectRotationSensitivity";

    public const string KeyboardRotationSensitivityKey =
        "KeyboardRotationSensitivity";


    // =====================================================
    // Mouse Rotation
    // =====================================================

    [Header("Mouse Rotation")]

    [Tooltip("기본 좌클릭 드래그 회전 속도")]
    [SerializeField]
    private float baseMouseRotationSpeed = 0.2f;

    [Tooltip("환경설정 값이 없을 때 사용할 기본 마우스 회전 감도")]
    [Range(MinRotationSensitivity, MaxRotationSensitivity)]
    [SerializeField]
    private float defaultObjectRotationSensitivity = 1f;


    // =====================================================
    // Keyboard Rotation
    // =====================================================

    [Header("Keyboard Rotation")]

    [Tooltip("기본 방향키 회전 속도")]
    [SerializeField]
    private float baseKeyboardRotationSpeed = 30f;

    [Tooltip("환경설정 값이 없을 때 사용할 기본 방향키 회전 감도")]
    [Range(MinRotationSensitivity, MaxRotationSensitivity)]
    [SerializeField]
    private float defaultKeyboardRotationSensitivity = 1f;


    // =====================================================
    // Zoom
    // =====================================================

    [Header("Zoom")]

    [Tooltip("마우스 휠 확대/축소 속도")]
    [SerializeField]
    private float zoomSpeed = 0.15f;

    [Tooltip("최소 확대 배율")]
    [SerializeField]
    private float minZoomScale = 0.7f;

    [Tooltip("최대 확대 배율")]
    [SerializeField]
    private float maxZoomScale = 2.0f;

    private float currentZoomScale = 1f;


    // =====================================================
    // Current Motor State
    // =====================================================

    /// <summary>
    /// 현재 생성된 모터가 A등급인지
    /// true = A
    /// false = B / C / 폐기
    /// </summary>
    public bool CurrentMotorValid
    {
        get;
        private set;
    }


    /// <summary>
    /// 현재 생성된 모터의 최종 결과
    /// </summary>
    public InspectionResult CurrentResult
    {
        get;
        private set;
    }


    /// <summary>
    /// 현재 판정 기준에 따라 계산된 불량 항목 개수
    ///
    /// 판정 가능한 불량 항목:
    /// 1. Motor Material != 0
    /// 2. 앞뒤 프로펠러 번호 다름
    /// 3. 앞뒤 프로펠러 색상 다름
    ///
    /// 각 항목은 InspectionGameManager에서
    /// 오늘의 판정 조건에 따라 포함 / 제외 가능.
    ///
    /// 프로펠러 누락에 의한 폐기는 별도 규칙으로 판정.
    /// </summary>
    public int CurrentDefectCount
    {
        get;
        private set;
    }


    // =====================================================
    // 현재 생성된 상태 정보
    //
    // InspectionGameManager의
    // 오답 안내 문구 출력용
    // =====================================================

    // 앞 프로펠러 존재
    public bool CurrentHasFront =>
        currentRandomMotor != null &&
        currentRandomMotor.HasFront;


    // 뒤 프로펠러 존재
    public bool CurrentHasBack =>
        currentRandomMotor != null &&
        currentRandomMotor.HasBack;


    // 모터 얼룩 여부
    //
    // Material 0 = 정상
    // Material 1~4 = 얼룩 있음
    public bool CurrentMotorHasStain =>
        currentRandomMotor != null &&
        currentRandomMotor.SelectedMotorMaterial != 0;


    // 모터 Material 정상 여부
    public bool CurrentMotorMaterialCorrect =>
        currentRandomMotor != null &&
        currentRandomMotor.SelectedMotorMaterial == 0;


    // 앞뒤 프로펠러 번호가 같은지 여부
    public bool CurrentPropellerNumberSame =>
        currentRandomMotor != null &&
        currentRandomMotor.SelectedFrontNumber ==
        currentRandomMotor.SelectedBackNumber;


    // 앞뒤 프로펠러 색상이 같은지 여부
    public bool CurrentPropellerColorSame =>
        currentRandomMotor != null &&
        currentRandomMotor.SelectedFrontColor ==
        currentRandomMotor.SelectedBackColor;


    // =====================================================
    // Runtime
    // =====================================================

    private GameObject motorPrefab;

    private GameObject currentHolder;
    private GameObject currentMotor;

    private RandomMotor currentRandomMotor;

    private bool isReady;

    public bool IsReady =>
        isReady;


    // =====================================================
    // Start
    // =====================================================

    private void Start()
    {
        HideAnchor();

        ValidateSavedSensitivity();

        LoadPrefab();

        isReady =
            ValidateData();
    }


    // =====================================================
    // Update
    // =====================================================

    private void Update()
    {
        if (!isReady)
            return;


        // 팝업 열려 있으면
        // 모터 회전 / 휠 입력 차단
        if (
            PopupManager.Instance != null &&
            PopupManager.Instance.HasOpenPopup()
        )
        {
            return;
        }


        HandleMouseRotation();

        HandleMouseZoom();

        HandleKeyboardRotation();
    }


    // =====================================================
    // 저장된 감도 값 보정
    // =====================================================

    private void ValidateSavedSensitivity()
    {
        // =================================================
        // 마우스 감도
        // =================================================

        float mouseSensitivity =
            PlayerPrefs.GetFloat(
                ObjectRotationSensitivityKey,
                defaultObjectRotationSensitivity
            );


        mouseSensitivity =
            Mathf.Clamp(
                mouseSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        PlayerPrefs.SetFloat(
            ObjectRotationSensitivityKey,
            mouseSensitivity
        );


        // =================================================
        // 방향키 감도
        // =================================================

        float keyboardSensitivity =
            PlayerPrefs.GetFloat(
                KeyboardRotationSensitivityKey,
                defaultKeyboardRotationSensitivity
            );


        keyboardSensitivity =
            Mathf.Clamp(
                keyboardSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        PlayerPrefs.SetFloat(
            KeyboardRotationSensitivityKey,
            keyboardSensitivity
        );


        PlayerPrefs.Save();
    }


    // =====================================================
    // Anchor 숨기기
    // =====================================================

    private void HideAnchor()
    {
        if (displayAnchor == null)
            return;


        MeshRenderer[] renderers =
            displayAnchor.GetComponentsInChildren<MeshRenderer>();


        foreach (MeshRenderer r in renderers)
        {
            r.enabled =
                false;
        }
    }


    // =====================================================
    // Prefab Load
    // =====================================================

    private void LoadPrefab()
    {
        motorPrefab =
            Resources.Load<GameObject>(
                motorResourcePath
            );
    }


    // =====================================================
    // Validate
    // =====================================================

    private bool ValidateData()
    {
        bool valid =
            true;


        // =================================================
        // Motor Prefab
        // =================================================

        if (motorPrefab == null)
        {
            Debug.LogError(
                "Motor Prefab을 찾을 수 없습니다. " +
                "Resources/" +
                motorResourcePath +
                " 경로를 확인해주세요."
            );


            valid =
                false;
        }


        // =================================================
        // Display Anchor
        // =================================================

        if (displayAnchor == null)
        {
            Debug.LogError(
                "DisplayAnchor가 연결되지 않았습니다."
            );


            valid =
                false;
        }


        // =================================================
        // RandomMotor
        // =================================================

        if (motorPrefab != null)
        {
            RandomMotor randomMotor =
                motorPrefab.GetComponentInChildren<RandomMotor>(
                    true
                );


            if (randomMotor == null)
            {
                Debug.LogError(
                    "Motor Prefab에 RandomMotor 스크립트가 없습니다."
                );


                valid =
                    false;
            }
        }


        return valid;
    }


    // =====================================================
    // Motor 랜덤 생성
    // =====================================================
    //
    // boxIsNormal == true
    //
    // 정상 박스
    // → GenerateMotor(true)
    // → 무조건 A 모터 생성
    //
    //
    // boxIsNormal == false
    //
    // 개봉된 박스
    // → GenerateMotor(false)
    // → A/B/C/폐기 모두 가능
    //
    // 기존 함수이므로
    // 앞 / 뒤 프로펠러 모두 필수,
    // 모든 판정 항목 ON으로 계산
    // =====================================================

    public InspectionResult CreateNextMatch(
        bool boxIsNormal
    )
    {
        // =================================================
        // 기존 모터 제거
        // =================================================

        RemoveCurrentMotor();


        // =================================================
        // Holder 생성
        // =================================================

        currentHolder =
            new GameObject(
                "CurrentMotorHolder"
            );


        currentHolder.transform.SetParent(
            displayAnchor,
            false
        );


        currentHolder.transform.localPosition =
            Vector3.zero;


        currentHolder.transform.localRotation =
            Quaternion.identity;


        currentHolder.transform.localScale =
            Vector3.one;


        currentZoomScale =
            1f;


        // =================================================
        // Motor Prefab 생성
        // =================================================

        currentMotor =
            Instantiate(
                motorPrefab,
                currentHolder.transform
            );


        currentMotor.transform.localPosition =
            motorLocalPosition;


        currentMotor.transform.localRotation =
            Quaternion.Euler(
                motorLocalRotation
            );


        currentMotor.transform.localScale =
            motorScale;


        // =================================================
        // RandomMotor 찾기
        // =================================================

        currentRandomMotor =
            currentMotor.GetComponentInChildren<RandomMotor>(
                true
            );


        if (currentRandomMotor == null)
        {
            Debug.LogError(
                "생성된 Motor Prefab에서 " +
                "RandomMotor 스크립트를 찾을 수 없습니다."
            );


            CurrentMotorValid =
                false;


            CurrentDefectCount =
                0;


            CurrentResult =
                InspectionResult.Discard;


            return CurrentResult;
        }


        // =================================================
        // 박스 상태에 따라 모터 생성
        // =================================================

        if (boxIsNormal)
        {
            // =============================================
            // 정상 박스
            //
            // 무조건 A 모터 생성
            // =============================================

            currentRandomMotor.GenerateMotor(
                true
            );
        }
        else
        {
            // =============================================
            // 개봉된 박스
            //
            // 모든 결과 가능
            // =============================================

            currentRandomMotor.GenerateMotor(
                false
            );
        }


        // =================================================
        // 최종 결과 계산
        //
        // 기존 함수에서는 기존 규칙 유지
        //
        // 앞 프로펠러 필수
        // 뒤 프로펠러 필수
        // 얼룩 판정
        // 번호 판정
        // 색상 판정
        // =================================================

        CurrentResult =
            DetermineMotorResult(
                true,
                true,
                true,
                true,
                true
            );


        // =================================================
        // A인 경우에만 정상 true
        // =================================================

        CurrentMotorValid =
            CurrentResult ==
            InspectionResult.A;


        // =================================================
        // Debug
        // =================================================

        Debug.Log(
            "=============== MOTOR MATCH ===============" +

            "\n박스 정상 여부 : " +
            boxIsNormal +

            "\n" +

            "\n앞 프로펠러 존재 : " +
            currentRandomMotor.HasFront +

            "\n뒤 프로펠러 존재 : " +
            currentRandomMotor.HasBack +

            "\n" +

            "\nMotor Material : " +
            currentRandomMotor.SelectedMotorMaterial +

            "\n앞 프로펠러 번호 : " +
            currentRandomMotor.SelectedFrontNumber +

            "\n뒤 프로펠러 번호 : " +
            currentRandomMotor.SelectedBackNumber +

            "\n앞 프로펠러 색상 : " +
            currentRandomMotor.SelectedFrontColor +

            "\n뒤 프로펠러 색상 : " +
            currentRandomMotor.SelectedBackColor +

            "\n" +

            "\n불량 항목 개수 : " +
            CurrentDefectCount +

            "\n모터 정상 여부 : " +
            CurrentMotorValid +

            "\n최종 결과 : " +
            CurrentResult +

            "\n==========================================="
        );


        return CurrentResult;
    }


    // =====================================================
    // 조건 기반 Motor 랜덤 생성
    // =====================================================

    public InspectionResult CreateNextMatchByCondition(
        bool spawnFront,
        float frontChance,

        bool spawnBack,
        float backChance,

        bool useMotorTextureCondition,
        float motor0Chance,

        bool useSameNumberCondition,
        float sameNumberChance,

        bool useSameColorCondition,
        float sameColorChance,

        bool allowTripleDefectMotor,

        bool requireFrontPropeller,
        bool requireBackPropeller,
        bool gradeMotorStain,
        bool gradePropellerNumber,
        bool gradePropellerColor
    )
    {
        // =================================================
        // 기존 모터 제거
        // =================================================

        RemoveCurrentMotor();


        // =================================================
        // Holder 생성
        // =================================================

        currentHolder =
            new GameObject(
                "CurrentMotorHolder"
            );


        currentHolder.transform.SetParent(
            displayAnchor,
            false
        );


        currentHolder.transform.localPosition =
            Vector3.zero;


        currentHolder.transform.localRotation =
            Quaternion.identity;


        currentHolder.transform.localScale =
            Vector3.one;


        currentZoomScale =
            1f;


        // =================================================
        // Motor Prefab 생성
        // =================================================

        currentMotor =
            Instantiate(
                motorPrefab,
                currentHolder.transform
            );


        currentMotor.transform.localPosition =
            motorLocalPosition;


        currentMotor.transform.localRotation =
            Quaternion.Euler(
                motorLocalRotation
            );


        currentMotor.transform.localScale =
            motorScale;


        // =================================================
        // RandomMotor 찾기
        // =================================================

        currentRandomMotor =
            currentMotor.GetComponentInChildren<RandomMotor>(
                true
            );


        if (currentRandomMotor == null)
        {
            Debug.LogError(
                "생성된 Motor Prefab에서 RandomMotor를 찾을 수 없습니다."
            );


            CurrentMotorValid =
                false;


            CurrentDefectCount =
                0;


            CurrentResult =
                InspectionResult.Discard;


            return CurrentResult;
        }


        // =================================================
        // 조건 기반으로 모터 생성
        //
        // allowTripleDefectMotor == true
        // → 실제 모터 상태 기준 모든 결과 허용
        //
        // allowTripleDefectMotor == false
        // → 아래 3개 실제 불량이 동시에 발생하면 다시 생성
        //
        // 1. Motor Material != 0
        // 2. 앞뒤 Number 다름
        // 3. 앞뒤 Color 다름
        //
        // 이 설정은 생성에 관한 설정이므로
        // 오늘의 grade 설정과는 별개
        // =================================================

        const int maxGenerateAttempts =
            100;

        int generateAttempt =
            0;


        while (true)
        {
            // =============================================
            // 모터 생성
            // =============================================

            currentRandomMotor.GenerateMotorByCondition(
                spawnFront,
                frontChance,

                spawnBack,
                backChance,

                useMotorTextureCondition,
                motor0Chance,

                useSameNumberCondition,
                sameNumberChance,

                useSameColorCondition,
                sameColorChance
            );


            generateAttempt++;


            // =============================================
            // 실제 3중 불량 모터 발생 여부
            //
            // 앞뒤가 모두 있을 때만
            // 번호 / 색상 비교 가능
            // =============================================

            bool isTripleDefect =
                currentRandomMotor.HasFront &&
                currentRandomMotor.HasBack &&

                currentRandomMotor.SelectedMotorMaterial != 0 &&

                currentRandomMotor.SelectedFrontNumber !=
                currentRandomMotor.SelectedBackNumber &&

                currentRandomMotor.SelectedFrontColor !=
                currentRandomMotor.SelectedBackColor;


            // =============================================
            // 체크 ON
            // → 3중 불량도 허용
            // =============================================

            if (allowTripleDefectMotor)
            {
                break;
            }


            // =============================================
            // 체크 OFF인데
            // 3중 불량이 아니면 허용
            // =============================================

            if (!isTripleDefect)
            {
                break;
            }


            // =============================================
            // 생성 횟수 제한
            //
            // 확률 설정상 정상 조합을 만들 수 없는 경우
            // 100회 이후 A 모터 생성
            // =============================================

            if (
                generateAttempt >=
                maxGenerateAttempts
            )
            {
                Debug.LogWarning(
                    "3중 불량 모터 발생을 막을 수 없는 확률 설정입니다. " +
                    "A급 모터로 대체합니다."
                );


                currentRandomMotor.GenerateMotor(
                    true
                );


                break;
            }
        }


        // =================================================
        // 최종 결과 계산
        //
        // 오늘의 판정 기준 반영
        // =================================================

        CurrentResult =
            DetermineMotorResult(
                requireFrontPropeller,
                requireBackPropeller,
                gradeMotorStain,
                gradePropellerNumber,
                gradePropellerColor
            );


        CurrentMotorValid =
            CurrentResult ==
            InspectionResult.A;


        // =================================================
        // Debug
        // =================================================

        Debug.Log(
            "======= MOTOR CONDITION GENERATION =======" +

            "\n앞 프로펠러 : " +
            currentRandomMotor.HasFront +

            "\n뒤 프로펠러 : " +
            currentRandomMotor.HasBack +

            "\n모터 Material : " +
            currentRandomMotor.SelectedMotorMaterial +

            "\n앞 번호 : " +
            currentRandomMotor.SelectedFrontNumber +

            "\n뒤 번호 : " +
            currentRandomMotor.SelectedBackNumber +

            "\n앞 색상 : " +
            currentRandomMotor.SelectedFrontColor +

            "\n뒤 색상 : " +
            currentRandomMotor.SelectedBackColor +

            "\n" +

            "\nRequire Front Propeller : " +
            requireFrontPropeller +

            "\nRequire Back Propeller : " +
            requireBackPropeller +

            "\nGrade Motor Stain : " +
            gradeMotorStain +

            "\nGrade Propeller Number : " +
            gradePropellerNumber +

            "\nGrade Propeller Color : " +
            gradePropellerColor +

            "\n" +

            "\n판정 불량 개수 : " +
            CurrentDefectCount +

            "\n최종 결과 : " +
            CurrentResult +

            "\n=========================================="
        );


        return CurrentResult;
    }


    // =====================================================
    // Motor 등급 계산
    // =====================================================
    //
    // [프로펠러 폐기 규칙]
    //
    // requireFrontPropeller == true
    // → 앞 프로펠러가 없으면 폐기
    //
    // requireBackPropeller == true
    // → 뒤 프로펠러가 없으면 폐기
    //
    //
    // 예시
    //
    // Front ON / Back ON
    // → 앞뒤 모두 필수
    //
    // Front ON / Back OFF
    // → 앞만 필수
    //
    // Front OFF / Back ON
    // → 뒤만 필수
    //
    // Front OFF / Back OFF
    // → 프로펠러 누락으로는 폐기하지 않음
    //
    //
    // 번호 / 색상은
    // 앞뒤 프로펠러가 둘 다 존재할 때만 비교.
    //
    //
    // [A]
    // 판정 대상 불량 0개
    //
    // [B]
    // 판정 대상 불량 1개
    //
    // [C]
    // 판정 대상 불량 2개 이상
    // =====================================================

    private InspectionResult DetermineMotorResult(
        bool requireFrontPropeller,
        bool requireBackPropeller,
        bool gradeMotorStain,
        bool gradePropellerNumber,
        bool gradePropellerColor
    )
    {
        CurrentDefectCount =
            0;


        // =================================================
        // 현재 프로펠러 상태
        // =================================================

        bool hasFront =
            currentRandomMotor.HasFront;


        bool hasBack =
            currentRandomMotor.HasBack;


        // =================================================
        // 앞 프로펠러 폐기 판정
        // =================================================

        if (
            requireFrontPropeller &&
            !hasFront
        )
        {
            return InspectionResult.Discard;
        }


        // =================================================
        // 뒤 프로펠러 폐기 판정
        // =================================================

        if (
            requireBackPropeller &&
            !hasBack
        )
        {
            return InspectionResult.Discard;
        }


        // =================================================
        // 불량 항목 1
        //
        // 모터 얼룩
        //
        // 오늘 얼룩 판정이 ON일 때만 +1
        // =================================================

        if (
            gradeMotorStain &&
            currentRandomMotor.SelectedMotorMaterial != 0
        )
        {
            CurrentDefectCount++;
        }


        // =================================================
        // 앞뒤 비교 가능 여부
        //
        // 둘 다 존재할 때만
        // Number / Color 비교 가능
        // =================================================

        bool canComparePropellers =
            hasFront &&
            hasBack;


        // =================================================
        // 불량 항목 2
        //
        // 프로펠러 앞뒤 번호 다름
        // =================================================

        if (
            canComparePropellers &&
            gradePropellerNumber &&
            currentRandomMotor.SelectedFrontNumber !=
            currentRandomMotor.SelectedBackNumber
        )
        {
            CurrentDefectCount++;
        }


        // =================================================
        // 불량 항목 3
        //
        // 프로펠러 앞뒤 색상 다름
        // =================================================

        if (
            canComparePropellers &&
            gradePropellerColor &&
            currentRandomMotor.SelectedFrontColor !=
            currentRandomMotor.SelectedBackColor
        )
        {
            CurrentDefectCount++;
        }


        // =================================================
        // A
        // =================================================

        if (CurrentDefectCount == 0)
        {
            return InspectionResult.A;
        }


        // =================================================
        // B
        // =================================================

        if (CurrentDefectCount == 1)
        {
            return InspectionResult.B;
        }


        // =================================================
        // C
        // =================================================

        return InspectionResult.C;
    }


    // =====================================================
    // 현재 Motor 제거
    // =====================================================

    public void RemoveCurrentMotor()
    {
        if (currentHolder != null)
        {
            Destroy(
                currentHolder
            );


            currentHolder =
                null;


            currentMotor =
                null;


            currentRandomMotor =
                null;
        }


        CurrentMotorValid =
            false;


        CurrentDefectCount =
            0;


        currentZoomScale =
            1f;
    }


    // =====================================================
    // 마우스 회전
    // =====================================================

    private void HandleMouseRotation()
    {
        if (currentHolder == null)
            return;


        if (Mouse.current == null)
            return;


        if (!Mouse.current.leftButton.isPressed)
            return;


        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();


        float objectRotationSensitivity =
            PlayerPrefs.GetFloat(
                ObjectRotationSensitivityKey,
                defaultObjectRotationSensitivity
            );


        objectRotationSensitivity =
            Mathf.Clamp(
                objectRotationSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        float finalRotationSpeed =
            baseMouseRotationSpeed *
            objectRotationSensitivity;


        float horizontalRotation =
            -mouseDelta.x *
            finalRotationSpeed;


        float verticalRotation =
            mouseDelta.y *
            finalRotationSpeed;


        currentHolder.transform.Rotate(
            Vector3.up,
            horizontalRotation,
            Space.World
        );


        currentHolder.transform.Rotate(
            Vector3.left,
            verticalRotation,
            Space.World
        );
    }


    // =====================================================
    // 마우스 휠 확대 / 축소
    // =====================================================

    private void HandleMouseZoom()
    {
        if (currentHolder == null)
            return;


        if (Mouse.current == null)
            return;


        float scrollValue =
            Mouse.current.scroll.ReadValue().y;


        if (
            ScrollInvertSetting
                .IsScrollInverted()
        )
        {
            scrollValue *=
                -1f;
        }


        if (
            Mathf.Abs(
                scrollValue
            ) < 0.01f
        )
        {
            return;
        }


        currentZoomScale +=
            Mathf.Sign(
                scrollValue
            ) *
            zoomSpeed;


        currentZoomScale =
            Mathf.Clamp(
                currentZoomScale,
                minZoomScale,
                maxZoomScale
            );


        currentHolder.transform.localScale =
            Vector3.one *
            currentZoomScale;
    }


    // =====================================================
    // 방향키 회전
    // =====================================================

    private void HandleKeyboardRotation()
    {
        if (currentHolder == null)
            return;


        if (Keyboard.current == null)
            return;


        float keyboardSensitivity =
            PlayerPrefs.GetFloat(
                KeyboardRotationSensitivityKey,
                defaultKeyboardRotationSensitivity
            );


        keyboardSensitivity =
            Mathf.Clamp(
                keyboardSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        float finalRotationSpeed =
            baseKeyboardRotationSpeed *
            keyboardSensitivity;


        float verticalRotation =
            0f;


        float horizontalRotation =
            0f;


        // 위
        if (
            Keyboard.current
                .upArrowKey
                .isPressed
        )
        {
            verticalRotation +=
                finalRotationSpeed *
                Time.deltaTime;
        }


        // 아래
        if (
            Keyboard.current
                .downArrowKey
                .isPressed
        )
        {
            verticalRotation -=
                finalRotationSpeed *
                Time.deltaTime;
        }


        // 왼쪽
        if (
            Keyboard.current
                .leftArrowKey
                .isPressed
        )
        {
            horizontalRotation +=
                finalRotationSpeed *
                Time.deltaTime;
        }


        // 오른쪽
        if (
            Keyboard.current
                .rightArrowKey
                .isPressed
        )
        {
            horizontalRotation -=
                finalRotationSpeed *
                Time.deltaTime;
        }


        if (verticalRotation != 0f)
        {
            currentHolder.transform.Rotate(
                Vector3.right,
                verticalRotation,
                Space.World
            );
        }


        if (horizontalRotation != 0f)
        {
            currentHolder.transform.Rotate(
                Vector3.up,
                horizontalRotation,
                Space.World
            );
        }
    }


    // =====================================================
    // Inspector 값 보정
    // =====================================================

    private void OnValidate()
    {
        baseMouseRotationSpeed =
            Mathf.Max(
                0f,
                baseMouseRotationSpeed
            );


        defaultObjectRotationSensitivity =
            Mathf.Clamp(
                defaultObjectRotationSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        baseKeyboardRotationSpeed =
            Mathf.Max(
                0f,
                baseKeyboardRotationSpeed
            );


        defaultKeyboardRotationSensitivity =
            Mathf.Clamp(
                defaultKeyboardRotationSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        zoomSpeed =
            Mathf.Max(
                0f,
                zoomSpeed
            );


        minZoomScale =
            Mathf.Max(
                0.1f,
                minZoomScale
            );


        maxZoomScale =
            Mathf.Max(
                minZoomScale,
                maxZoomScale
            );
    }
}