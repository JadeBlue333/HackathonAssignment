using UnityEngine;

public class RandomMotor : MonoBehaviour
{
    // =========================================================
    // 확률 설정
    // =========================================================
    //
    // Spawn Chance = 실제 퍼센트(%)
    // Weight = 상대적 등장 가중치
    // =========================================================


    // =========================================================
    // Propeller Spawn Chance
    // =========================================================

    [Header("Probability - Spawn")]

    [Range(0f, 100f)]
    [Tooltip("앞 프로펠러가 생성될 확률 (%)")]
    public float frontSpawnChance = 80f;

    [Range(0f, 100f)]
    [Tooltip("뒤 프로펠러가 생성될 확률 (%)")]
    public float backSpawnChance = 80f;


    // =========================================================
    // Motor Material Weight
    // =========================================================

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


    // =========================================================
    // Front Propeller Type Weight
    // =========================================================

    [Header("Probability - Front Propeller Type")]

    [Tooltip("Front 3의 등장 가중치")]
    public float front3Weight = 1f;

    [Tooltip("Front 4의 등장 가중치")]
    public float front4Weight = 1f;

    [Tooltip("Front 5의 등장 가중치")]
    public float front5Weight = 1f;


    // =========================================================
    // Back Propeller Type Weight
    // =========================================================

    [Header("Probability - Back Propeller Type")]

    [Tooltip("Back 3의 등장 가중치")]
    public float back3Weight = 1f;

    [Tooltip("Back 4의 등장 가중치")]
    public float back4Weight = 1f;

    [Tooltip("Back 5의 등장 가중치")]
    public float back5Weight = 1f;


    // =========================================================
    // Front Propeller Color Weight
    // =========================================================

    [Header("Probability - Front Propeller Color")]

    public float frontColor0Weight = 1f;
    public float frontColor1Weight = 1f;
    public float frontColor2Weight = 1f;
    public float frontColor3Weight = 1f;


    // =========================================================
    // Back Propeller Color Weight
    // =========================================================

    [Header("Probability - Back Propeller Color")]

    public float backColor0Weight = 1f;
    public float backColor1Weight = 1f;
    public float backColor2Weight = 1f;
    public float backColor3Weight = 1f;





    // =========================================================
    // Debug
    // =========================================================

    [Header("Debug")]

    [Tooltip("모터가 생성될 때 결과를 Console에 출력할지 여부")]
    public bool showDebugLog = true;


    // =========================================================
    // Main Body
    // =========================================================

    [Header("Main Body")]

    // 항상 존재하는 모터 본체
    public GameObject motorMain;

    // Element 순서
    //
    // 0 = Motor 0
    // 1 = Motor 1
    // 2 = Motor 2
    // 3 = Motor 3
    // 4 = Motor 4
    public Material[] motorMaterials;


    // =========================================================
    // Front Propeller
    // =========================================================

    [Header("Front Propeller")]

    // Element 순서
    //
    // 0 = prof_front_3
    // 1 = prof_front_4
    // 2 = prof_front_5
    public GameObject[] frontPropellers;


    // =========================================================
    // Back Propeller
    // =========================================================

    [Header("Back Propeller")]

    // Element 순서
    //
    // 0 = prof_back_3
    // 1 = prof_back_4
    // 2 = prof_back_5
    public GameObject[] backPropellers;


    // =========================================================
    // Propeller Materials
    // =========================================================

    [Header("Propeller Materials")]

    // Element 순서
    //
    // 0 = Color 0
    // 1 = Color 1
    // 2 = Color 2
    // 3 = Color 3
    public Material[] propellerMaterials;


    // =========================================================
    // 최종 정상 / 불량 판정
    // =========================================================

    // true  = A 조건을 만족하는 정상 모터
    // false = 하나 이상의 문제가 있는 모터
    public bool IsValidMotor
    {
        get;
        private set;
    }


    // =========================================================
    // 현재 생성된 모터 정보
    // =========================================================

    // 앞 프로펠러 존재 여부
    private bool hasFront;

    // 뒤 프로펠러 존재 여부
    private bool hasBack;


