using UnityEngine;

public class RandomMotor : MonoBehaviour
{
    // =========================================================
    // 기존 랜덤 생성용 확률
    //
    // GenerateMotor(false)를 사용할 때만 사용합니다.
    // GenerateMotorByCondition()에서는 사용하지 않습니다.
    // =========================================================

    [Header("Legacy Random - Spawn")]

    [Range(0f, 100f)]
    public float frontSpawnChance = 80f;

    [Range(0f, 100f)]
    public float backSpawnChance = 80f;


    // =========================================================
    // Motor Material Weight
    // =========================================================

    [Header("Motor Material Weight")]

    public float motor0Weight = 1f;
    public float motor1Weight = 1f;
    public float motor2Weight = 1f;
    public float motor3Weight = 1f;
    public float motor4Weight = 1f;


    // =========================================================
    // Front Propeller Type Weight
    // =========================================================

    [Header("Front Propeller Type Weight")]

    public float front3Weight = 1f;
    public float front4Weight = 1f;
    public float front5Weight = 1f;


    // =========================================================
    // Back Propeller Type Weight
    // =========================================================

    [Header("Back Propeller Type Weight")]

    public float back3Weight = 1f;
    public float back4Weight = 1f;
    public float back5Weight = 1f;


    // =========================================================
    // Front Propeller Color Weight
    // =========================================================

    [Header("Front Propeller Color Weight")]

    public float frontColor0Weight = 1f;
    public float frontColor1Weight = 1f;
    public float frontColor2Weight = 1f;
    public float frontColor3Weight = 1f;


    // =========================================================
    // Back Propeller Color Weight
    // =========================================================

    [Header("Back Propeller Color Weight")]

    public float backColor0Weight = 1f;
    public float backColor1Weight = 1f;
    public float backColor2Weight = 1f;
    public float backColor3Weight = 1f;


    // =========================================================
    // Debug
    // =========================================================

    [Header("Debug")]

    [SerializeField]
    private bool showDebugLog = true;


    // =========================================================
    // Main Body
    // =========================================================

    [Header("Main Body")]

    public GameObject motorMain;

    // 0 = 정상
    // 1~4 = 얼룩
    public Material[] motorMaterials;


    // =========================================================
    // Front Propeller
    // =========================================================

    [Header("Front Propeller")]

    // Element 0 = front_3
    // Element 1 = front_4
    // Element 2 = front_5
    public GameObject[] frontPropellers;


    // =========================================================
    // Back Propeller
    // =========================================================

    [Header("Back Propeller")]

    // Element 0 = back_3
    // Element 1 = back_4
    // Element 2 = back_5
    public GameObject[] backPropellers;


    // =========================================================
    // Propeller Materials
    // =========================================================

    [Header("Propeller Materials")]

    // 0~3
    public Material[] propellerMaterials;


    // =========================================================
    // Current State
    // =========================================================

    public bool IsValidMotor
    {
        get;
        private set;
    }


    private bool hasFront;
    private bool hasBack;

    private int selectedFrontNumber = -1;
    private int selectedBackNumber = -1;

    private int selectedFrontColor = -1;
    private int selectedBackColor = -1;

    private int selectedMotorMaterial = -1;


    // =========================================================
    // 외부 공개 정보
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

    public void GenerateMotor()
    {
        GenerateMotor(false);
    }


    // =========================================================
    // InspectionGameManager 조건 기반 생성
    //
    // spawnFront
    // OFF = 앞 프로펠러 0%
    // ON  = frontChance 확률
    //
    // spawnBack
    // OFF = 뒤 프로펠러 0%
    // ON  = backChance 확률
    //
    // useMotorTextureCondition
    // OFF = 무조건 Motor 0
    // ON  = motor0Chance 확률로 Motor 0
    //
    // useSameNumberCondition
    // OFF = 무조건 다름
    // ON  = sameNumberChance 확률로 같음
    //
    // useSameColorCondition
    // OFF = 무조건 다름
    // ON  = sameColorChance 확률로 같음
    // =========================================================

