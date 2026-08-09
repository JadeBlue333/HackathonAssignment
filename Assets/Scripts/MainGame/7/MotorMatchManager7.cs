using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMatchManager7 : MonoBehaviour
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
    // Rotation
    // =====================================================

    [Header("Rotation")]

    [Tooltip("기본 좌클릭 드래그 회전 속도")]
    [SerializeField]
    private float baseMouseRotationSpeed = 0.2f;

    [Tooltip("환경설정 값이 없을 때 사용할 기본 물체 회전 감도")]
    [SerializeField]
    private float defaultObjectRotationSensitivity = 1f;

    [SerializeField]
    private float fineRotationSpeed = 30f;

    private const string ObjectRotationSensitivityKey =
        "ObjectRotationSensitivity";


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


    // =====================================================
    // Color
    // =====================================================

    [Header("Color")]

    [Range(0f, 100f)]
    [SerializeField]
    private float sameColorChance = 50f;

    [SerializeField]
    private List<Material> propellerMaterials = new();

    [SerializeField]
    private int frontMaterialIndex = 0;

    [SerializeField]
    private int backMaterialIndex = 0;


    // =====================================================
    // Missing Part
    // =====================================================

    [Header("Missing Part")]

    [Range(0f, 100f)]
    [SerializeField]
    private float completeChance = 50f;


    // =====================================================
    // Stain
    // =====================================================

    [Header("Stain")]

    [SerializeField]
    private string bodyMeshName = "Body";

    [SerializeField]
    private int bodyMaterialIndex = 0;

    [SerializeField]
    private List<Material> bodyMaterials = new();


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

        LoadPrefab();

        isReady = ValidateData();
    }


    // =====================================================
    // Update
    // =====================================================

    private void Update()
    {
        if (!isReady)
            return;

        HandleMouseRotation();

        HandleMouseZoom();

        HandleFineRotation();
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
            r.enabled = false;
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
                "Propeller Material을 2개 이상 넣어주세요."
            );

            valid = false;
        }

        if (bodyMaterials.Count < 5)
        {
            Debug.LogError(
                "Body Material을 5개 이상 넣어주세요."
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
        }

        MeshRenderer frontRenderer =
            front.GetComponent<MeshRenderer>();

        MeshRenderer backRenderer =
            back.GetComponent<MeshRenderer>();

        if (frontRenderer == null ||
            backRenderer == null)
        {
            Debug.LogError(
                "프로펠러 MeshRenderer를 찾을 수 없습니다."
            );
        }


        // -------------------------------------------------
        // Body 찾기
        // -------------------------------------------------

        Transform body =
            FindChildRecursive(
                currentMotor.transform,
                bodyMeshName
            );

        if (body == null)
        {
            Debug.LogError(
                "Body를 찾을 수 없습니다."
            );
        }

        MeshRenderer bodyRenderer =
            body.GetComponent<MeshRenderer>();

        if (bodyRenderer == null)
        {
            Debug.LogError(
                "Body MeshRenderer가 없습니다."
            );
        }


        // -------------------------------------------------
        // 각 프로펠러 Material 결정
        // -------------------------------------------------

        int frontIndex =
            Random.Range(
                0,
                propellerMaterials.Count
            );

        int backIndex;


        // -------------------------------------------------
        // 정상 / 불량 결정
        // -------------------------------------------------

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


        // -------------------------------------------------
        // Body Material 랜덤 결정
        // -------------------------------------------------

        int bodyIndex =
            Random.Range(
                0,
                bodyMaterials.Count
            );

        bool noStain =
            bodyIndex == 0;


        // -------------------------------------------------
        // 프로펠러 색상 결정
        // -------------------------------------------------

        if (sameColor)
        {
            backIndex = frontIndex;
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
            while (backIndex == frontIndex);
        }


        // -------------------------------------------------
        // 부품 누락 결정
        // -------------------------------------------------

        if (isComplete)
        {
            front.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
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
                case 0:

                    front.gameObject.SetActive(false);
                    back.gameObject.SetActive(true);

                    break;


                case 1:

                    front.gameObject.SetActive(true);
                    back.gameObject.SetActive(false);

                    break;


                case 2:

                    front.gameObject.SetActive(false);
                    back.gameObject.SetActive(false);

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

        frontMats[frontMaterialIndex] =
            propellerMaterials[frontIndex];

        backMats[backMaterialIndex] =
            propellerMaterials[backIndex];

        frontRenderer.materials =
            frontMats;

        backRenderer.materials =
            backMats;


        // -------------------------------------------------
        // Body Material 적용
        // -------------------------------------------------

        Material[] bodyMats =
            bodyRenderer.materials;

        bodyMats[bodyMaterialIndex] =
            bodyMaterials[bodyIndex];

        bodyRenderer.materials =
            bodyMats;


        int passCount = 0;

        if (sameColor)
            passCount++;

        if (noStain)
            passCount++;

        InspectionResult result;


        // -------------------------------------------------
        // 부품이 하나라도 누락되면 폐기
        // -------------------------------------------------

        if (!isComplete)
        {
            result =
                InspectionResult.Discard;
        }
        else
        {
            if (sameColor)
                passCount++;

            if (noStain)
                passCount++;

            passCount++;


            switch (passCount)
            {
                case 3:

                    result =
                        InspectionResult.A;

                    break;


                case 2:

                    result =
                        InspectionResult.B;

                    break;


                case 1:

                    result =
                        InspectionResult.C;

                    break;


                default:

                    result =
                        InspectionResult.Discard;

                    break;
            }
        }


        // =====================================================
        // Debug
        // 정답만 표시
        // =====================================================

        Debug.Log(
            $"정답 : {result}"
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
            if (child.name == targetName)
                return child;

            Transform result =
                FindChildRecursive(
                    child,
                    targetName
                );

            if (result != null)
                return result;
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

        if (!Mouse.current.leftButton.isPressed)
            return;


        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();


        float objectRotationSensitivity =
            PlayerPrefs.GetFloat(
                ObjectRotationSensitivityKey,
                defaultObjectRotationSensitivity
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


        if (ScrollInvertSetting.IsScrollInverted())
        {
            scrollValue *= -1f;
        }


        if (Mathf.Abs(scrollValue) < 0.01f)
            return;


        currentZoomScale +=
            Mathf.Sign(scrollValue) *
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
    // WASD 미세 회전
    // =====================================================

    private void HandleFineRotation()
    {
        if (currentHolder == null)
            return;

        if (Keyboard.current == null)
            return;


        float verticalRotation = 0f;
        float horizontalRotation = 0f;


        if (Keyboard.current.wKey.isPressed)
        {
            verticalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.sKey.isPressed)
        {
            verticalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.aKey.isPressed)
        {
            horizontalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.dKey.isPressed)
        {
            horizontalRotation -=
                fineRotationSpeed *
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


        baseMouseRotationSpeed =
            Mathf.Max(
                0f,
                baseMouseRotationSpeed
            );


        defaultObjectRotationSensitivity =
            Mathf.Max(
                0f,
                defaultObjectRotationSensitivity
            );


        fineRotationSpeed =
            Mathf.Max(
                0f,
                fineRotationSpeed
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