    // 현재 선택된 앞 프로펠러 번호
    //
    // 3 / 4 / 5
    // 없으면 -1
    private int selectedFrontNumber = -1;


    // 현재 선택된 뒤 프로펠러 번호
    //
    // 3 / 4 / 5
    // 없으면 -1
    private int selectedBackNumber = -1;


    // 현재 선택된 앞 프로펠러 컬러
    //
    // 0 ~ 3
    // 없으면 -1
    private int selectedFrontColor = -1;


    // 현재 선택된 뒤 프로펠러 컬러
    //
    // 0 ~ 3
    // 없으면 -1
    private int selectedBackColor = -1;


    // 현재 선택된 Motor Material
    //
    // 0 ~ 4
    private int selectedMotorMaterial = -1;


    // =========================================================
    // 외부 코드에서 읽을 수 있는 현재 모터 정보
    // =========================================================
    //
    // MotorMatchManager에서
    // A / B / C / 폐기 판정에 사용
    //
    // 외부에서는 읽기만 가능하고
    // 직접 값을 변경할 수 없음.
    // =========================================================

    public bool HasFront =>
        hasFront;

    public bool HasBack =>
        hasBack;


    public int SelectedFrontNumber =>
        selectedFrontNumber;

    public int SelectedBackNumber =>
        selectedBackNumber;


    public int SelectedFrontColor =>
        selectedFrontColor;

    public int SelectedBackColor =>
        selectedBackColor;


    public int SelectedMotorMaterial =>
        selectedMotorMaterial;




    // =========================================================
    // 기존 코드 호환용
    // =========================================================
    //
    // 인자 없이 호출하면 일반 랜덤 생성
    // =========================================================

    public void GenerateMotor()
    {
        GenerateMotor(false);
    }

    // =========================================================
    // 지정 조건으로 모터 생성
    // =========================================================
    //
    // spawnFront
    // true  = 앞 프로펠러 생성
    // false = 앞 프로펠러 없음
    //
    // spawnBack
    // true  = 뒤 프로펠러 생성
    // false = 뒤 프로펠러 없음
    //
    // useStainTexture
    // false = Motor Material 0
    // true  = Motor Material 1~4 중 랜덤
    //
    // sameNumber
    // true  = 앞뒤 날개 개수 동일
    // false = 앞뒤 날개 개수 다름
    //
    // sameColor
    // true  = 앞뒤 색상 동일
    // false = 앞뒤 색상 다름
    //
    // =========================================================

