using UnityEngine;
using UnityEngine.InputSystem;

public class RandomMotor : MonoBehaviour
{
    // =========================================================
    // 확률 설정
    // =========================================================
    //
    // 여기 있는 값들만 수정하면 랜덤 생성 확률을 조절할 수 있음.
    //
    // Spawn Chance는 실제 퍼센트(%)
    // 예) 50 = 50%, 80 = 80%, 0 = 절대 생성 안 됨, 100 = 무조건 생성
    //
    // Weight는 "가중치" 방식
    // 예)
    // Motor0 = 10
    // Motor1 = 1
    // Motor2 = 1
    // 이면 Motor0가 다른 것보다 10배 자주 나옴.
    //
    // 전부 1이면 모두 같은 확률.
    // =========================================================


    [Header("Probability - Spawn")]

    [Range(0f, 100f)]
    [Tooltip("앞 프로펠러가 생성될 확률 (%)")]
    public float frontSpawnChance = 80f;

    [Range(0f, 100f)]
    [Tooltip("뒤 프로펠러가 생성될 확률 (%)")]
    public float backSpawnChance = 80f;



    [Header("Probability - Motor Material")]

    [Tooltip("Motor 0의 등장 가중치")]
    public float motor0Weight = 1f;

    [Tooltip("Motor 1의 등장 가중치")]
    public float motor1Weight = 1f;

    [Tooltip("Motor 2의 등장 가중치")]
    public float motor2Weight = 1f;

    [Tooltip("Motor 3의 등장 가중치")]
    public float motor3Weight = 1f;

    [Tooltip("Motor 4의 등장 가중치")]
    public float motor4Weight = 1f;



    [Header("Probability - Front Propeller Type")]

    [Tooltip("Front 3의 등장 가중치")]
    public float front3Weight = 1f;

    [Tooltip("Front 4의 등장 가중치")]
    public float front4Weight = 1f;

    [Tooltip("Front 5의 등장 가중치")]
    public float front5Weight = 1f;



    [Header("Probability - Back Propeller Type")]

    [Tooltip("Back 3의 등장 가중치")]
    public float back3Weight = 1f;

    [Tooltip("Back 4의 등장 가중치")]
    public float back4Weight = 1f;

    [Tooltip("Back 5의 등장 가중치")]
    public float back5Weight = 1f;



    [Header("Probability - Front Propeller Color")]

    [Tooltip("프로펠러 컬러 0의 등장 가중치")]
    public float frontColor0Weight = 1f;

    [Tooltip("프로펠러 컬러 1의 등장 가중치")]
    public float frontColor1Weight = 1f;

    [Tooltip("프로펠러 컬러 2의 등장 가중치")]
    public float frontColor2Weight = 1f;

    [Tooltip("프로펠러 컬러 3의 등장 가중치")]
    public float frontColor3Weight = 1f;



    [Header("Probability - Back Propeller Color")]

    [Tooltip("프로펠러 컬러 0의 등장 가중치")]
    public float backColor0Weight = 1f;

    [Tooltip("프로펠러 컬러 1의 등장 가중치")]
    public float backColor1Weight = 1f;

    [Tooltip("프로펠러 컬러 2의 등장 가중치")]
    public float backColor2Weight = 1f;

    [Tooltip("프로펠러 컬러 3의 등장 가중치")]
    public float backColor3Weight = 1f;



    // =========================================================
    // 디버그 설정
    // =========================================================

    [Header("Debug")]

    [Tooltip("모터가 생성될 때 결과를 Console에 출력할지 여부")]
    public bool showDebugLog = true;



    // =========================================================
    // 오브젝트 / Material 연결
    // =========================================================

    [Header("Main Body")]

    // 항상 존재하는 모터 본체
    public GameObject motorMain;

    // Element 순서:
    // 0 = Motor 0
    // 1 = Motor 1
    // 2 = Motor 2
    // 3 = Motor 3
    // 4 = Motor 4
    public Material[] motorMaterials;



    [Header("Front Propeller")]

    // Element 순서:
    // 0 = prof_front_3
    // 1 = prof_front_4
    // 2 = prof_front_5
    public GameObject[] frontPropellers;



    [Header("Back Propeller")]

    // Element 순서:
    // 0 = prof_back_3
    // 1 = prof_back_4
    // 2 = prof_back_5
    public GameObject[] backPropellers;



    [Header("Propeller Materials")]

