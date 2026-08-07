using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class BoxMatchManager : MonoBehaviour
{
    // =========================================================
    // Resources 경로
    // =========================================================

    [Header("Resources 폴더 내부 경로")]

    [Tooltip("Assets/Resources를 제외한 Boxes 폴더 경로")]
    [SerializeField]
    private string boxesResourcePath = "Box+Tape/Boxes";

    [Tooltip("Assets/Resources를 제외한 Tapes 폴더 경로")]
    [SerializeField]
    private string tapesResourcePath = "Box+Tape/Tapes";


    // =========================================================
    // 표시 기준점
    // =========================================================

    [Header("소품 표시 위치")]

    [Tooltip("위치 기준으로 사용할 Cube를 넣으세요. Cube의 Mesh Renderer는 자동으로 꺼집니다.")]
    [SerializeField]
    private Transform displayAnchor;


    // =========================================================
    // Box / Tape 상대 위치
    // =========================================================

    [Header("Box / Tape 위치 보정")]

    [Tooltip("Anchor를 기준으로 한 Box의 상대 위치")]
    [SerializeField]
    private Vector3 boxLocalPosition = Vector3.zero;

    [Tooltip("Anchor를 기준으로 한 Tape의 상대 위치")]
    [SerializeField]
    private Vector3 tapeLocalPosition = Vector3.zero;


    // =========================================================
    // 회전
    // =========================================================

    [Header("Box / Tape 회전 보정")]

    [SerializeField]
    private Vector3 boxLocalRotation = Vector3.zero;

    [SerializeField]
    private Vector3 tapeLocalRotation = Vector3.zero;


    // =========================================================
    // 크기
    // =========================================================

    [Header("Box / Tape 크기")]

    [SerializeField]
    private Vector3 boxScale = Vector3.one;

    [SerializeField]
    private Vector3 tapeScale = Vector3.one;


    // =========================================================
    // 확률
    // =========================================================

    [Header("결과 확률")]

    [Tooltip("Open 테이프가 선택될 확률")]
    [Range(0f, 100f)]
    [SerializeField]
    private float openChance = 30f;

    [Tooltip("Demage 박스가 선택될 확률")]
    [Range(0f, 100f)]
    [SerializeField]
    private float demageChance = 30f;


    // =========================================================
    // 회전 조작
    // =========================================================

    [Header("마우스 회전")]

    [Tooltip("좌클릭 드래그 회전 속도")]
    [SerializeField]
    private float mouseRotationSpeed = 0.2f;


    // =========================================================
    // WASD 위치 조작
    // =========================================================

    [Header("WASD 미세 회전")]

[Tooltip("WASD 미세 회전 속도")]
[SerializeField]
private float fineRotationSpeed = 30f;


    // =========================================================
    // Prefab 목록
    // =========================================================

    private readonly List<GameObject> normalBoxes = new();
    private readonly List<GameObject> demageBoxes = new();

    private readonly List<GameObject> closeTapes = new();
    private readonly List<GameObject> openTapes = new();


    // =========================================================
    // 현재 생성된 오브젝트
    // =========================================================

    private GameObject currentHolder;
    private GameObject currentBox;
    private GameObject currentTape;


    // =========================================================
    // 진행 상태
    // =========================================================

    private bool isReady = false;
    public bool IsReady => isReady;
    private bool testFinished = false;

    // 현재 보고 있는 상자 번호
    // 첫 번째 상자 = 1
    private int currentBoxNumber = 0;

    // False 총 개수
    private int falseCount = 0;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        HideAnchorCube();
        LoadPrefabs();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (!isReady || testFinished)
            return;


        // -----------------------------------------------------
        // 마우스 좌클릭 드래그 회전
        // -----------------------------------------------------

        HandleMouseRotation();


        // -----------------------------------------------------
        // WASD 위치 미세 조절
        // -----------------------------------------------------

        HandleFineRotation();
    }


    // =========================================================
    // Anchor Cube 숨기기
    // =========================================================

    private void HideAnchorCube()
    {
        if (displayAnchor == null)
            return;


        // Cube 자체 Renderer
        MeshRenderer renderer =
            displayAnchor.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.enabled = false;
        }


        // 혹시 자식 Renderer가 있다면 같이 숨김
        MeshRenderer[] childRenderers =
            displayAnchor.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer childRenderer in childRenderers)
        {
            childRenderer.enabled = false;
        }
    }


    // =========================================================
    // Prefab 로딩
    // =========================================================

    private void LoadPrefabs()
    {
        normalBoxes.Clear();
        demageBoxes.Clear();

        closeTapes.Clear();
        openTapes.Clear();


        GameObject[] allBoxes =
            Resources.LoadAll<GameObject>(
                boxesResourcePath
            );


        GameObject[] allTapes =
            Resources.LoadAll<GameObject>(
                tapesResourcePath
            );


        // -----------------------------------------------------
        // Box 분류
        // -----------------------------------------------------

        foreach (GameObject boxPrefab in allBoxes)
        {
            string lowerName =
                boxPrefab.name.ToLowerInvariant();


            if (lowerName.Contains("demage") ||
                lowerName.Contains("damage"))
            {
                demageBoxes.Add(boxPrefab);
            }
            else
            {
                normalBoxes.Add(boxPrefab);
            }
        }


        // -----------------------------------------------------
        // Tape 분류
        // -----------------------------------------------------

        foreach (GameObject tapePrefab in allTapes)
        {
            string lowerName =
                tapePrefab.name.ToLowerInvariant();


            if (lowerName.Contains("open"))
            {
                openTapes.Add(tapePrefab);
            }
            else
            {
                closeTapes.Add(tapePrefab);
            }
        }


        isReady =
            ValidatePrefabLists();
    }


    // =========================================================
    // Prefab 확인
    // =========================================================

    private bool ValidatePrefabLists()
    {
        bool valid = true;


        if (normalBoxes.Count == 0)
        {
            Debug.LogError(
                $"일반 Box가 없습니다. Resources/{boxesResourcePath}"
            );

            valid = false;
        }


        if (demageBoxes.Count == 0)
        {
            Debug.LogError(
                $"Demage Box가 없습니다. Resources/{boxesResourcePath}"
            );

            valid = false;
        }


        if (closeTapes.Count == 0)
        {
            Debug.LogError(
                $"Close Tape가 없습니다. Resources/{tapesResourcePath}"
            );

            valid = false;
        }


        if (openTapes.Count == 0)
        {
            Debug.LogError(
                $"Open Tape가 없습니다. Resources/{tapesResourcePath}"
            );

            valid = false;
        }


        if (displayAnchor == null)
        {
            Debug.LogError(
                "Display Anchor가 설정되지 않았습니다."
            );

            valid = false;
        }


        return valid;
    }


    // =========================================================
    // 랜덤 Box + Tape 생성
    // =========================================================

    public InspectionResult CreateNextMatch()
    {
        RemoveCurrentMatch();


        // -----------------------------------------------------
        // 진행 번호 증가
        // -----------------------------------------------------

        currentBoxNumber++;


        // -----------------------------------------------------
        // 확률 보정
        // -----------------------------------------------------

        float safeOpenChance =
            Mathf.Clamp(
                openChance,
                0f,
                100f
            );


        float safeDemageChance =
            Mathf.Clamp(
                demageChance,
                0f,
                100f - safeOpenChance
            );


        float randomValue =
            Random.Range(
                0f,
                100f
            );


        GameObject selectedBoxPrefab;
        GameObject selectedTapePrefab;

        InspectionResult answer;


        // 헌박스 (Opened)

        if (randomValue < safeOpenChance)
        {
            selectedBoxPrefab = GetRandomPrefab(normalBoxes);
            selectedTapePrefab = GetRandomPrefab(openTapes);

            answer = InspectionResult.Opened;
        }

        else if (randomValue < safeOpenChance + safeDemageChance)
        {
            selectedBoxPrefab = GetRandomPrefab(demageBoxes);
            selectedTapePrefab = GetRandomPrefab(closeTapes);

            answer = InspectionResult.Opened;
        }

        // 새 박스 (Unopened)

        else
        {
            selectedBoxPrefab = GetRandomPrefab(normalBoxes);
            selectedTapePrefab = GetRandomPrefab(closeTapes);

            answer = InspectionResult.Unopened;
        }

        // =====================================================
        // Box + Tape를 한 묶음으로 만들 Holder 생성
        // =====================================================

        currentHolder =
            new GameObject(
                "CurrentBoxHolder"
            );


        currentHolder.transform.SetParent(
            displayAnchor,
            false
        );


        currentHolder.transform.localPosition =
            Vector3.zero;


        currentHolder.transform.localRotation =
            Quaternion.identity;


        // =====================================================
        // Box 생성
        // =====================================================

        currentBox =
            Instantiate(
                selectedBoxPrefab,
                currentHolder.transform
            );


        currentBox.transform.localPosition =
            boxLocalPosition;


        currentBox.transform.localRotation =
            Quaternion.Euler(
                boxLocalRotation
            );


        currentBox.transform.localScale =
            boxScale;


        // =====================================================
        // Tape 생성
        // =====================================================

        currentTape =
            Instantiate(
                selectedTapePrefab,
                currentHolder.transform
            );


        currentTape.transform.localPosition =
            tapeLocalPosition;


        currentTape.transform.localRotation =
            Quaternion.Euler(
                tapeLocalRotation
            );


        currentTape.transform.localScale =
            tapeScale;

        return answer;
    }


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


        // 좌 / 우
        // 월드 Y축 기준
        currentHolder.transform.Rotate(
            Vector3.up,
            horizontalRotation,
            Space.World
        );


        // 위 / 아래
        // 월드 X축 기준
        currentHolder.transform.Rotate(
            Vector3.left,
            verticalRotation,
            Space.World
        );
    }


    // =========================================================
    // WASD 미세 회전
    // 전부 월드 기준
    // =========================================================

    private void HandleFineRotation()
    {
        if (currentHolder == null)
            return;

        if (Keyboard.current == null)
            return;


        float verticalRotation = 0f;
        float horizontalRotation = 0f;


        // W = 위쪽 회전
        if (Keyboard.current.wKey.isPressed)
        {
            verticalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // S = 아래쪽 회전
        if (Keyboard.current.sKey.isPressed)
        {
            verticalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // A = 왼쪽 회전
        if (Keyboard.current.aKey.isPressed)
        {
            horizontalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // D = 오른쪽 회전
        if (Keyboard.current.dKey.isPressed)
        {
            horizontalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        // W / S
        // 월드 X축 기준
        if (verticalRotation != 0f)
        {
            currentHolder.transform.Rotate(
                Vector3.right,
                verticalRotation,
                Space.World
            );
        }


        // A / D
        // 월드 Y축 기준
        if (horizontalRotation != 0f)
        {
            currentHolder.transform.Rotate(
                Vector3.up,
                horizontalRotation,
                Space.World
            );
        }
    }


    // =========================================================
    // 랜덤 Prefab 선택
    // =========================================================

    private GameObject GetRandomPrefab(
        List<GameObject> prefabList
    )
    {
        int randomIndex =
            Random.Range(
                0,
                prefabList.Count
            );


        return prefabList[
            randomIndex
        ];
    }


    // =========================================================
    // 현재 Box 제거
    // =========================================================

    public void RemoveCurrentMatch()
    {
        if (currentHolder != null)
        {
            Destroy(
                currentHolder
            );

            currentHolder =
                null;

            currentBox =
                null;

            currentTape =
                null;
        }
    }


    // =========================================================
    // Inspector 값 제한
    // =========================================================

    private void OnValidate()
    {
        openChance =
            Mathf.Clamp(
                openChance,
                0f,
                100f
            );


        demageChance =
            Mathf.Clamp(
                demageChance,
                0f,
                100f - openChance
            );


        fineRotationSpeed =
    Mathf.Max(
        0f,
        fineRotationSpeed
    );

        mouseRotationSpeed =
            Mathf.Max(
                0f,
                mouseRotationSpeed
            );
    }
}