    public void GenerateMotorByCondition(
        bool spawnFront,
        bool spawnBack,
        bool useStainTexture,
        bool sameNumber,
        bool sameColor
    )
    {
        // =====================================================
        // 초기화
        // =====================================================

        if (motorMain != null)
        {
            motorMain.SetActive(true);
        }


        foreach (GameObject prop in frontPropellers)
        {
            if (prop != null)
            {
                prop.SetActive(false);
            }
        }


        foreach (GameObject prop in backPropellers)
        {
            if (prop != null)
            {
                prop.SetActive(false);
            }
        }


        hasFront = false;
        hasBack = false;

        selectedFrontNumber = -1;
        selectedBackNumber = -1;

        selectedFrontColor = -1;
        selectedBackColor = -1;

        selectedMotorMaterial = -1;


        // =====================================================
        // 1. Motor Texture
        // =====================================================

        if (useStainTexture)
        {
            // Material 1~4 중 랜덤
            float[] stainWeights =
            {
            motor1Weight,
            motor2Weight,
            motor3Weight,
            motor4Weight
        };

            selectedMotorMaterial =
                GetWeightedRandomIndex(
                    stainWeights
                ) + 1;
        }
        else
        {
            // 정상 Motor
            selectedMotorMaterial = 0;
        }


        ApplyMotorMaterial(
            selectedMotorMaterial
        );


        // =====================================================
        // 프로펠러 존재 여부 저장
        // =====================================================

        hasFront = spawnFront;
        hasBack = spawnBack;


        // =====================================================
        // 둘 다 존재하는 경우
        // =====================================================

        if (hasFront && hasBack)
        {
            // =============================================
            // 앞 프로펠러 선택
            // =============================================

            float[] frontTypeWeights =
            {
            front3Weight,
            front4Weight,
            front5Weight
        };

            int frontIndex =
                GetWeightedRandomIndex(
                    frontTypeWeights
                );


            GameObject front =
                frontPropellers[
                    frontIndex
                ];

            front.SetActive(true);

            selectedFrontNumber =
                GetPropellerNumber(
                    front
                );


            // =============================================
            // 뒤 프로펠러 선택
            // =============================================

            int backIndex;


            if (sameNumber)
            {
                // 앞과 같은 날개 개수
                backIndex =
                    FindPropellerIndexByNumber(
                        backPropellers,
                        selectedFrontNumber
                    );
            }
            else
            {
                // 앞과 다른 날개 개수
                backIndex =
                    GetDifferentPropellerIndex(
                        backPropellers,
                        selectedFrontNumber
                    );
            }


            GameObject back =
                backPropellers[
                    backIndex
                ];

            back.SetActive(true);

            selectedBackNumber =
                GetPropellerNumber(
                    back
                );


            // =============================================
            // 앞 색상
            // =============================================

            float[] frontColorWeights =
            {
            frontColor0Weight,
            frontColor1Weight,
            frontColor2Weight,
            frontColor3Weight
        };

            selectedFrontColor =
                GetWeightedRandomIndex(
                    frontColorWeights
                );


            // =============================================
            // 뒤 색상
            // =============================================

            if (sameColor)
            {
                selectedBackColor =
                    selectedFrontColor;
            }
            else
            {
                selectedBackColor =
                    GetDifferentColorIndex(
                        selectedFrontColor
                    );
            }


            ApplyFrontColor(
                frontIndex,
                selectedFrontColor
            );

            ApplyBackColor(
                backIndex,
                selectedBackColor
            );
        }


        // =====================================================
        // 앞만 존재
        // =====================================================

        else if (hasFront)
        {
            float[] frontTypeWeights =
            {
            front3Weight,
            front4Weight,
            front5Weight
        };

            int frontIndex =
                GetWeightedRandomIndex(
                    frontTypeWeights
                );

            GameObject front =
                frontPropellers[
                    frontIndex
                ];

            front.SetActive(true);

            selectedFrontNumber =
                GetPropellerNumber(
                    front
                );


            float[] colorWeights =
            {
            frontColor0Weight,
            frontColor1Weight,
            frontColor2Weight,
            frontColor3Weight
        };

            selectedFrontColor =
                GetWeightedRandomIndex(
                    colorWeights
                );

            ApplyFrontColor(
                frontIndex,
                selectedFrontColor
            );
        }


        // =====================================================
        // 뒤만 존재
        // =====================================================

        else if (hasBack)
        {
            float[] backTypeWeights =
            {
            back3Weight,
            back4Weight,
            back5Weight
        };

            int backIndex =
                GetWeightedRandomIndex(
                    backTypeWeights
                );

            GameObject back =
                backPropellers[
                    backIndex
                ];

            back.SetActive(true);

            selectedBackNumber =
                GetPropellerNumber(
                    back
                );


            float[] colorWeights =
            {
            backColor0Weight,
            backColor1Weight,
            backColor2Weight,
            backColor3Weight
        };

            selectedBackColor =
                GetWeightedRandomIndex(
                    colorWeights
                );

            ApplyBackColor(
                backIndex,
                selectedBackColor
            );
        }


        // =====================================================
        // 정상 여부 계산
        // =====================================================

        CheckMotorValidity();


        if (showDebugLog)
        {
            Debug.Log(
                "조건 지정 Motor 생성 완료"
            );
        }
    }

    // =========================================================
    // Motor 생성
    // =========================================================
    //
    // forceValid == true
    //
    // → A 조건을 만족하는 정상 모터만 생성
    //
    //
    // forceValid == false
    //
    // → 기존 확률을 사용하여
    //   모든 조합 랜덤 생성
    //
    // =========================================================