    // Element 순서:
    // 0 = PropellerFront 0
    // 1 = PropellerFront 1
    // 2 = PropellerFront 2
    // 3 = PropellerFront 3
    public Material[] propellerMaterials;



    // =========================================================
    // 다른 코드에서 가져갈 최종 판정 결과
    // =========================================================
    //
    // true:
    // 올바른 모터
    //
    // false:
    // 틀린 모터
    //
    // 다른 스크립트에서:
    // randomMotor.IsValidMotor
    // 로 바로 읽을 수 있음.
    // =========================================================

    public bool IsValidMotor { get; private set; }



    // =========================================================
    // 현재 생성된 모터 정보
    // =========================================================

    // 앞 프로펠러가 존재하는가?
    private bool hasFront;

    // 뒤 프로펠러가 존재하는가?
    private bool hasBack;


    // 현재 선택된 앞 프로펠러 번호
    // 3 / 4 / 5
    // 없으면 -1
    private int selectedFrontNumber = -1;

    // 현재 선택된 뒤 프로펠러 번호
    // 3 / 4 / 5
    // 없으면 -1
    private int selectedBackNumber = -1;


    // 현재 선택된 앞 프로펠러 컬러 번호
    // 0 ~ 3
    // 없으면 -1
    private int selectedFrontColor = -1;

    // 현재 선택된 뒤 프로펠러 컬러 번호
    // 0 ~ 3
    // 없으면 -1
    private int selectedBackColor = -1;


    // 현재 선택된 Motor Material 번호
    // 0 ~ 4
    private int selectedMotorMaterial = -1;



    // =========================================================
    // 게임 시작
    // =========================================================

    void Start()
    {
        // 시작하자마자 모터 한 번 생성
        GenerateMotor();
    }



    // =========================================================
    // 테스트용 입력
    // =========================================================

