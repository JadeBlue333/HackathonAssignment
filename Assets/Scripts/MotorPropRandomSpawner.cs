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
    // 생성 위치
    // =========================================================

    [Header("Spawn Settings")]

    [Tooltip("motorProp 생성 위치")]
    public Vector3 spawnPosition = Vector3.zero;

    [Tooltip("motorProp 생성 회전값")]
    public Vector3 spawnRotation = Vector3.zero;

    [Tooltip("motorProp 생성 크기")]
    public Vector3 spawnScale = Vector3.one;


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

    [Tooltip("Propeller의 변경할 Material Element 번호. Element 1이면 1")]
    public int propellerMaterialIndex = 1;


    // =========================================================
    // 점수 설정
    // =========================================================

    [Header("Score Settings")]

    [Range(0, 10)]
    [Tooltip("prop poly가 없을 때 추가되는 점수")]
    public int missingPropPolyScore = 1;

    [Range(0, 10)]
    [Tooltip("Propeller와 prop poly의 Material이 다를 때 추가되는 점수")]
    public int materialMismatchScore = 1;


    // =========================================================
    // 내부 변수
    // =========================================================

    private GameObject motorPropPrefab;

    private GameObject currentMotorProp;


    // =========================================================
    // 시작
    // =========================================================

    void Start()
    {
        // Assets/Resources/Motors+Prop/motorProp.prefab
        // 을 자동으로 찾음
        motorPropPrefab =
            Resources.Load<GameObject>("Motors+Prop/motorProp");


        if (motorPropPrefab == null)
        {
            Debug.LogError(
                "motorProp Prefab을 찾을 수 없습니다.\n" +
                "Prefab 위치를 확인하세요:\n" +
                "Assets/Resources/Motors+Prop/motorProp.prefab"
            );
        }
    }


    // =========================================================
    // 화면 클릭 감지
    // =========================================================

    void Update()
    {
        bool clicked = false;


#if ENABLE_INPUT_SYSTEM

        // 새로운 Input System 사용
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicked = true;
        }

#elif ENABLE_LEGACY_INPUT_MANAGER

        // 기존 Input Manager 사용
        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
        }

#endif


        if (clicked)
        {
            SpawnMotorProp();
        }
    }


    // =========================================================
    // motorProp 생성 및 랜덤 설정
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
        // 이전에 생성한 테스트용 motorProp 삭제
        // -----------------------------------------------------

        if (currentMotorProp != null)
        {
            Destroy(currentMotorProp);
        }


        // -----------------------------------------------------
        // 새 motorProp 생성
        // -----------------------------------------------------

        Quaternion rotation =
            Quaternion.Euler(spawnRotation);


        currentMotorProp =
            Instantiate(
                motorPropPrefab,
                spawnPosition,
                rotation
            );


        currentMotorProp.transform.localScale =
            spawnScale;


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


        // -----------------------------------------------------
        // 필수 오브젝트 확인
        // -----------------------------------------------------

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


        // AC Motor의 Renderer 찾기
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


        // AC Motor Material 변경
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
        // 2. Propeller Element 1 Material 랜덤 선택
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
            propellerMaterialIndex >= propellerMats.Length)
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


        propellerMats[propellerMaterialIndex] =
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
                Random.Range(0f, 100f);


            if (randomValue <
                propPolyDeleteChance)
            {
                propPolyExists =
                    false;


                // 실제 Prefab을 파괴하지 않고
                // 이번에 생성된 복제본만 비활성화
                propPoly.gameObject.SetActive(false);
            }
        }


        // =====================================================
        // 4. prop poly가 있으면 Material 랜덤 선택
        // =====================================================

        Material selectedPropPolyMaterial =
            null;


        if (propPolyExists)
        {
            PropellerMaterialOption
                selectedPropPolyOption =
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


                    if (propPolyMaterialIndex >= 0 &&
                        propPolyMaterialIndex <
                        propPolyMats.Length)
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


        // -----------------------------------------------------
        // 조건 1
        // Motor Material에 지정된 점수
        //
        // Motor 0 = 0
        // 나머지 기본 1
        // Inspector에서 0~10 변경 가능
        // -----------------------------------------------------

        finalScore +=
            selectedMotorOption.score;


        // -----------------------------------------------------
        // 조건 2
        // prop poly 존재 여부
        // -----------------------------------------------------

        if (!propPolyExists)
        {
            finalScore +=
                missingPropPolyScore;
        }


        // -----------------------------------------------------
        // 조건 3
        // Propeller Element 1과
        // prop poly Material 일치 여부
        // -----------------------------------------------------

        bool materialSame =
            false;


        if (propPolyExists &&
            selectedPropPolyMaterial != null)
        {
            materialSame =
                selectedPropellerOption.material ==
                selectedPropPolyMaterial;
        }


        // prop poly가 없으면
        // Material 비교 조건도 실패로 처리
        if (!materialSame)
        {
            finalScore +=
                materialMismatchScore;
        }


        // =====================================================
        // 콘솔 출력
        // =====================================================

        string propPolyStatus =
            propPolyExists
            ? "존재"
            : "삭제됨";


        string propPolyMaterialName =
            selectedPropPolyMaterial != null
            ? selectedPropPolyMaterial.name
            : "없음";


        Debug.Log(finalScore);
    }


    // =========================================================
    // Motor Material Weighted Random
    // =========================================================

    private MotorMaterialOption
        GetRandomMotorMaterial()
    {
        if (motorMaterials == null ||
            motorMaterials.Length == 0)
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
                    Mathf.Max(0f, option.weight);
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


            if (randomValue <=
                currentWeight)
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
        if (propellerMaterials == null ||
            propellerMaterials.Length == 0)
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


            if (randomValue <=
                currentWeight)
            {
                return option;
            }
        }


        return propellerMaterials[
            propellerMaterials.Length - 1
        ];
    }


    // =========================================================
    // 하위 오브젝트 이름으로 찾기
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
            if (child.name ==
                targetName)
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
}
