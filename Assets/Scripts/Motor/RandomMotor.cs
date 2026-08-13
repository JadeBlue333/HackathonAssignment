using UnityEngine;
using UnityEngine.InputSystem;

public class RandomMotor : MonoBehaviour
{
    [Header("Main Body")]
    public GameObject motorMain;
    public Material[] motorMaterials; // Motor 0 ~ Motor 4


    [Header("Front Propeller")]
    public GameObject[] frontPropellers; // prof_front_3 ~ 5


    [Header("Back Propeller")]
    public GameObject[] backPropellers; // prof_back_3 ~ 5


    [Header("Propeller Materials")]
    public Material[] propellerMaterials; // PropellerFront 0 ~ 3


    // =====================================================
    // 다른 코드에서 받아갈 최종 결과값
    // =====================================================

    public bool IsValidMotor { get; private set; }


    // =====================================================
    // 현재 랜덤으로 선택된 값
    // =====================================================

    private bool hasFront;
    private bool hasBack;

    private int selectedFrontNumber = -1;
    private int selectedBackNumber = -1;

    private int selectedFrontColor = -1;
    private int selectedBackColor = -1;

    private int selectedMotorMaterial = -1;


    void Start()
    {
        GenerateMotor();
    }


    void Update()
    {
        // R키를 누르면 다시 랜덤 생성
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            GenerateMotor();
        }
    }


    public void GenerateMotor()
    {
        // =====================================================
        // 1. 본체 켜기
        // =====================================================

        motorMain.SetActive(true);


        // =====================================================
        // 2. 본체 Material 랜덤
        // =====================================================

        Renderer motorRenderer =
            motorMain.GetComponentInChildren<Renderer>();

        if (motorRenderer != null && motorMaterials.Length > 0)
        {
            selectedMotorMaterial =
                Random.Range(0, motorMaterials.Length);

            motorRenderer.material =
                motorMaterials[selectedMotorMaterial];
        }


        // =====================================================
        // 앞 프로펠러 초기화
        // =====================================================

        foreach (GameObject prop in frontPropellers)
        {
            prop.SetActive(false);
        }

        hasFront = false;
        selectedFrontNumber = -1;
        selectedFrontColor = -1;


        // =====================================================
        // 3. 앞 프로펠러 생성 여부
        // =====================================================

        hasFront = Random.value < 0.5f;


        // =====================================================
        // 4~5. 앞 프로펠러 종류 + 컬러
        // =====================================================

        if (hasFront && frontPropellers.Length > 0)
        {
            int randomFront =
                Random.Range(0, frontPropellers.Length);

            GameObject selectedFront =
                frontPropellers[randomFront];

            selectedFront.SetActive(true);


            // 이름 뒤 숫자 가져오기
            // prof_front_3 → 3
            selectedFrontNumber =
                GetPropellerNumber(selectedFront);


            Renderer frontRenderer =
                selectedFront.GetComponentInChildren<Renderer>();


            if (frontRenderer != null &&
                propellerMaterials.Length > 0)
            {
                Material[] materials =
                    frontRenderer.materials;


                selectedFrontColor =
                    Random.Range(0, propellerMaterials.Length);


                // 앞 프로펠러는 Element 0 변경
                if (materials.Length > 0)
                {
                    materials[0] =
                        propellerMaterials[selectedFrontColor];

                    frontRenderer.materials = materials;
                }
            }
        }


        // =====================================================
        // 뒤 프로펠러 초기화
        // =====================================================

        foreach (GameObject prop in backPropellers)
        {
            prop.SetActive(false);
        }

        hasBack = false;
        selectedBackNumber = -1;
        selectedBackColor = -1;


        // =====================================================
        // 6. 뒤 프로펠러 생성 여부
        // =====================================================

        hasBack = Random.value < 0.5f;


        // =====================================================
        // 7~8. 뒤 프로펠러 종류 + 컬러
        // =====================================================

        if (hasBack && backPropellers.Length > 0)
        {
            int randomBack =
                Random.Range(0, backPropellers.Length);

            GameObject selectedBack =
                backPropellers[randomBack];

            selectedBack.SetActive(true);


            // 이름 뒤 숫자 가져오기
            // prof_back_3 → 3
            selectedBackNumber =
                GetPropellerNumber(selectedBack);


            Renderer backRenderer =
                selectedBack.GetComponentInChildren<Renderer>();


            if (backRenderer != null &&
                propellerMaterials.Length > 0)
            {
                Material[] materials =
                    backRenderer.materials;


                selectedBackColor =
                    Random.Range(0, propellerMaterials.Length);


                // =============================================
                // Back 종류별 변경해야 하는 Material Element
                //
                // back_3 → Element 2
                // back_4 → Element 3
                // back_5 → Element 3
                // =============================================

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


                // 해당 Element가 실제로 존재할 경우
                if (materialElement >= 0 &&
                    materialElement < materials.Length)
                {
                    materials[materialElement] =
                        propellerMaterials[selectedBackColor];

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
        // 최종 판정
        // =====================================================

        CheckMotorValidity();


        // =====================================================
        // Console 확인
        // =====================================================

        //PrintResult();
    }


    // =========================================================
    // 오브젝트 이름에서 3 / 4 / 5 가져오기
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

        return -1;
    }


    // =========================================================
    // 최종 True / False 판정
    // =========================================================

    void CheckMotorValidity()
    {
        // 일단 True로 시작
        IsValidMotor = true;


        // -----------------------------------------------------
        // 1. 프로펠러가 하나라도 없으면 False
        // -----------------------------------------------------

        if (!hasFront || !hasBack)
        {
            IsValidMotor = false;
        }


        // -----------------------------------------------------
        // 2. 앞뒤 프로펠러 숫자가 다르면 False
        //
        // front_3 + back_3 = OK
        // front_4 + back_4 = OK
        // front_5 + back_5 = OK
        //
        // 그 외 = False
        // -----------------------------------------------------

        if (selectedFrontNumber != selectedBackNumber)
        {
            IsValidMotor = false;
        }


        // -----------------------------------------------------
        // 3. 앞뒤 프로펠러 컬러가 다르면 False
        // -----------------------------------------------------

        if (selectedFrontColor != selectedBackColor)
        {
            IsValidMotor = false;
        }


        // -----------------------------------------------------
        // 4. 본체가 Motor 0가 아니면 False
        //
        // Inspector의 Motor Materials
        // Element 0 = Motor 0 이라고 가정
        // -----------------------------------------------------

        if (selectedMotorMaterial != 0)
        {
            IsValidMotor = false;
        }
    }


    // =========================================================
    // Console 테스트
    // =========================================================

    //void PrintResult()
    //{
    //    Debug.Log(
    //        "================ MOTOR RESULT ================" +
    //        "\n본체 Material : Motor " + selectedMotorMaterial +
    //        "\n앞 프로펠러 있음 : " + hasFront +
    //        "\n앞 프로펠러 번호 : " + selectedFrontNumber +
    //        "\n앞 프로펠러 컬러 : " + selectedFrontColor +
    //        "\n뒤 프로펠러 있음 : " + hasBack +
    //        "\n뒤 프로펠러 번호 : " + selectedBackNumber +
    //        "\n뒤 프로펠러 컬러 : " + selectedBackColor +
    //        "\n" +
    //        "\n최종 결과 : " + IsValidMotor +
    //        "\n=============================================="
    //    );
    //}


    // =========================================================
    // 다른 코드에서 결과를 받아갈 때 사용할 함수
    // =========================================================

    public bool GetMotorResult()
    {
        return IsValidMotor;
    }
}