    public void GenerateMotor(
        bool forceValid
    )
    {
        // =====================================================
        // 초기화
        // =====================================================

        if (motorMain != null)
        {
            motorMain.SetActive(
                true
            );
        }


        // 모든 앞 프로펠러 끄기
        foreach (
            GameObject prop in frontPropellers
        )
        {
            if (prop != null)
            {
                prop.SetActive(
                    false
                );
            }
        }


        // 모든 뒤 프로펠러 끄기
        foreach (
            GameObject prop in backPropellers
        )
        {
            if (prop != null)
            {
                prop.SetActive(
                    false
                );
            }
        }


        // =====================================================
        // 현재 정보 초기화
        // =====================================================

        hasFront =
            false;

        hasBack =
            false;


        selectedFrontNumber =
            -1;

        selectedBackNumber =
            -1;


        selectedFrontColor =
            -1;

        selectedBackColor =
            -1;


        selectedMotorMaterial =
            -1;


        // =====================================================
        // 생성
        // =====================================================

        if (forceValid)
        {
            // 정상 박스일 때
            // 무조건 A 조건의 모터 생성
            GenerateValidMotor();
        }
        else
        {
            // 비정상 박스일 때
            // 모든 경우 랜덤 생성 가능
            GenerateRandomMotor();
        }


        // =====================================================
        // 최종 정상 여부 확인
        // =====================================================

        CheckMotorValidity();


        // =====================================================
        // Debug
        // =====================================================

        if (showDebugLog)
        {
            PrintResult(
                forceValid
            );
        }
    }


    // =========================================================
    // 정상 모터 강제 생성
    // =========================================================
    //
    // A 조건
    //
    // Motor Material = 0
    //
    // 앞 프로펠러 있음
    // 뒤 프로펠러 있음
    //
    // 앞뒤 프로펠러 번호 동일
    //
    // 앞뒤 프로펠러 컬러 동일
    //
    // =========================================================

    private void GenerateValidMotor()
    {
        // =====================================================
        // 1. Motor Material은 무조건 0
        // =====================================================

        selectedMotorMaterial =
            0;


        ApplyMotorMaterial(
            selectedMotorMaterial
        );


        // =====================================================
        // 2. 프로펠러 앞뒤 모두 존재
        // =====================================================

        hasFront =
            true;

        hasBack =
            true;


        // =====================================================
        // 3. 앞 프로펠러 종류 랜덤 선택
        // =====================================================

        float[] frontTypeWeights =
        {
            front3Weight,
            front4Weight,
            front5Weight
        };


        int frontIndex =
            GetWeightedRandomIndex(
                frontTypeWeights
            );


        if (
            frontIndex < 0 ||
            frontIndex >= frontPropellers.Length
        )
        {
            Debug.LogError(
                "Front Propeller Index가 잘못되었습니다."
            );

            hasFront =
                false;

            hasBack =
                false;

            return;
        }


        GameObject selectedFront =
            frontPropellers[
                frontIndex
            ];


        if (selectedFront == null)
        {
            Debug.LogError(
                "Front Propeller가 연결되어 있지 않습니다."
            );

            hasFront =
                false;

            hasBack =
                false;

            return;
        }


        selectedFront.SetActive(
            true
        );


        selectedFrontNumber =
            GetPropellerNumber(
                selectedFront
            );


        // =====================================================
        // 4. 같은 번호의 뒤 프로펠러 찾기
        // =====================================================

        int backIndex =
            FindPropellerIndexByNumber(
                backPropellers,
                selectedFrontNumber
            );


        if (backIndex < 0)
        {
            Debug.LogError(
                "앞 프로펠러 번호와 같은 " +
                "뒤 프로펠러를 찾을 수 없습니다."
            );


            hasBack =
                false;

            return;
        }


        GameObject selectedBack =
            backPropellers[
                backIndex
            ];


        if (selectedBack == null)
        {
            Debug.LogError(
                "Back Propeller가 연결되어 있지 않습니다."
            );


            hasBack =
                false;

            return;
        }


        selectedBack.SetActive(
            true
        );


        selectedBackNumber =
            GetPropellerNumber(
                selectedBack
            );


        // =====================================================
        // 5. 프로펠러 컬러 하나 선택
        // =====================================================

        float[] frontColorWeights =
        {
            frontColor0Weight,
            frontColor1Weight,
            frontColor2Weight,
            frontColor3Weight
        };


        int colorIndex =
            GetWeightedRandomIndex(
                frontColorWeights
            );


        selectedFrontColor =
            colorIndex;


        selectedBackColor =
            colorIndex;


        // =====================================================
        // 6. 같은 컬러 적용
        // =====================================================

        ApplyFrontColor(
            frontIndex,
            colorIndex
        );


        ApplyBackColor(
            backIndex,
            colorIndex
        );
    }


