using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxMatchManager : MonoBehaviour
{
    // =========================================================
    // Resources 경로
    // =========================================================

    [Header("Resources 경로 설정")]

    [Tooltip("Assets/Resources 안에 있는 Boxes 폴더 경로")]
    [SerializeField]
    private string boxesResourcePath = "Box+Tape/Boxes";

    [Tooltip("Assets/Resources 안에 있는 Tapes 폴더 경로")]
    [SerializeField]
    private string tapesResourcePath = "Box+Tape/Tapes";


    // =========================================================
    // 상품 표시 위치
    // =========================================================

    [Header("상품 표시 위치")]

    [Tooltip("박스/테이프가 생성될 기준 Transform")]
    [SerializeField]
    private Transform displayAnchor;


    // =========================================================
    // Box / Tape 기본 위치
    // =========================================================

    [Header("Box / Tape 위치 설정")]

    [Tooltip("Anchor 기준 Box 로컬 위치")]
    [SerializeField]
    private Vector3 boxLocalPosition = Vector3.zero;

    [Tooltip("Anchor 기준 Tape 로컬 위치")]
    [SerializeField]
    private Vector3 tapeLocalPosition = Vector3.zero;


    // =========================================================
    // Box / Tape 기본 회전
    // =========================================================

    [Header("Box / Tape 회전 설정")]

    [SerializeField]
    private Vector3 boxLocalRotation = Vector3.zero;

    [SerializeField]
    private Vector3 tapeLocalRotation = Vector3.zero;


    // =========================================================
    // Box / Tape 기본 크기
    // =========================================================

    [Header("Box / Tape 크기")]

    [SerializeField]
    private Vector3 boxScale = Vector3.one;

    [SerializeField]
    private Vector3 tapeScale = Vector3.one;


    // =========================================================
    // 생성 확률
    // =========================================================

    [Header("생성 확률")]

    [Tooltip("Open 상태가 선택될 확률")]
    [Range(0f, 100f)]
    [SerializeField]
    private float openChance = 30f;

    [Tooltip("Damage 박스가 선택될 확률")]
    [Range(0f, 100f)]
    [SerializeField]
    private float demageChance = 30f;


    // =========================================================
    // 마우스 회전
    // =========================================================

    [Header("마우스 회전")]

    [Tooltip("좌클릭 드래그 회전 감도")]
    [SerializeField]
    private float mouseRotationSpeed = 0.2f;


    // =========================================================
    // 마우스 줌
    // =========================================================

    [Header("마우스 줌")]

    [Tooltip("마우스 휠 확대/축소 속도")]
    [SerializeField]
    private float zoomSpeed = 0.15f;

    [Tooltip("최소 확대 비율")]
    [SerializeField]
    private float minZoomScale = 0.9f;

    [Tooltip("최대 확대 비율")]
    [SerializeField]
    private float maxZoomScale = 1.7f;

    private float currentZoomScale = 1f;


    // =========================================================
    // WASD 미세 회전
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

    private int currentBoxNumber = 0;

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

        // 좌클릭 드래그 회전
        HandleMouseRotation();

        // 마우스 휠 확대 / 축소
        HandleMouseZoom();

        // WASD 미세 회전
        HandleFineRotation();
    }


    // =========================================================
    // Anchor Cube 숨기기
    // =========================================================

    private void HideAnchorCube()
    {
        if (displayAnchor == null)
            return;

        MeshRenderer renderer =
            displayAnchor.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.enabled = false;
        }

        MeshRenderer[] childRenderers =
            displayAnchor.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer childRenderer in childRenderers)
        {
            childRenderer.enabled = false;
        }
    }


    // =========================================================
    // Prefab 로드
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
    // Prefab 유효성 검사
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
                $"Damage Box가 없습니다. Resources/{boxesResourcePath}"
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
                "Display Anchor가 지정되지 않았습니다."
            );

            valid = false;
        }

        return valid;
    }


    // =========================================================
    // 다음 Box + Tape 생성
    // =========================================================

    public InspectionResult CreateNextMatch()
    {
        RemoveCurrentMatch();

        currentBoxNumber++;


        // -----------------------------------------------------
        // 확률 계산
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


        // -----------------------------------------------------
        // Opened
        // -----------------------------------------------------

        if (randomValue < safeOpenChance)
        {
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(openTapes);

            answer =
                InspectionResult.Opened;
        }

        // -----------------------------------------------------
        // Damage Box
        // 내부 손상 박스이므로 결국 개봉 필요
        // -----------------------------------------------------

        else if (
            randomValue <
            safeOpenChance + safeDemageChance
        )
        {
            selectedBoxPrefab =
                GetRandomPrefab(demageBoxes);

            selectedTapePrefab =
                GetRandomPrefab(closeTapes);

            answer =
                InspectionResult.Opened;
        }

        // -----------------------------------------------------
        // 정상 미개봉
        // -----------------------------------------------------

        else
        {
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(closeTapes);

            answer =
                InspectionResult.Unopened;
        }


        // =====================================================
        // Box + Tape 공통 Holder 생성
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


        // 새 박스 생성 시 줌 초기화
        currentZoomScale = 1f;

        currentHolder.transform.localScale =
            Vector3.one;


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


    // =========================================================
    // 마우스 좌클릭 드래그 회전
    // =========================================================

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
        // World Y축 회전
        currentHolder.transform.Rotate(
            Vector3.up,
            horizontalRotation,
            Space.World
        );


        // 위 / 아래
        // World X축 회전
        currentHolder.transform.Rotate(
            Vector3.left,
            verticalRotation,
            Space.World
        );
    }


    // =========================================================
    // 마우스 휠 확대 / 축소
    // =========================================================

    private void HandleMouseZoom()
    {
        if (currentHolder == null)
            return;

        if (Mouse.current == null)
            return;

        float scrollValue =
            Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollValue) < 0.01f)
            return;


        // 휠 위 = 확대
        // 휠 아래 = 축소
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


    // =========================================================
    // WASD 미세 회전
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
        if (verticalRotation != 0f)
        {
            currentHolder.transform.Rotate(
                Vector3.right,
                verticalRotation,
                Space.World
            );
        }


        // A / D
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

            currentZoomScale =
                1f;
        }
    }


    // =========================================================
    // Inspector 값 보정
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