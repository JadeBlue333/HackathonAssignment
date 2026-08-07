using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class MotorPropRandomSpawner : MonoBehaviour
{
    // =========================================================
    // Motor Material 옵션
    // =========================================================

    [System.Serializable]
    public class MotorMaterialOption
    {
        [Tooltip("구분용 이름")]
        public string optionName;

        [Tooltip("적용할 Motor Material")]
        public Material material;

        [Min(0f)]
        [Tooltip("이 Material이 선택될 상대적인 확률")]
        public float weight = 1f;

        [Range(0, 10)]
        [Tooltip("이 Material이 선택됐을 때 추가되는 점수")]
        public int score = 1;
    }


    // =========================================================
    // Propeller Material 옵션
    // =========================================================

    [System.Serializable]
    public class PropellerMaterialOption
    {
        [Tooltip("구분용 이름")]
        public string optionName;

        [Tooltip("적용할 PropellerFront Material")]
        public Material material;

        [Min(0f)]
        [Tooltip("이 Material이 선택될 상대적인 확률")]
        public float weight = 1f;
    }


    // =========================================================
    // 표시 위치 기준 Cube
    // =========================================================

    [Header("Motor Display Point")]

    [Tooltip(
        "Motor가 생성될 위치에 배치한 Cube를 넣으세요. " +
        "실행 시 Cube Renderer는 자동으로 숨겨집니다."
    )]
    public Transform displayAnchor;


    // =========================================================
    // Motor 위치 / 회전 / 크기 보정
    // =========================================================

    [Header("Motor Transform Offset")]

    [Tooltip("Display Anchor 기준 Motor 위치")]
    public Vector3 motorLocalPosition = Vector3.zero;

    [Tooltip("Display Anchor 기준 Motor 초기 회전")]
    public Vector3 motorLocalRotation = Vector3.zero;

    [Tooltip("Motor 크기")]
    public Vector3 motorScale = Vector3.one;


    // =========================================================
    // 생성 개수
    // =========================================================

    [Header("Test Count")]

    [Min(1)]
    [Tooltip("이번 테스트에서 생성할 Motor 총 개수")]
    public int totalMotorCount = 5;


    // =========================================================
    // 마우스 회전
    // =========================================================

    [Header("Mouse Rotation")]

    [Tooltip("좌클릭 드래그 회전 속도")]
    public float mouseRotationSpeed = 0.2f;


    // =========================================================
    // WASD 미세 회전
    // =========================================================

    [Header("WASD Fine Rotation")]

    [Tooltip("WASD 미세 회전 속도")]
    public float fineRotationSpeed = 30f;


    // =========================================================
    // Motor Material
    // =========================================================

    [Header("AC Motor Materials")]

    [Tooltip("Motor 0 ~ Motor 4를 등록")]
    public MotorMaterialOption[] motorMaterials;


    // =========================================================
    // Propeller Material
    // =========================================================

    [Header("PropellerFront Materials")]

    [Tooltip("PropellerFront 0 ~ 3을 등록")]
    public PropellerMaterialOption[] propellerMaterials;


    // =========================================================
    // prop poly 설정
    // =========================================================

    [Header("Prop Poly Settings")]

    [Range(0f, 100f)]
    [Tooltip("prop poly가 사라질 확률 (%)")]
    public float propPolyDeleteChance = 20f;

    [Tooltip("prop poly에서 변경할 Material Element 번호")]
    public int propPolyMaterialIndex = 0;


    // =========================================================
    // Propeller 설정
    // =========================================================

    [Header("Propeller Settings")]

    [Tooltip(
        "Propeller에서 변경할 Material Element 번호. " +
        "Element 1이면 1"
    )]
    public int propellerMaterialIndex = 1;


    // =========================================================
    // 점수 설정
    // =========================================================

    [Header("Score Settings")]

    [Range(0, 10)]
    [Tooltip("prop poly가 없을 때 추가되는 점수")]
    public int missingPropPolyScore = 1;

    [Range(0, 10)]
    [Tooltip(
        "Propeller와 prop poly Material이 다를 때 추가되는 점수"
    )]
    public int materialMismatchScore = 1;


    // =========================================================
    // 내부 변수
    // =========================================================

    private GameObject motorPropPrefab;

    // 회전 중심
    private GameObject currentMotorHolder;

    // 실제 motorProp
    private GameObject currentMotorProp;


    // 현재 몇 번째 Motor인지
    private int currentMotorNumber = 0;

    private bool isReady = false;
    private bool testFinished = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // -----------------------------------------------------
        // 위치 기준 Cube 숨기기
        // -----------------------------------------------------

        HideDisplayAnchor();


        // -----------------------------------------------------
        // 기존 방식 그대로:
        // Resources에서 motorProp 자동 로드
        // -----------------------------------------------------

        motorPropPrefab =
            Resources.Load<GameObject>(
                "Motors+Prop/motorProp"
            );


        if (motorPropPrefab == null)
        {
            Debug.LogError(
                "motorProp Prefab을 찾을 수 없습니다.\n" +
                "Prefab 위치를 확인하세요:\n" +
                "Assets/Resources/Motors+Prop/motorProp.prefab"
            );

            return;
        }


        // -----------------------------------------------------
        // Display Anchor 확인
        // -----------------------------------------------------

        if (displayAnchor == null)
        {
            Debug.LogError(
                "Motor Display Point가 설정되지 않았습니다.\n" +
                "씬에 Cube를 만들고 Display Anchor에 연결하세요."
            );

            return;
        }


        isReady = true;


        // -----------------------------------------------------
        // 실행 직후 첫 번째 Motor 생성
        // -----------------------------------------------------

        SpawnNextMotor();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (!isReady || testFinished)
            return;


        // 마우스 = 큰 회전
        HandleMouseRotation();


        // WASD = 미세 회전
        HandleFineRotation();


        // P = 다음 Motor
#if ENABLE_INPUT_SYSTEM

        if (Keyboard.current != null &&
            Keyboard.current.pKey.wasPressedThisFrame)
        {
            GoToNextMotor();
        }

#elif ENABLE_LEGACY_INPUT_MANAGER

        if (Input.GetKeyDown(KeyCode.P))
        {
            GoToNextMotor();
        }

#endif
    }


    // =========================================================
    // Display Anchor Cube 숨기기
    // =========================================================

    private void HideDisplayAnchor()
    {
        if (displayAnchor == null)
            return;


        // Cube 포함 자식의 Renderer를 모두 숨김
        Renderer[] renderers =
            displayAnchor.GetComponentsInChildren<Renderer>(
                true
            );


        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }


    // =========================================================
    // P키
    // =========================================================

    private void GoToNextMotor()
    {
        // 마지막 Motor를 보고 있는 상태에서 P를 누르면 종료
        if (currentMotorNumber >= totalMotorCount)
        {
            testFinished = true;

            return;
        }


        // 다음 Motor 생성
        SpawnNextMotor();
    }


    // =========================================================
    // 다음 Motor
    // =========================================================

    private void SpawnNextMotor()
    {
        currentMotorNumber++;

        SpawnMotorProp();
    }


    // =========================================================
    // motorProp 생성
    // =========================================================

    public void SpawnMotorProp()
    {
        // -----------------------------------------------------
        // Prefab 확인
        // -----------------------------------------------------

        if (motorPropPrefab == null)
        {
            Debug.LogError(
                "motorProp Prefab이 로드되지 않았습니다."
            );

            return;
        }


        // -----------------------------------------------------
        // 이전 Motor 제거
        // -----------------------------------------------------

        RemoveCurrentMotor();


        // =====================================================
        // 회전 중심 Holder 생성
        // =====================================================

        currentMotorHolder =
            new GameObject(
                "CurrentMotorHolder"
            );


        // Cube의 자식으로 들어감
        currentMotorHolder.transform.SetParent(
            displayAnchor,
            false
        );


        // Cube 중심에 위치
        currentMotorHolder.transform.localPosition =
            Vector3.zero;


        currentMotorHolder.transform.localRotation =
            Quaternion.identity;


        currentMotorHolder.transform.localScale =
            Vector3.one;


        // =====================================================
        // 실제 motorProp 생성
        // =====================================================

        currentMotorProp =
            Instantiate(
                motorPropPrefab,
                currentMotorHolder.transform
            );


        // Cube 기준 상대 위치
        currentMotorProp.transform.localPosition =
            motorLocalPosition;


        // 초기 회전
        currentMotorProp.transform.localRotation =
            Quaternion.Euler(
                motorLocalRotation
            );


        // 크기
        currentMotorProp.transform.localScale =
            motorScale;


        // =====================================================
        // 내부 오브젝트 찾기
        // =====================================================

        Transform acMotor =
            FindChildRecursive(
                currentMotorProp.transform,
                "AC Motor"
            );


        Transform propeller =
            FindChildRecursive(
                currentMotorProp.transform,
                "Propeller"
            );


        Transform propPoly =
            FindChildRecursive(
                currentMotorProp.transform,
                "prop poly"
            );


        // =====================================================
        // 필수 오브젝트 확인
        // =====================================================

        if (acMotor == null)
        {
            Debug.LogError(
                "motorProp 안에서 'AC Motor'를 찾을 수 없습니다."
            );

            return;
        }


        if (propeller == null)
        {
            Debug.LogError(
                "motorProp 안에서 'Propeller'를 찾을 수 없습니다."
            );

            return;
        }


        // =====================================================
        // 1. AC Motor Material 랜덤 선택
        // =====================================================

        MotorMaterialOption selectedMotorOption =
            GetRandomMotorMaterial();


        if (selectedMotorOption == null ||
            selectedMotorOption.material == null)
        {
            Debug.LogError(
                "Motor Material 설정을 확인하세요."
            );

            return;
        }


        MeshRenderer acMotorRenderer =
            acMotor.GetComponent<MeshRenderer>();


        if (acMotorRenderer == null)
        {
            acMotorRenderer =
                acMotor.GetComponentInChildren<MeshRenderer>();
        }


        if (acMotorRenderer == null)
        {
            Debug.LogError(
                "AC Motor의 Mesh Renderer를 찾을 수 없습니다."
            );

            return;
        }


        Material[] motorMats =
            acMotorRenderer.materials;


        if (motorMats.Length > 0)
        {
            motorMats[0] =
                selectedMotorOption.material;


            acMotorRenderer.materials =
                motorMats;
        }
        else
        {
            Debug.LogError(
                "AC Motor에 Material 슬롯이 없습니다."
            );

            return;
        }


        // =====================================================
        // 2. Propeller Element 1 Material 랜덤
        // =====================================================

        PropellerMaterialOption selectedPropellerOption =
            GetRandomPropellerMaterial();


        if (selectedPropellerOption == null ||
            selectedPropellerOption.material == null)
        {
            Debug.LogError(
                "Propeller Material 설정을 확인하세요."
            );

            return;
        }


        MeshRenderer propellerRenderer =
            propeller.GetComponent<MeshRenderer>();


        if (propellerRenderer == null)
        {
            propellerRenderer =
                propeller.GetComponentInChildren<MeshRenderer>();
        }


        if (propellerRenderer == null)
        {
            Debug.LogError(
                "Propeller의 Mesh Renderer를 찾을 수 없습니다."
            );

            return;
        }


        Material[] propellerMats =
            propellerRenderer.materials;


        if (propellerMaterialIndex < 0 ||
            propellerMaterialIndex >=
            propellerMats.Length)
        {
            Debug.LogError(
                "Propeller Material Index가 잘못되었습니다.\n" +
                "현재 설정값: " +
                propellerMaterialIndex +
                "\nPropeller Material 슬롯 개수: " +
                propellerMats.Length
            );

            return;
        }


        // Element 1만 변경
        propellerMats[
            propellerMaterialIndex
        ] =
            selectedPropellerOption.material;


        propellerRenderer.materials =
            propellerMats;


        // =====================================================
        // 3. prop poly 삭제 여부
        // =====================================================

        bool propPolyExists =
            propPoly != null;


        if (propPolyExists)
        {
            float randomValue =
                Random.Range(
                    0f,
                    100f
                );


            if (randomValue <
                propPolyDeleteChance)
            {
                propPolyExists =
                    false;


                propPoly.gameObject.SetActive(
                    false
                );
            }
        }


        // =====================================================
        // 4. prop poly Material 랜덤 선택
        // =====================================================

        Material selectedPropPolyMaterial =
            null;


        if (propPolyExists)
        {
            PropellerMaterialOption selectedPropPolyOption =
                GetRandomPropellerMaterial();


            if (selectedPropPolyOption != null &&
                selectedPropPolyOption.material != null)
            {
                selectedPropPolyMaterial =
                    selectedPropPolyOption.material;


                MeshRenderer propPolyRenderer =
                    propPoly.GetComponent<MeshRenderer>();


                if (propPolyRenderer == null)
                {
                    propPolyRenderer =
                        propPoly.GetComponentInChildren<MeshRenderer>();
                }


                if (propPolyRenderer == null)
                {
                    Debug.LogError(
                        "prop poly의 Mesh Renderer를 찾을 수 없습니다."
                    );
                }
                else
                {
                    Material[] propPolyMats =
                        propPolyRenderer.materials;


                    if (
                        propPolyMaterialIndex >= 0 &&
                        propPolyMaterialIndex <
                        propPolyMats.Length
                    )
                    {
                        propPolyMats[
                            propPolyMaterialIndex
                        ] =
                            selectedPropPolyMaterial;


                        propPolyRenderer.materials =
                            propPolyMats;
                    }
                    else
                    {
                        Debug.LogError(
                            "prop poly Material Index가 잘못되었습니다.\n" +
                            "현재 설정값: " +
                            propPolyMaterialIndex +
                            "\nMaterial 슬롯 개수: " +
                            propPolyMats.Length
                        );
                    }
                }
            }
        }


        // =====================================================
        // 점수 계산
        // =====================================================

        int finalScore = 0;


        // Motor Material 점수
        finalScore +=
            selectedMotorOption.score;


        // prop poly가 없으면 추가
        if (!propPolyExists)
        {
            finalScore +=
                missingPropPolyScore;
        }


        // =====================================================
        // Material 일치 여부
        // =====================================================

        bool materialSame =
            false;


        if (
            propPolyExists &&
            selectedPropPolyMaterial != null
        )
        {
            materialSame =
                selectedPropellerOption.material ==
                selectedPropPolyMaterial;
        }


        // 다르면 점수 추가
        if (!materialSame)
        {
            finalScore +=
                materialMismatchScore;
        }


        // =====================================================
        // 기존 Console 출력 유지
        // =====================================================

        Debug.Log(finalScore);
    }


    // =========================================================
    // 마우스 회전
    // 전부 월드 기준
    // =========================================================

    private void HandleMouseRotation()
    {
        if (currentMotorHolder == null)
            return;


#if ENABLE_INPUT_SYSTEM

        if (Mouse.current == null)
            return;


        if (!Mouse.current.leftButton.isPressed)
            return;


        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();


        // -----------------------------------------------------
        // 좌우
        // -----------------------------------------------------

        float horizontalRotation =
            mouseDelta.x *
            -mouseRotationSpeed;


        // -----------------------------------------------------
        // 위아래
        // -----------------------------------------------------

        float verticalRotation =
            mouseDelta.y *
            mouseRotationSpeed;


        // -----------------------------------------------------
        // 월드 Y축 회전
        // -----------------------------------------------------

        currentMotorHolder.transform.Rotate(
            Vector3.up,
            horizontalRotation,
            Space.World
        );


        // -----------------------------------------------------
        // 월드 X축 회전
        // -----------------------------------------------------

        currentMotorHolder.transform.Rotate(
            Vector3.right,
            verticalRotation,
            Space.World
        );

#endif
    }


    // =========================================================
    // WASD 미세 회전
    // 전부 월드 기준
    // =========================================================

    private void HandleFineRotation()
    {
        if (currentMotorHolder == null)
            return;


#if ENABLE_INPUT_SYSTEM

        if (Keyboard.current == null)
            return;


        float verticalRotation = 0f;
        float horizontalRotation = 0f;


        // =====================================================
        // W
        // =====================================================

        if (Keyboard.current.wKey.isPressed)
        {
            verticalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // =====================================================
        // S
        // =====================================================

        if (Keyboard.current.sKey.isPressed)
        {
            verticalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // =====================================================
        // A
        // 좌우 반전 적용된 방향
        // =====================================================

        if (Keyboard.current.aKey.isPressed)
        {
            horizontalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // =====================================================
        // D
        // =====================================================

        if (Keyboard.current.dKey.isPressed)
        {
            horizontalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // -----------------------------------------------------
        // W / S
        // 월드 X축
        // -----------------------------------------------------

        if (verticalRotation != 0f)
        {
            currentMotorHolder.transform.Rotate(
                Vector3.right,
                verticalRotation,
                Space.World
            );
        }


        // -----------------------------------------------------
        // A / D
        // 월드 Y축
        // -----------------------------------------------------

        if (horizontalRotation != 0f)
        {
            currentMotorHolder.transform.Rotate(
                Vector3.up,
                horizontalRotation,
                Space.World
            );
        }

#endif
    }


    // =========================================================
    // 이전 Motor 제거
    // =========================================================

    private void RemoveCurrentMotor()
    {
        if (currentMotorHolder != null)
        {
            Destroy(
                currentMotorHolder
            );


            currentMotorHolder =
                null;


            currentMotorProp =
                null;
        }
    }


    // =========================================================
    // Motor Material Weighted Random
    // =========================================================

    private MotorMaterialOption
        GetRandomMotorMaterial()
    {
        if (
            motorMaterials == null ||
            motorMaterials.Length == 0
        )
        {
            return null;
        }


        float totalWeight = 0f;


        foreach (
            MotorMaterialOption option
            in motorMaterials
        )
        {
            if (option != null)
            {
                totalWeight +=
                    Mathf.Max(
                        0f,
                        option.weight
                    );
            }
        }


        if (totalWeight <= 0f)
        {
            return null;
        }


        float randomValue =
            Random.Range(
                0f,
                totalWeight
            );


        float currentWeight =
            0f;


        foreach (
            MotorMaterialOption option
            in motorMaterials
        )
        {
            if (option == null)
                continue;


            currentWeight +=
                Mathf.Max(
                    0f,
                    option.weight
                );


            if (
                randomValue <=
                currentWeight
            )
            {
                return option;
            }
        }


        return motorMaterials[
            motorMaterials.Length - 1
        ];
    }


    // =========================================================
    // Propeller Material Weighted Random
    // =========================================================

    private PropellerMaterialOption
        GetRandomPropellerMaterial()
    {
        if (
            propellerMaterials == null ||
            propellerMaterials.Length == 0
        )
        {
            return null;
        }


        float totalWeight =
            0f;


        foreach (
            PropellerMaterialOption option
            in propellerMaterials
        )
        {
            if (option != null)
            {
                totalWeight +=
                    Mathf.Max(
                        0f,
                        option.weight
                    );
            }
        }


        if (totalWeight <= 0f)
        {
            return null;
        }


        float randomValue =
            Random.Range(
                0f,
                totalWeight
            );


        float currentWeight =
            0f;


        foreach (
            PropellerMaterialOption option
            in propellerMaterials
        )
        {
            if (option == null)
                continue;


            currentWeight +=
                Mathf.Max(
                    0f,
                    option.weight
                );


            if (
                randomValue <=
                currentWeight
            )
            {
                return option;
            }
        }


        return propellerMaterials[
            propellerMaterials.Length - 1
        ];
    }


    // =========================================================
    // 하위 오브젝트 이름 검색
    // =========================================================

    private Transform FindChildRecursive(
        Transform parent,
        string targetName
    )
    {
        foreach (
            Transform child
            in parent
        )
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


    // =========================================================
    // Inspector 값 제한
    // =========================================================

    private void OnValidate()
    {
        totalMotorCount =
            Mathf.Max(
                1,
                totalMotorCount
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


        propPolyDeleteChance =
            Mathf.Clamp(
                propPolyDeleteChance,
                0f,
                100f
            );
    }
}