    // =========================================================
    // 일반 랜덤 모터 생성
    // =========================================================

    private void GenerateRandomMotor()
    {
        // =====================================================
        // 1. Motor Material 랜덤 선택
        // =====================================================

        float[] motorWeights =
        {
            motor0Weight,
            motor1Weight,
            motor2Weight,
            motor3Weight,
            motor4Weight
        };


        selectedMotorMaterial =
            GetWeightedRandomIndex(
                motorWeights
            );


        ApplyMotorMaterial(
            selectedMotorMaterial
        );


        // =====================================================
        // 2. 앞 프로펠러 생성 여부 결정
        // =====================================================

        hasFront =
            Random.Range(
                0f,
                100f
            ) <
            frontSpawnChance;


        // =====================================================
        // 3. 앞 프로펠러 종류 + 컬러 결정
        // =====================================================

        if (
            hasFront &&
            frontPropellers.Length >= 3
        )
        {
            float[] frontTypeWeights =
            {
                front3Weight,
                front4Weight,
                front5Weight
            };


            int frontIndex =
                GetWeightedRandomIndex(
                    frontTypeWeights
                );


            GameObject selectedFront =
                frontPropellers[
                    frontIndex
                ];


            if (selectedFront != null)
            {
                selectedFront.SetActive(
                    true
                );


                selectedFrontNumber =
                    GetPropellerNumber(
                        selectedFront
                    );


                float[] frontColorWeights =
                {
                    frontColor0Weight,
                    frontColor1Weight,
                    frontColor2Weight,
                    frontColor3Weight
                };


                selectedFrontColor =
                    GetWeightedRandomIndex(
                        frontColorWeights
                    );


                ApplyFrontColor(
                    frontIndex,
                    selectedFrontColor
                );
            }
            else
            {
                hasFront =
                    false;
            }
        }


        // =====================================================
        // 4. 뒤 프로펠러 생성 여부 결정
        // =====================================================

        hasBack =
            Random.Range(
                0f,
                100f
            ) <
            backSpawnChance;


        // =====================================================
        // 5. 뒤 프로펠러 종류 + 컬러 결정
        // =====================================================

        if (
            hasBack &&
            backPropellers.Length >= 3
        )
        {
            float[] backTypeWeights =
            {
                back3Weight,
                back4Weight,
                back5Weight
            };


            int backIndex =
                GetWeightedRandomIndex(
                    backTypeWeights
                );


            GameObject selectedBack =
                backPropellers[
                    backIndex
                ];


            if (selectedBack != null)
            {
                selectedBack.SetActive(
                    true
                );


                selectedBackNumber =
                    GetPropellerNumber(
                        selectedBack
                    );


                float[] backColorWeights =
                {
                    backColor0Weight,
                    backColor1Weight,
                    backColor2Weight,
                    backColor3Weight
                };


                selectedBackColor =
                    GetWeightedRandomIndex(
                        backColorWeights
                    );


                ApplyBackColor(
                    backIndex,
                    selectedBackColor
                );
            }
            else
            {
                hasBack =
                    false;
            }
        }
    }




    // =========================================================
    // Motor Material 적용
    // =========================================================