    public void GenerateMotorByCondition(
        bool spawnFront,
        float frontChance,

        bool spawnBack,
        float backChance,

        bool useMotorTextureCondition,
        float motor0Chance,

        bool useSameNumberCondition,
        float sameNumberChance,

        bool useSameColorCondition,
        float sameColorChance
    )
    {
        // =====================================================
        // 확률 보정
        // =====================================================

        frontChance =
            Mathf.Clamp(
                frontChance,
                0f,
                100f
            );

        backChance =
            Mathf.Clamp(
                backChance,
                0f,
                100f
            );

        motor0Chance =
            Mathf.Clamp(
                motor0Chance,
                0f,
                100f
            );

        sameNumberChance =
            Mathf.Clamp(
                sameNumberChance,
                0f,
                100f
            );

        sameColorChance =
            Mathf.Clamp(
                sameColorChance,
                0f,
                100f
            );


        // =====================================================
        // 초기화
        // =====================================================

        ResetMotor();


        // =====================================================
        // 1. Motor Material
        //
        // 조건 OFF
        // → Motor 0
        //
        // 조건 ON
        // → motor0Chance 확률로 Motor 0
        // → 실패 시 Motor 1~4
        // =====================================================

        if (!useMotorTextureCondition)
        {
            selectedMotorMaterial =
                0;
        }
        else
        {
            bool motor0Selected =
                RollChance(
                    motor0Chance
                );


            if (motor0Selected)
            {
                selectedMotorMaterial =
                    0;
            }
            else
            {
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
        }


        ApplyMotorMaterial(
            selectedMotorMaterial
        );


        // =====================================================
        // 2. 앞 프로펠러 생성 여부
        // =====================================================

        if (spawnFront)
        {
            hasFront =
                RollChance(
                    frontChance
                );
        }
        else
        {
            hasFront =
                false;
        }


        // =====================================================
        // 3. 뒤 프로펠러 생성 여부
        // =====================================================

        if (spawnBack)
        {
            hasBack =
                RollChance(
                    backChance
                );
        }
        else
        {
            hasBack =
                false;
        }


        // =====================================================
        // 4. 앞뒤 모두 존재
        // =====================================================

        if (
            hasFront &&
            hasBack
        )
        {
            GenerateBothPropellers(
                useSameNumberCondition,
                sameNumberChance,

                useSameColorCondition,
                sameColorChance
            );
        }


        // =====================================================
        // 5. 앞만 존재
        // =====================================================

        else if (hasFront)
        {
            GenerateFrontOnly();
        }


        // =====================================================
        // 6. 뒤만 존재
        // =====================================================

        else if (hasBack)
        {
            GenerateBackOnly();
        }


        // =====================================================
        // 최종 상태 계산
        // =====================================================

        CheckMotorValidity();


        // =====================================================
        // Debug
        // =====================================================

        if (showDebugLog)
        {
            Debug.Log(
                "============= CONDITION MOTOR =============" +

                "\n앞 조건 활성 : " +
                spawnFront +

                "\n앞 생성 확률 : " +
                frontChance +

                "\n앞 실제 생성 : " +
                hasFront +

                "\n" +

                "\n뒤 조건 활성 : " +
                spawnBack +

                "\n뒤 생성 확률 : " +
                backChance +

                "\n뒤 실제 생성 : " +
                hasBack +

                "\n" +

                "\nMotor 조건 활성 : " +
                useMotorTextureCondition +

                "\nMotor 0 확률 : " +
                motor0Chance +

                "\n실제 Motor Material : " +
                selectedMotorMaterial +

                "\n" +

                "\nNumber 조건 활성 : " +
                useSameNumberCondition +

                "\nNumber 같을 확률 : " +
                sameNumberChance +

                "\n앞 Number : " +
                selectedFrontNumber +

                "\n뒤 Number : " +
                selectedBackNumber +

                "\n" +

                "\nColor 조건 활성 : " +
                useSameColorCondition +

                "\nColor 같을 확률 : " +
                sameColorChance +

                "\n앞 Color : " +
                selectedFrontColor +

                "\n뒤 Color : " +
                selectedBackColor +

                "\n" +

                "\n최종 정상 여부 : " +
                IsValidMotor +

                "\n==========================================="
            );
        }
    }


    // =========================================================
    // 앞뒤 모두 존재
    // =========================================================

    private void GenerateBothPropellers(
        bool useSameNumberCondition,
        float sameNumberChance,

        bool useSameColorCondition,
        float sameColorChance
    )
    {
        // =====================================================
        // 앞 프로펠러 종류 선택
        // =====================================================

        int frontIndex =
            GetRandomFrontPropellerIndex();


        if (
            frontIndex < 0 ||
            frontIndex >= frontPropellers.Length ||
            frontPropellers[frontIndex] == null
        )
        {
            hasFront = false;

            return;
        }


        GameObject selectedFront =
            frontPropellers[
                frontIndex
            ];


        selectedFront.SetActive(
            true
        );


        selectedFrontNumber =
            GetPropellerNumber(
                selectedFront
            );


        // =====================================================
        // Number 같음 / 다름 결정
        // =====================================================

        bool makeSameNumber =
            false;


        if (useSameNumberCondition)
        {
            makeSameNumber =
                RollChance(
                    sameNumberChance
                );
        }


        int backIndex;


        if (makeSameNumber)
        {
            backIndex =
                FindPropellerIndexByNumber(
                    backPropellers,
                    selectedFrontNumber
                );
        }
        else
        {
            backIndex =
                GetDifferentPropellerIndex(
                    backPropellers,
                    selectedFrontNumber
                );
        }


        if (
            backIndex < 0 ||
            backIndex >= backPropellers.Length ||
            backPropellers[backIndex] == null
        )
        {
            hasBack = false;

            return;
        }


        GameObject selectedBack =
            backPropellers[
                backIndex
            ];


        selectedBack.SetActive(
            true
        );


        selectedBackNumber =
            GetPropellerNumber(
                selectedBack
            );


        // =====================================================
        // 앞 Color
        // =====================================================

        selectedFrontColor =
            GetRandomFrontColorIndex();


        // =====================================================
        // Color 같음 / 다름 결정
        // =====================================================

        bool makeSameColor =
            false;


        if (useSameColorCondition)
        {
            makeSameColor =
                RollChance(
                    sameColorChance
                );
        }


        if (makeSameColor)
        {
            selectedBackColor =
                selectedFrontColor;
        }
        else
        {
            selectedBackColor =
                GetDifferentBackColorIndex(
                    selectedFrontColor
                );
        }


        // =====================================================
        // Material 적용
        // =====================================================

        ApplyFrontColor(
            frontIndex,
            selectedFrontColor
        );


        ApplyBackColor(
            backIndex,
            selectedBackColor
        );
    }


    // =========================================================
    // 앞 프로펠러만 존재
    // =========================================================

    private void GenerateFrontOnly()
    {
        int frontIndex =
            GetRandomFrontPropellerIndex();


        if (
            frontIndex < 0 ||
            frontIndex >= frontPropellers.Length ||
            frontPropellers[frontIndex] == null
        )
        {
            hasFront = false;

            return;
        }


        GameObject selectedFront =
            frontPropellers[
                frontIndex
            ];


        selectedFront.SetActive(
            true
        );


        selectedFrontNumber =
            GetPropellerNumber(
                selectedFront
            );


        selectedFrontColor =
            GetRandomFrontColorIndex();


        ApplyFrontColor(
            frontIndex,
            selectedFrontColor
        );
    }


    // =========================================================
    // 뒤 프로펠러만 존재
    // =========================================================

    private void GenerateBackOnly()
    {
        int backIndex =
            GetRandomBackPropellerIndex();


        if (
            backIndex < 0 ||
            backIndex >= backPropellers.Length ||
            backPropellers[backIndex] == null
        )
        {
            hasBack = false;

            return;
        }


        GameObject selectedBack =
            backPropellers[
                backIndex
            ];


        selectedBack.SetActive(
            true
        );


        selectedBackNumber =
            GetPropellerNumber(
                selectedBack
            );


        selectedBackColor =
            GetRandomBackColorIndex();


        ApplyBackColor(
            backIndex,
            selectedBackColor
        );
    }


    // =========================================================
    // 기존 GenerateMotor
    // =========================================================

    public void GenerateMotor(
        bool forceValid
    )
    {
        ResetMotor();


        if (forceValid)
        {
            GenerateValidMotor();
        }
        else
        {
            GenerateRandomMotor();
        }


        CheckMotorValidity();


        if (showDebugLog)
        {
            PrintResult(
                forceValid
            );
        }
    }


    // =========================================================
    // A등급 강제 생성
    // =========================================================

    private void GenerateValidMotor()
    {
        // Motor 0
        selectedMotorMaterial =
            0;


        ApplyMotorMaterial(
            selectedMotorMaterial
        );


        // 앞뒤 모두 존재
        hasFront =
            true;

        hasBack =
            true;


        // 앞 종류 랜덤
        int frontIndex =
            GetRandomFrontPropellerIndex();


        if (
            frontIndex < 0 ||
            frontIndex >= frontPropellers.Length ||
            frontPropellers[frontIndex] == null
        )
        {
            hasFront = false;
            hasBack = false;

            return;
        }


        GameObject selectedFront =
            frontPropellers[
                frontIndex
            ];


        selectedFront.SetActive(
            true
        );


        selectedFrontNumber =
            GetPropellerNumber(
                selectedFront
            );


        // 같은 번호의 뒤 프로펠러
        int backIndex =
            FindPropellerIndexByNumber(
                backPropellers,
                selectedFrontNumber
            );


        if (
            backIndex < 0 ||
            backIndex >= backPropellers.Length ||
            backPropellers[backIndex] == null
        )
        {
            hasBack = false;

            return;
        }


        GameObject selectedBack =
            backPropellers[
                backIndex
            ];


        selectedBack.SetActive(
            true
        );


        selectedBackNumber =
            GetPropellerNumber(
                selectedBack
            );


        // 같은 색
        selectedFrontColor =
            GetRandomFrontColorIndex();

        selectedBackColor =
            selectedFrontColor;


        ApplyFrontColor(
            frontIndex,
            selectedFrontColor
        );


        ApplyBackColor(
            backIndex,
            selectedBackColor
        );
    }


    // =========================================================
    // 기존 완전 랜덤 생성
    // =========================================================

    private void GenerateRandomMotor()
    {
        // =====================================================
        // Motor Material
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
        // 프로펠러 생성 여부
        // =====================================================

        hasFront =
            RollChance(
                frontSpawnChance
            );


        hasBack =
            RollChance(
                backSpawnChance
            );


        // =====================================================
        // 둘 다 생성
        //
        // 완전 랜덤이므로 서로 독립적으로 생성
        // =====================================================

        if (
            hasFront &&
            hasBack
        )
        {
            int frontIndex =
                GetRandomFrontPropellerIndex();

            int backIndex =
                GetRandomBackPropellerIndex();


            if (
                frontIndex < 0 ||
                frontIndex >= frontPropellers.Length ||
                frontPropellers[frontIndex] == null
            )
            {
                hasFront = false;
            }
            else
            {
                GameObject selectedFront =
                    frontPropellers[
                        frontIndex
                    ];


                selectedFront.SetActive(
                    true
                );


                selectedFrontNumber =
                    GetPropellerNumber(
                        selectedFront
                    );


                selectedFrontColor =
                    GetRandomFrontColorIndex();


                ApplyFrontColor(
                    frontIndex,
                    selectedFrontColor
                );
            }


            if (
                backIndex < 0 ||
                backIndex >= backPropellers.Length ||
                backPropellers[backIndex] == null
            )
            {
                hasBack = false;
            }
            else
            {
                GameObject selectedBack =
                    backPropellers[
                        backIndex
                    ];


                selectedBack.SetActive(
                    true
                );


                selectedBackNumber =
                    GetPropellerNumber(
                        selectedBack
                    );


                selectedBackColor =
                    GetRandomBackColorIndex();


                ApplyBackColor(
                    backIndex,
                    selectedBackColor
                );
            }
        }

        else if (hasFront)
        {
            GenerateFrontOnly();
        }

        else if (hasBack)
        {
            GenerateBackOnly();
        }
    }


    // =========================================================
    // 초기화
    // =========================================================

    private void ResetMotor()
    {
        if (motorMain != null)
        {
            motorMain.SetActive(
                true
            );
        }


        if (frontPropellers != null)
        {
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
        }


        if (backPropellers != null)
        {
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
        }


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


        IsValidMotor =
            false;
    }


    // =========================================================
    // 확률 판정
    // =========================================================

    private bool RollChance(
        float chance
    )
    {
        chance =
            Mathf.Clamp(
                chance,
                0f,
                100f
            );


        return
            Random.Range(
                0f,
                100f
            ) <
            chance;
    }


    // =========================================================
    // Front Type Random
    // =========================================================

    private int GetRandomFrontPropellerIndex()
    {
        if (
            frontPropellers == null ||
            frontPropellers.Length < 3
        )
        {
            Debug.LogError(
                "Front Propeller 배열에 3 / 4 / 5를 모두 연결해주세요."
            );

            return -1;
        }


        float[] weights =
        {
            front3Weight,
            front4Weight,
            front5Weight
        };


        return GetWeightedRandomIndex(
            weights
        );
    }


    // =========================================================
    // Back Type Random
    // =========================================================

    private int GetRandomBackPropellerIndex()
    {
        if (
            backPropellers == null ||
            backPropellers.Length < 3
        )
        {
            Debug.LogError(
                "Back Propeller 배열에 3 / 4 / 5를 모두 연결해주세요."
            );

            return -1;
        }


        float[] weights =
        {
            back3Weight,
            back4Weight,
            back5Weight
        };


        return GetWeightedRandomIndex(
            weights
        );
    }


    // =========================================================
    // Front Color Random
    // =========================================================

    private int GetRandomFrontColorIndex()
    {
        if (
            propellerMaterials == null ||
            propellerMaterials.Length == 0
        )
        {
            return -1;
        }


        float[] weights =
        {
            frontColor0Weight,
            frontColor1Weight,
            frontColor2Weight,
            frontColor3Weight
        };


        int index =
            GetWeightedRandomIndex(
                weights
            );


        if (
            index < 0 ||
            index >= propellerMaterials.Length
        )
        {
            return 0;
        }


        return index;
    }


    // =========================================================
    // Back Color Random
    // =========================================================

    private int GetRandomBackColorIndex()
    {
        if (
            propellerMaterials == null ||
            propellerMaterials.Length == 0
        )
        {
            return -1;
        }


        float[] weights =
        {
            backColor0Weight,
            backColor1Weight,
            backColor2Weight,
            backColor3Weight
        };


        int index =
            GetWeightedRandomIndex(
                weights
            );


        if (
            index < 0 ||
            index >= propellerMaterials.Length
        )
        {
            return 0;
        }


        return index;
    }


    // =========================================================
    // 앞과 다른 Back Number 선택
    // =========================================================

    private int GetDifferentPropellerIndex(
        GameObject[] propellers,
        int excludedNumber
    )
    {
        if (
            propellers == null ||
            propellers.Length == 0
        )
        {
            return -1;
        }


        int[] candidates =
            new int[propellers.Length];

        float[] candidateWeights =
            new float[propellers.Length];

        int candidateCount =
            0;


        for (
            int i = 0;
            i < propellers.Length;
            i++
        )
        {
            if (propellers[i] == null)
                continue;


            int number =
                GetPropellerNumber(
                    propellers[i]
                );


            if (
                number ==
                excludedNumber
            )
            {
                continue;
            }


            candidates[
                candidateCount
            ] =
                i;


            // Back 가중치 사용
            if (number == 3)
            {
                candidateWeights[
                    candidateCount
                ] =
                    back3Weight;
            }
            else if (number == 4)
            {
                candidateWeights[
                    candidateCount
                ] =
                    back4Weight;
            }
            else if (number == 5)
            {
                candidateWeights[
                    candidateCount
                ] =
                    back5Weight;
            }
            else
            {
                candidateWeights[
                    candidateCount
                ] =
                    1f;
            }


            candidateCount++;
        }


        if (candidateCount == 0)
        {
            Debug.LogError(
                "다른 번호의 뒤 프로펠러를 찾을 수 없습니다."
            );

            return -1;
        }


        float[] finalWeights =
            new float[
                candidateCount
            ];


        for (
            int i = 0;
            i < candidateCount;
            i++
        )
        {
            finalWeights[i] =
                candidateWeights[i];
        }


        int selectedCandidate =
            GetWeightedRandomIndex(
                finalWeights
            );


        return candidates[
            selectedCandidate
        ];
    }


    // =========================================================
    // 앞과 다른 Back Color 선택
    // =========================================================

    private int GetDifferentBackColorIndex(
        int excludedColor
    )
    {
        if (
            propellerMaterials == null ||
            propellerMaterials.Length <= 1
        )
        {
            Debug.LogError(
                "서로 다른 프로펠러 색상을 만들 수 없습니다."
            );

            return -1;
        }


        int colorCount =
            Mathf.Min(
                propellerMaterials.Length,
                4
            );


        int[] candidates =
            new int[
                colorCount
            ];

        float[] candidateWeights =
            new float[
                colorCount
            ];


        int candidateCount =
            0;


        for (
            int i = 0;
            i < colorCount;
            i++
        )
        {
            if (
                i ==
                excludedColor
            )
            {
                continue;
            }


            candidates[
                candidateCount
            ] =
                i;


            candidateWeights[
                candidateCount
            ] =
                GetBackColorWeight(
                    i
                );


            candidateCount++;
        }


        if (candidateCount == 0)
        {
            return -1;
        }


        float[] finalWeights =
            new float[
                candidateCount
            ];


        for (
            int i = 0;
            i < candidateCount;
            i++
        )
        {
            finalWeights[i] =
                candidateWeights[i];
        }


        int selectedCandidate =
            GetWeightedRandomIndex(
                finalWeights
            );


        return candidates[
            selectedCandidate
        ];
    }


    // =========================================================
    // Back Color Weight
    // =========================================================

    private float GetBackColorWeight(
        int colorIndex
    )
    {
        switch (colorIndex)
        {
            case 0:
                return backColor0Weight;

            case 1:
                return backColor1Weight;

            case 2:
                return backColor2Weight;

            case 3:
                return backColor3Weight;
        }


        return 1f;
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
    // Front Color 적용
    // =========================================================

    private void ApplyFrontColor(
        int propellerIndex,
        int colorIndex
    )
    {
        if (
            frontPropellers == null ||
            propellerIndex < 0 ||
            propellerIndex >= frontPropellers.Length
        )
        {
            return;
        }


        if (
            propellerMaterials == null ||
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


        // Front는 Element 0
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
    // Back Color 적용
    // =========================================================

    private void ApplyBackColor(
        int propellerIndex,
        int colorIndex
    )
    {
        if (
            backPropellers == null ||
            propellerIndex < 0 ||
            propellerIndex >= backPropellers.Length
        )
        {
            return;
        }


        if (
            propellerMaterials == null ||
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
        // back_3 = Element 3
        // back_4 = Element 1
        // back_5 = Element 3
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
    // 같은 Number 찾기
    // =========================================================

    private int FindPropellerIndexByNumber(
        GameObject[] propellers,
        int targetNumber
    )
    {
        if (propellers == null)
            return -1;


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
                ) ==
                targetNumber
            )
            {
                return i;
            }
        }


        return -1;
    }


    // =========================================================
    // 가중치 랜덤
    // =========================================================

    private int GetWeightedRandomIndex(
        float[] weights
    )
    {
        if (
            weights == null ||
            weights.Length == 0
        )
        {
            return -1;
        }


        float totalWeight =
            0f;


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


        // 전부 0이면 균등 랜덤
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


            if (
                randomValue <
                weights[i]
            )
            {
                return i;
            }


            randomValue -=
                weights[i];
        }


        return weights.Length - 1;
    }


    // =========================================================
    // 이름에서 Propeller Number 가져오기
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
    // 최종 A 여부
    // =========================================================

    private void CheckMotorValidity()
    {
        IsValidMotor =
            true;


        // 프로펠러 누락
        if (
            !hasFront ||
            !hasBack
        )
        {
            IsValidMotor =
                false;
        }


        // Number 다름
        if (
            selectedFrontNumber !=
            selectedBackNumber
        )
        {
            IsValidMotor =
                false;
        }


        // Color 다름
        if (
            selectedFrontColor !=
            selectedBackColor
        )
        {
            IsValidMotor =
                false;
        }


        // Motor 얼룩
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
    // 기존 호환용
    // =========================================================

    public bool GetMotorResult()
    {
        return IsValidMotor;
    }


    // =========================================================
    // 기존 Debug
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