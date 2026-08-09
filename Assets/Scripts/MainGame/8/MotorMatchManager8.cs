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
    // Rotation
    // =====================================================

    [Header("Rotation")]

    [SerializeField]
    private float mouseRotationSpeed = 0.2f;

    [SerializeField]
    private float fineRotationSpeed = 30f;


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

    [Header("Material Index")]

    [SerializeField]
    private int frontMaterialIndex = 0;

    [SerializeField]
    private int backMaterialIndex = 0;


    [Header("Color")]

    [Range(0f, 100f)]
    [SerializeField]
    private float sameColorChance = 50f;

    [Header("Missing Part")]

    [Range(0f, 100f)]
    [SerializeField]
    private float completeChance = 50f;


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
            Debug.LogError("Motor Prefab이 없습니다.");
            valid = false;
        }

        if (displayAnchor == null)
        {
            Debug.LogError("DisplayAnchor가 없습니다.");
            valid = false;
        }

        if (propellerMaterials.Count < 2)
        {
            Debug.LogError("Material을 2개 이상 넣어주세요.");
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
            Debug.LogError("프로펠러를 찾을 수 없습니다.");

            return InspectionResult.B;
        }


        MeshRenderer frontRenderer =
            front.GetComponent<MeshRenderer>();

        MeshRenderer backRenderer =
            back.GetComponent<MeshRenderer>();

        if (frontRenderer == null ||
            backRenderer == null)
        {
            Debug.LogError("MeshRenderer를 찾을 수 없습니다.");

            return InspectionResult.B;
        }


        // -------------------------------------------------
        // 앞 프로펠러 Material 선택
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
    Random.Range(0f, 100f) < completeChance;


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

        if (isComplete)
        {
            front.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
        }
        else
        {
            int missingType = Random.Range(0, 3);

            switch (missingType)
            {
                // 앞만 없음
                case 0:
                    front.gameObject.SetActive(false);
                    back.gameObject.SetActive(true);
                    break;

                // 뒤만 없음
                case 1:
                    front.gameObject.SetActive(true);
                    back.gameObject.SetActive(false);
                    break;

                // 둘 다 없음
                case 2:
                    front.gameObject.SetActive(false);
                    back.gameObject.SetActive(false);
                    break;
            }
        }

        // -------------------------------------------------
        // Material 적용
        // -------------------------------------------------

        Material[] frontMats = frontRenderer.materials;
        Material[] backMats = backRenderer.materials;

        frontMats[frontMaterialIndex] =
            propellerMaterials[frontIndex];

        backMats[backMaterialIndex] =
            propellerMaterials[backIndex];

        frontRenderer.materials = frontMats;
        backRenderer.materials = backMats;

        InspectionResult result;

        if (!isComplete)
        {
            // 부품이 하나라도 없으면 무조건 폐기
            result = InspectionResult.Discard;
        }
        else if (sameColor)
        {
            // 부품 완성 + 색깔 정상
            result = InspectionResult.A;
        }
        else
        {
            // 부품 완성 + 색깔 비정상
            result = InspectionResult.B;
        }

        Debug.Log(
            $"Color : {(sameColor ? "Same" : "Different")} | " +
            $"Part : {(isComplete ? "Complete" : "Missing")} | " +
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
            Destroy(currentHolder);

            currentHolder = null;
            currentMotor = null;
        }
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

        float horizontalRotation =
            -mouseDelta.x *
            mouseRotationSpeed;

        float verticalRotation =
            mouseDelta.y *
            mouseRotationSpeed;

        // 좌우
        currentHolder.transform.Rotate(
            Vector3.up,
            horizontalRotation,
            Space.World
        );

        // 위아래
        currentHolder.transform.Rotate(
            Vector3.left,
            verticalRotation,
            Space.World
        );
    }


    // =====================================================
    // WASD 회전
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
    // Inspector 제한
    // =====================================================

    private void OnValidate()
    {
        sameColorChance =
            Mathf.Clamp(
                sameColorChance,
                0f,
                100f
            );

        mouseRotationSpeed =
            Mathf.Max(
                0f,
                mouseRotationSpeed
            );

        fineRotationSpeed =
            Mathf.Max(
                0f,
                fineRotationSpeed
            );
    }
}