    private void ApplyMotorMaterial(
        int materialIndex
    )
    {
        if (motorMain == null)
        {
            Debug.LogError(
                "motorMain이 연결되어 있지 않습니다."
            );

            return;
        }


        Renderer motorRenderer =
            motorMain.GetComponentInChildren<Renderer>();


        if (motorRenderer == null)
        {
            Debug.LogError(
                "Motor Renderer를 찾을 수 없습니다."
            );

            return;
        }


        if (
            motorMaterials == null ||
            materialIndex < 0 ||
            materialIndex >= motorMaterials.Length
        )
        {
            Debug.LogError(
                "Motor Material Index를 확인해주세요."
            );

            return;
        }


        motorRenderer.material =
            motorMaterials[
                materialIndex
            ];
    }


    // =========================================================
    // 앞 프로펠러 컬러 적용
    // =========================================================

    private void ApplyFrontColor(
        int propellerIndex,
        int colorIndex
    )
    {
        if (
            propellerIndex < 0 ||
            propellerIndex >= frontPropellers.Length
        )
        {
            return;
        }


        if (
            colorIndex < 0 ||
            colorIndex >= propellerMaterials.Length
        )
        {
            return;
        }


        GameObject propeller =
            frontPropellers[
                propellerIndex
            ];


        if (propeller == null)
            return;


        Renderer frontRenderer =
            propeller.GetComponentInChildren<Renderer>();


        if (frontRenderer == null)
            return;


        Material[] materials =
            frontRenderer.materials;


        // 앞 프로펠러는
        // Material Element 0 사용
        if (materials.Length > 0)
        {
            materials[0] =
                propellerMaterials[
                    colorIndex
                ];


            frontRenderer.materials =
                materials;
        }
    }


    // =========================================================
    // 뒤 프로펠러 컬러 적용
    // =========================================================

    private void ApplyBackColor(
        int propellerIndex,
        int colorIndex
    )
    {
        if (
            propellerIndex < 0 ||
            propellerIndex >= backPropellers.Length
        )
        {
            return;
        }


        if (
            colorIndex < 0 ||
            colorIndex >= propellerMaterials.Length
        )
        {
            return;
        }


        GameObject propeller =
            backPropellers[
                propellerIndex
            ];


        if (propeller == null)
            return;


        Renderer backRenderer =
            propeller.GetComponentInChildren<Renderer>();


        if (backRenderer == null)
            return;


        Material[] materials =
            backRenderer.materials;


        int propellerNumber =
            GetPropellerNumber(
                propeller
            );


        // =====================================================
        // 현재 FBX Material 구조
        //
        // back_3 → Element 3
        // back_4 → Element 1
        // back_5 → Element 3
        // =====================================================

        int materialElement =
            -1;


        if (propellerNumber == 3)
        {
            materialElement =
                3;
        }
        else if (propellerNumber == 4)
        {
            materialElement =
                1;
        }
        else if (propellerNumber == 5)
        {
            materialElement =
                3;
        }


        if (
            materialElement >= 0 &&
            materialElement < materials.Length
        )
        {
            materials[
                materialElement
            ] =
                propellerMaterials[
                    colorIndex
                ];


            backRenderer.materials =
                materials;
        }
        else
        {
            Debug.LogWarning(
                propeller.name +
                " : Material Element " +
                materialElement +
                "가 존재하지 않습니다."
            );
        }
    }


    // =========================================================
    // 같은 번호의 프로펠러 Index 찾기
    // =========================================================

    private int FindPropellerIndexByNumber(
        GameObject[] propellers,
        int targetNumber
    )
    {
        for (
            int i = 0;
            i < propellers.Length;
            i++
        )
        {
            if (
                propellers[i] != null &&
                GetPropellerNumber(
                    propellers[i]
                ) == targetNumber
            )
            {
                return i;
            }
        }


        return -1;
    }

    // =========================================================
    // 지정한 번호와 다른 프로펠러 선택
    // =========================================================

    private int GetDifferentPropellerIndex(
        GameObject[] propellers,
        int excludedNumber
    )
    {
        int[] candidates =
            new int[propellers.Length];

        int candidateCount = 0;


        for (int i = 0; i < propellers.Length; i++)
        {
            if (
                propellers[i] != null &&
                GetPropellerNumber(propellers[i]) != excludedNumber
            )
            {
                candidates[candidateCount] = i;
                candidateCount++;
            }
        }


        if (candidateCount == 0)
        {
            Debug.LogError(
                "다른 번호의 프로펠러를 찾을 수 없습니다."
            );

            return 0;
        }


        return candidates[
            Random.Range(
                0,
                candidateCount
            )
        ];
    }


