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

    [Tooltip("최소 확대 비율")]
    [SerializeField]
    private float minZoomScale = 0.7f;

    [Tooltip("최대 확대 비율")]
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
    /// 현재 생성된 모터의 최종 등급
    /// </summary>
    public InspectionResult CurrentResult
    {
        get;
        private set;
    }


    /// <summary>
    /// 현재 모터의 불량 조건 개수
    ///
    /// 불량 조건:
    /// 1. Motor Material != 0
    /// 2. 앞뒤 프로펠러 번호 다름
    /// 3. 앞뒤 프로펠러 색상 다름
    ///
    /// 프로펠러가 없으면 폐기이므로
    /// A/B/C 판정용 불량 개수는 계산하지 않음.
    /// </summary>
    public int CurrentDefectCount
    {
        get;
        private set;
    }


    // =====================================================
    // 현재 모터 상세 상태
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


    // 모터 얼룩 존재
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


    // 앞뒤 프로펠러 날개 개수 동일 여부
    public bool CurrentPropellerNumberSame =>
        currentRandomMotor != null &&
        currentRandomMotor.SelectedFrontNumber ==
        currentRandomMotor.SelectedBackNumber;


    // 앞뒤 프로펠러 색상 동일 여부
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
        // 모터 회전 / 줌 조작 차단
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
    // 저장된 감도 보정
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
                "DisplayAnchor가 지정되지 않았습니다."
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
    // Motor 문제 생성
    // =====================================================
    //
    // boxIsNormal == true
    //
    // 정상 박스
    // → GenerateMotor(true)
    // → 무조건 A 모터
    //
    //
    // boxIsNormal == false
    //
    // 비정상 박스
    // → GenerateMotor(false)
    // → A/B/C/폐기 모두 가능
    //
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
            // 무조건 A 조건 모터
            // =============================================

            currentRandomMotor.GenerateMotor(
                true
            );
        }
        else
        {
            // =============================================
            // 비정상 박스
            //
            // 모든 경우 랜덤
            // =============================================

            currentRandomMotor.GenerateMotor(
                false
            );
        }


        // =================================================
        // 최종 등급 판정
        // =================================================

        CurrentResult =
            DetermineMotorResult();


        // =================================================
        // A일 경우에만 정상 true
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

            "\n앞 프로펠러 컬러 : " +
            currentRandomMotor.SelectedFrontColor +

            "\n뒤 프로펠러 컬러 : " +
            currentRandomMotor.SelectedBackColor +

            "\n" +

            "\n불량 조건 개수 : " +
            CurrentDefectCount +

            "\n최종 정상 여부 : " +
            CurrentMotorValid +

            "\n최종 등급 : " +
            CurrentResult +

            "\n==========================================="
        );


        return CurrentResult;
    }


    // =====================================================
    // Motor 등급 판정
    // =====================================================
    //
    // [폐기]
    //
    // 앞 / 뒤 프로펠러 중
    // 하나라도 없음
    //
    //
    // [A]
    //
    // 프로펠러 모두 존재
    // +
    // Motor Material = 0
    // +
    // 날개 개수 동일
    // +
    // 색상 동일
    //
    //
    // [B]
    //
    // 아래 중 정확히 1개 문제
    //
    // - Motor Material != 0
    // - 날개 개수 다름
    // - 색상 다름
    //
    //
    // [C]
    //
    // 위 문제 중 2개 이상
    //
    // =====================================================

    private InspectionResult DetermineMotorResult()
    {
        CurrentDefectCount =
            0;


        // =================================================
        // 폐기
        //
        // 프로펠러 하나라도 없으면
        // 나머지 조건과 관계없이 폐기
        // =================================================

        if (
            !currentRandomMotor.HasFront ||
            !currentRandomMotor.HasBack
        )
        {
            return InspectionResult.Discard;
        }


        // =================================================
        // 불량 조건 1
        //
        // 모터 얼룩
        // Motor Material != 0
        // =================================================

        if (
            currentRandomMotor.SelectedMotorMaterial !=
            0
        )
        {
            CurrentDefectCount++;
        }


        // =================================================
        // 불량 조건 2
        //
        // 프로펠러 날개 개수 다름
        // =================================================

        if (
            currentRandomMotor.SelectedFrontNumber !=
            currentRandomMotor.SelectedBackNumber
        )
        {
            CurrentDefectCount++;
        }


        // =================================================
        // 불량 조건 3
        //
        // 프로펠러 색상 다름
        // =================================================

        if (
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