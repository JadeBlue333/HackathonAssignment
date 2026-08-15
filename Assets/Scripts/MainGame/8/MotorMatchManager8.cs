using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMatchManager8 : MonoBehaviour
{
    // =====================================================
    // Resources
    // =====================================================

    [Header("Resources")]

    [SerializeField]
    private string motorResourcePath = "Motors+Prop/motorProp";


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
    private Vector3 motorLocalPosition = Vector3.zero;

    [SerializeField]
    private Vector3 motorLocalRotation = Vector3.zero;

    [SerializeField]
    private Vector3 motorScale = Vector3.one;


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
    // Propeller
    // =====================================================

    [Header("Propeller")]

    [SerializeField]
    private string frontPropellerName = "P_Front";

    [SerializeField]
    private string backPropellerName = "P_Back";

    [SerializeField]
    private List<Material> propellerMaterials = new();


    // =====================================================
    // Material Index
    // =====================================================

    [Header("Material Index")]

    [SerializeField]
    private int frontMaterialIndex = 0;

    [SerializeField]
    private int backMaterialIndex = 0;


    // =====================================================
    // Color
    // =====================================================

    [Header("Color")]

    [Range(0f, 100f)]
    [SerializeField]
    private float sameColorChance = 50f;


    // =====================================================
    // Missing Part
    // =====================================================

    [Header("Missing Part")]

    [Range(0f, 100f)]
    [SerializeField]
    private float completeChance = 50f;


    // =====================================================
    // Current Inspection State
    // =====================================================

    public bool CurrentSameColor
    {
        get;
        private set;
    }


    public bool CurrentIsComplete
    {
        get;
        private set;
    }


    // =====================================================
    // Runtime
    // =====================================================

    private GameObject motorPrefab;

    private GameObject currentHolder;

    private GameObject currentMotor;

    private bool isReady;

    public bool IsReady => isReady;


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


        // 팝업이 하나라도 열려 있으면
        // 모터 조작만 차단
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
    // Anchor
    // =====================================================

    private void HideAnchor()
    {
        if (displayAnchor == null)
            return;


        MeshRenderer[] renderers =
            displayAnchor.GetComponentsInChildren<MeshRenderer>();


        foreach (MeshRenderer r in renderers)
        {
            r.enabled = false;
        }
    }


    // =====================================================
    // Load
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
        bool valid = true;


        if (motorPrefab == null)
        {
            Debug.LogError(
                "Motor Prefab을 찾을 수 없습니다."
            );

            valid = false;
        }


        if (displayAnchor == null)
        {
            Debug.LogError(
                "DisplayAnchor가 지정되지 않았습니다."
            );

            valid = false;
        }


        if (propellerMaterials.Count < 2)
        {
            Debug.LogError(
                "Material을 2개 이상 넣어주세요."
            );

            valid = false;
        }


        return valid;
    }


    // =====================================================
    // 문제 생성
    // =====================================================

    public InspectionResult CreateNextMatch()
    {
        RemoveCurrentMotor();


        // -------------------------------------------------
        // Holder 생성
        // -------------------------------------------------

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


        // -------------------------------------------------
        // Zoom 초기화
        // -------------------------------------------------

        currentZoomScale =
            1f;


        currentHolder.transform.localScale =
            Vector3.one;


        // -------------------------------------------------
        // Motor 생성
        // -------------------------------------------------

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


        // -------------------------------------------------
        // 프로펠러 찾기
        // -------------------------------------------------

        Transform front =
            FindChildRecursive(
                currentMotor.transform,
                frontPropellerName
            );


        Transform back =
            FindChildRecursive(
                currentMotor.transform,
                backPropellerName
            );


        if (front == null || back == null)
        {
            Debug.LogError(
                "프로펠러를 찾을 수 없습니다."
            );


            return InspectionResult.B;
        }


        MeshRenderer frontRenderer =
            front.GetComponent<MeshRenderer>();


        MeshRenderer backRenderer =
            back.GetComponent<MeshRenderer>();


        if (
            frontRenderer == null ||
            backRenderer == null
        )
        {
            Debug.LogError(
                "MeshRenderer를 찾을 수 없습니다."
            );


            return InspectionResult.B;
        }


        // -------------------------------------------------
        // 프로펠러 색상 결정
        // -------------------------------------------------

        int frontIndex =
            Random.Range(
                0,
                propellerMaterials.Count
            );


        int backIndex;


        bool sameColor =
            Random.Range(
                0f,
                100f
            ) < sameColorChance;


        bool isComplete =
            Random.Range(
                0f,
                100f
            ) < completeChance;


        // =====================================================
        // 현재 문제 상태 저장
        // =====================================================

        CurrentSameColor =
            sameColor;


        CurrentIsComplete =
            isComplete;


        // -------------------------------------------------
        // 색상 동일 / 다름
        // -------------------------------------------------

        if (sameColor)
        {
            backIndex =
                frontIndex;
        }
        else
        {
            do
            {
                backIndex =
                    Random.Range(
                        0,
                        propellerMaterials.Count
                    );
            }
            while (
                backIndex ==
                frontIndex
            );
        }


        // -------------------------------------------------
        // 부품 존재 / 누락
        // -------------------------------------------------

        if (isComplete)
        {
            front.gameObject.SetActive(
                true
            );


            back.gameObject.SetActive(
                true
            );
        }
        else
        {
            int missingType =
                Random.Range(
                    0,
                    3
                );


            switch (missingType)
            {
                // 앞 프로펠러 누락
                case 0:

                    front.gameObject.SetActive(
                        false
                    );


                    back.gameObject.SetActive(
                        true
                    );

                    break;


                // 뒤 프로펠러 누락
                case 1:

                    front.gameObject.SetActive(
                        true
                    );


                    back.gameObject.SetActive(
                        false
                    );

                    break;


                // 둘 다 누락
                case 2:

                    front.gameObject.SetActive(
                        false
                    );


                    back.gameObject.SetActive(
                        false
                    );

                    break;
            }
        }


        // -------------------------------------------------
        // Material 적용
        // -------------------------------------------------

        Material[] frontMats =
            frontRenderer.materials;


        Material[] backMats =
            backRenderer.materials;


        if (
            frontMaterialIndex < 0 ||
            frontMaterialIndex >= frontMats.Length
        )
        {
            Debug.LogError(
                "Front Material Index 범위를 확인해주세요."
            );


            return InspectionResult.B;
        }


        if (
            backMaterialIndex < 0 ||
            backMaterialIndex >= backMats.Length
        )
        {
            Debug.LogError(
                "Back Material Index 범위를 확인해주세요."
            );


            return InspectionResult.B;
        }


        frontMats[
            frontMaterialIndex
        ] =
            propellerMaterials[
                frontIndex
            ];


        backMats[
            backMaterialIndex
        ] =
            propellerMaterials[
                backIndex
            ];


        frontRenderer.materials =
            frontMats;


        backRenderer.materials =
            backMats;


        // -------------------------------------------------
        // 결과 결정
        // -------------------------------------------------

        InspectionResult result;


        if (!isComplete)
        {
            result =
                InspectionResult.Discard;
        }
        else if (sameColor)
        {
            result =
                InspectionResult.A;
        }
        else
        {
            result =
                InspectionResult.B;
        }


        Debug.Log(
            $"Color : " +
            $"{(sameColor ? "Same" : "Different")} | " +
            $"Part : " +
            $"{(isComplete ? "Complete" : "Missing")} | " +
            $"Result : {result}"
        );


        return result;
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


            currentHolder = null;

            currentMotor = null;
        }


        currentZoomScale =
            1f;
    }


    // =====================================================
    // 이름으로 자식 찾기
    // =====================================================

    private Transform FindChildRecursive(
        Transform parent,
        string targetName
    )
    {
        foreach (Transform child in parent)
        {
            if (
                child.name ==
                targetName
            )
            {
                return child;
            }


            Transform result =
                FindChildRecursive(
                    child,
                    targetName
                );


            if (result != null)
            {
                return result;
            }
        }


        return null;
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


        if (
            !Mouse.current.leftButton
                .isPressed
        )
        {
            return;
        }


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


        if (
            Keyboard.current.upArrowKey
                .isPressed
        )
        {
            verticalRotation +=
                finalRotationSpeed *
                Time.deltaTime;
        }


        if (
            Keyboard.current.downArrowKey
                .isPressed
        )
        {
            verticalRotation -=
                finalRotationSpeed *
                Time.deltaTime;
        }


        if (
            Keyboard.current.leftArrowKey
                .isPressed
        )
        {
            horizontalRotation +=
                finalRotationSpeed *
                Time.deltaTime;
        }


        if (
            Keyboard.current.rightArrowKey
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
        sameColorChance =
            Mathf.Clamp(
                sameColorChance,
                0f,
                100f
            );


        completeChance =
            Mathf.Clamp(
                completeChance,
                0f,
                100f
            );


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