    // =========================================================
    // 지정한 색상과 다른 색상 선택
    // =========================================================

    private int GetDifferentColorIndex(
        int excludedColor
    )
    {
        if (
            propellerMaterials == null ||
            propellerMaterials.Length <= 1
        )
        {
            Debug.LogError(
                "서로 다른 프로펠러 색상을 선택할 수 없습니다."
            );

            return 0;
        }


        int randomColor;


        do
        {
            randomColor =
                Random.Range(
                    0,
                    propellerMaterials.Length
                );
        }
        while (
            randomColor == excludedColor
        );


        return randomColor;
    }

    // =========================================================
    // 가중치 랜덤
    // =========================================================

    private int GetWeightedRandomIndex(
        float[] weights
    )
    {
        float totalWeight =
            0f;


        // 전체 가중치 계산
        for (
            int i = 0;
            i < weights.Length;
            i++
        )
        {
            if (weights[i] > 0f)
            {
                totalWeight +=
                    weights[i];
            }
        }


        // 모든 가중치가 0이면
        // 일반 랜덤
        if (totalWeight <= 0f)
        {
            return Random.Range(
                0,
                weights.Length
            );
        }


        float randomValue =
            Random.Range(
                0f,
                totalWeight
            );


        for (
            int i = 0;
            i < weights.Length;
            i++
        )
        {
            if (weights[i] <= 0f)
                continue;


            if (randomValue < weights[i])
            {
                return i;
            }


            randomValue -=
                weights[i];
        }


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
    // prof_back_4 → 4
    // prof_back_5 → 5
    //
    // =========================================================

    private int GetPropellerNumber(
        GameObject propeller
    )
    {
        if (propeller == null)
            return -1;


        string objectName =
            propeller.name;


        if (objectName.EndsWith("3"))
        {
            return 3;
        }


        if (objectName.EndsWith("4"))
        {
            return 4;
        }


        if (objectName.EndsWith("5"))
        {
            return 5;
        }


        return -1;
    }


    // =========================================================
    // 최종 정상 / 불량 판정
    // =========================================================
    //
    // 이 bool은 기존 코드 호환용으로 유지
    //
    // true:
    // A 조건
    //
    // false:
    // B / C / 폐기 중 하나
    //
    // 실제 A/B/C/폐기 세부 구분은
    // MotorMatchManager가 아래 공개 값들을 이용해 판단
    //
    // HasFront
    // HasBack
    // SelectedMotorMaterial
    // SelectedFrontNumber
    // SelectedBackNumber
    // SelectedFrontColor
    // SelectedBackColor
    //
    // =========================================================

    private void CheckMotorValidity()
    {
        IsValidMotor =
            true;


        // 프로펠러 하나라도 없음
        if (
            !hasFront ||
            !hasBack
        )
        {
            IsValidMotor =
                false;
        }


        // 프로펠러 번호 다름
        if (
            selectedFrontNumber !=
            selectedBackNumber
        )
        {
            IsValidMotor =
                false;
        }


        // 프로펠러 컬러 다름
        if (
            selectedFrontColor !=
            selectedBackColor
        )
        {
            IsValidMotor =
                false;
        }


        // Motor Material이 0번이 아님
        if (
            selectedMotorMaterial !=
            0
        )
        {
            IsValidMotor =
                false;
        }
    }


    // =========================================================
    // 기존 코드 호환용 결과 함수
    // =========================================================

    public bool GetMotorResult()
    {
        return IsValidMotor;
    }


    // =========================================================
    // Console 출력
    // =========================================================

    private void PrintResult(
        bool forceValid
    )
    {
        Debug.Log(
            "================ MOTOR RESULT ================" +

            "\n생성 모드 : " +
            (
                forceValid
                    ? "정상 모터 강제 생성"
                    : "일반 랜덤 생성"
            ) +

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

            "\n최종 정상 여부 : " +
            IsValidMotor +

            "\n=============================================="
        );
    }
}