    void Update()
    {
        // R키를 누르면 새로운 랜덤 모터 생성
        // New Input System 사용
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            GenerateMotor();
        }
    }



    // =========================================================
    // 모터 전체 생성
    // =========================================================

    public void GenerateMotor()
    {
        // =====================================================
        // 1. 본체 켜기
        // =====================================================

        motorMain.SetActive(true);



        // =====================================================
        // 2. 본체 Material 선택
        // =====================================================

        Renderer motorRenderer =
            motorMain.GetComponentInChildren<Renderer>();


        if (motorRenderer != null &&
            motorMaterials.Length >= 5)
        {
            // 위에서 설정한 Motor 가중치를 배열로 만듦
            float[] motorWeights =
            {
                motor0Weight,
                motor1Weight,
                motor2Weight,
                motor3Weight,
                motor4Weight
            };


            // 가중치에 따라 Motor 0~4 중 하나 선택
            selectedMotorMaterial =
                GetWeightedRandomIndex(motorWeights);


            // 선택된 Material 적용
            motorRenderer.material =
                motorMaterials[selectedMotorMaterial];
        }



        // =====================================================
        // 3. 앞 프로펠러 초기화
        // =====================================================

        // 기존에 켜져있던 앞 프로펠러 모두 끔
        foreach (GameObject prop in frontPropellers)
        {
            prop.SetActive(false);
        }


        // 선택 정보 초기화
        hasFront = false;
        selectedFrontNumber = -1;
        selectedFrontColor = -1;



        // =====================================================
        // 4. 앞 프로펠러 생성 여부 결정
        // =====================================================

        // 0~100 사이 랜덤값이
        // frontSpawnChance보다 작으면 생성
        //
        // 예:
        // frontSpawnChance = 70
        // → 70% 확률로 true
        hasFront =
            Random.Range(0f, 100f) < frontSpawnChance;



        // =====================================================
        // 5. 앞 프로펠러 종류 + 컬러 결정
        // =====================================================

        if (hasFront && frontPropellers.Length >= 3)
        {
            // 앞 프로펠러 종류별 가중치
            float[] frontTypeWeights =
            {
                front3Weight,
                front4Weight,
                front5Weight
            };


            // 3종 중 하나 선택
            //
            // index 0 = front_3
            // index 1 = front_4
            // index 2 = front_5
            int randomFront =
                GetWeightedRandomIndex(frontTypeWeights);


            // 선택된 프로펠러 가져오기
            GameObject selectedFront =
                frontPropellers[randomFront];


            // 선택된 것만 활성화
            selectedFront.SetActive(true);


            // 이름에서 3 / 4 / 5 가져오기
            selectedFrontNumber =
                GetPropellerNumber(selectedFront);


            // Mesh Renderer 찾기
            Renderer frontRenderer =
                selectedFront.GetComponentInChildren<Renderer>();



            if (frontRenderer != null &&
                propellerMaterials.Length >= 4)
            {
                // 현재 프로펠러의 Material 슬롯 가져오기
                Material[] materials =
                    frontRenderer.materials;


                // 앞 프로펠러 컬러별 가중치
                float[] frontColorWeights =
                {
                    frontColor0Weight,
                    frontColor1Weight,
                    frontColor2Weight,
                    frontColor3Weight
                };


                // 4개 컬러 중 하나 선택
                selectedFrontColor =
                    GetWeightedRandomIndex(frontColorWeights);


                // 앞 프로펠러는
                // Material Element 0의 컬러만 변경
                if (materials.Length > 0)
                {
                    materials[0] =
                        propellerMaterials[selectedFrontColor];

                    frontRenderer.materials = materials;
                }
            }
        }



        // =====================================================
        // 6. 뒤 프로펠러 초기화
        // =====================================================

        // 기존에 켜져있던 뒤 프로펠러 모두 끔
        foreach (GameObject prop in backPropellers)
        {
            prop.SetActive(false);
        }


        // 선택 정보 초기화
        hasBack = false;
        selectedBackNumber = -1;
        selectedBackColor = -1;



        // =====================================================
        // 7. 뒤 프로펠러 생성 여부 결정
        // =====================================================

        // backSpawnChance에 설정된 확률로 생성
        hasBack =
            Random.Range(0f, 100f) < backSpawnChance;



        // =====================================================
        // 8. 뒤 프로펠러 종류 + 컬러 결정
        // =====================================================

        if (hasBack && backPropellers.Length >= 3)
        {
            // 뒤 프로펠러 종류별 가중치
            float[] backTypeWeights =
            {
                back3Weight,
                back4Weight,
                back5Weight
            };


            // 3종 중 하나 선택
            int randomBack =
                GetWeightedRandomIndex(backTypeWeights);


            // 선택된 오브젝트 가져오기
            GameObject selectedBack =
                backPropellers[randomBack];


            // 선택된 것만 켜기
            selectedBack.SetActive(true);


            // 이름에서 숫자 3 / 4 / 5 가져오기
            selectedBackNumber =
                GetPropellerNumber(selectedBack);


            // Renderer 가져오기
            Renderer backRenderer =
                selectedBack.GetComponentInChildren<Renderer>();



            if (backRenderer != null &&
                propellerMaterials.Length >= 4)
            {
                // 기존 Material 슬롯 가져오기
                Material[] materials =
                    backRenderer.materials;


                // 뒤 프로펠러 컬러별 가중치
                float[] backColorWeights =
                {
                    backColor0Weight,
                    backColor1Weight,
                    backColor2Weight,
                    backColor3Weight
                };


                // 컬러 하나 선택
                selectedBackColor =
                    GetWeightedRandomIndex(backColorWeights);



                // =================================================
                // 뒤 프로펠러 종류에 따라
                // 실제 컬러 Material이 들어있는 Element 위치가 다름
                // =================================================
                //
                // 현재 네 FBX 구조 기준:
                //
                // back_3 → Element 3
                // back_4 → Element 1
                // back_5 → Element 3
                //
                // =================================================

                int materialElement = -1;


                if (selectedBackNumber == 3)
                {
                    materialElement = 3;
                }
                else if (selectedBackNumber == 4)
                {
                    materialElement = 1;
                }
                else if (selectedBackNumber == 5)
                {
                    materialElement = 3;
                }



                // 실제 해당 Element가 존재하는지 확인
                if (materialElement >= 0 &&
                    materialElement < materials.Length)
                {
                    // 해당 Element만 컬러 변경
                    materials[materialElement] =
                        propellerMaterials[selectedBackColor];


                    // Renderer에 다시 적용
                    backRenderer.materials = materials;
                }
                else
                {
                    Debug.LogWarning(
                        selectedBack.name +
                        " : Material Element " +
                        materialElement +
                        "가 존재하지 않습니다."
                    );
                }
            }
        }



        // =====================================================
        // 9. 최종 정답 여부 검사
        // =====================================================

        CheckMotorValidity();



        // =====================================================
        // 10. Console 출력
        // =====================================================

        if (showDebugLog)
        {
            PrintResult();
        }
    }



    // =========================================================
    // 가중치 랜덤 함수
    // =========================================================
    //
    // 예:
    //
    // weights =
    // [10, 1, 1]
    //
    // 이면 첫 번째 항목이
    // 나머지 항목보다 10배 높은 확률로 선택됨.
    //
    // 반드시 합계가 100일 필요 없음.
    // =========================================================

    int GetWeightedRandomIndex(float[] weights)
    {
        // 전체 가중치 합
        float totalWeight = 0f;


        // 음수는 확률로 사용할 수 없으므로
        // 0보다 큰 값만 더함
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] > 0f)
            {
                totalWeight += weights[i];
            }
        }



        // 모든 Weight가 0인 경우
        // 오류 방지를 위해 일반 랜덤 사용
        if (totalWeight <= 0f)
        {
            return Random.Range(0, weights.Length);
        }



        // 0 ~ 전체 가중치 사이에서 랜덤 숫자 선택
        float randomValue =
            Random.Range(0f, totalWeight);



        // 앞에서부터 Weight를 차감하면서
        // 어느 구간에 들어가는지 확인
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0f)
                continue;


            if (randomValue < weights[i])
            {
                return i;
            }


            randomValue -= weights[i];
        }



        // 혹시 모를 float 오차 대비
        return weights.Length - 1;
    }



    // =========================================================
    // 프로펠러 이름에서 번호 가져오기
    // =========================================================
    //
    // prof_front_3 → 3
    // prof_front_4 → 4
    // prof_front_5 → 5
    //
    // prof_back_3 → 3
    // ...
    // =========================================================

    int GetPropellerNumber(GameObject propeller)
    {
        string objectName = propeller.name;


        if (objectName.EndsWith("3"))
            return 3;


        if (objectName.EndsWith("4"))
            return 4;


        if (objectName.EndsWith("5"))
            return 5;


        // 이름 형식이 맞지 않을 경우
        return -1;
    }



    // =========================================================
    // 최종 True / False 판정
    // =========================================================

    void CheckMotorValidity()
    {
        // 우선 정답이라고 가정
        IsValidMotor = true;



        // -----------------------------------------------------
        // 조건 1
        // 앞 또는 뒤 프로펠러 중
        // 하나라도 존재하지 않으면 False
        // -----------------------------------------------------

        if (!hasFront || !hasBack)
        {
            IsValidMotor = false;
        }



        // -----------------------------------------------------
        // 조건 2
        // 앞뒤 프로펠러 숫자가 다르면 False
        //
        // 3 + 3 = OK
        // 4 + 4 = OK
        // 5 + 5 = OK
        //
        // 3 + 4 등은 틀림
        // -----------------------------------------------------

        if (selectedFrontNumber != selectedBackNumber)
        {
            IsValidMotor = false;
        }



        // -----------------------------------------------------
        // 조건 3
        // 앞뒤 프로펠러 컬러가 다르면 False
        // -----------------------------------------------------

        if (selectedFrontColor != selectedBackColor)
        {
            IsValidMotor = false;
        }



        // -----------------------------------------------------
        // 조건 4
        // 본체가 Motor 0가 아니면 False
        //
        // Motor Materials 배열의
        // Element 0이 Motor 0라고 가정
        // -----------------------------------------------------

        if (selectedMotorMaterial != 0)
        {
            IsValidMotor = false;
        }
    }



    // =========================================================
    // Console 테스트 출력
    // =========================================================

    void PrintResult()
    {
        Debug.Log(
            "================ MOTOR RESULT ================" +

            "\n본체 Material : Motor " +
            selectedMotorMaterial +

            "\n앞 프로펠러 있음 : " +
            hasFront +

            "\n앞 프로펠러 번호 : " +
            selectedFrontNumber +

            "\n앞 프로펠러 컬러 : " +
            selectedFrontColor +

            "\n뒤 프로펠러 있음 : " +
            hasBack +

            "\n뒤 프로펠러 번호 : " +
            selectedBackNumber +

            "\n뒤 프로펠러 컬러 : " +
            selectedBackColor +

            "\n" +

            "\n최종 결과 : " +
            IsValidMotor +

            "\n=============================================="
        );
    }



    // =========================================================
    // 다른 코드에서 최종 결과 가져가는 함수
    // =========================================================
    //
    // 사용 예:
    //
    // bool result = randomMotor.GetMotorResult();
    //
    // 또는 그냥:
    //
    // bool result = randomMotor.IsValidMotor;
    //
    // =========================================================

    public bool GetMotorResult()
    {
        return IsValidMotor;
    }
}