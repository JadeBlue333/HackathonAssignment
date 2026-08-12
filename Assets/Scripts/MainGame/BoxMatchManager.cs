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

    [Tooltip("기본 좌클릭 드래그 회전 속도")]
    [SerializeField]
    private float baseMouseRotationSpeed = 0.2f;

    [Tooltip("환경설정 값이 없을 때 사용할 기본 물체 회전 감도")]
    [SerializeField]
    private float defaultObjectRotationSensitivity = 1f;

    private const string ObjectRotationSensitivityKey =
        "ObjectRotationSensitivity";


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
    // Current Inspection State
    // =========================================================

    /// <summary>
    /// 현재 박스가 손상된 박스인지
    /// true = 손상 있음
    /// false = 정상 박스
    /// </summary>
    public bool CurrentBoxDamaged
    {
        get;
        private set;
    }


    /// <summary>
    /// 현재 테이프가 개봉 흔적이 있는 상태인지
    /// true = Open Tape
    /// false = Close Tape
    /// </summary>
    public bool CurrentTapeOpened
    {
        get;
        private set;
    }


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

        HandleMouseRotation();

        HandleMouseZoom();

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


        // =====================================================
        // Open Tape
        //
        // 박스 정상
        // 테이프 개봉 흔적 있음
        // → Opened
        // =====================================================

        if (randomValue < safeOpenChance)
        {
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(openTapes);

            answer =
                InspectionResult.Opened;


            // 현재 상태 저장
            CurrentBoxDamaged =
                false;

            CurrentTapeOpened =
                true;
        }


        // =====================================================
        // Damage Box
        //
        // 박스 손상 있음
        // 테이프 정상
        // → Opened
        // =====================================================

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


            // 현재 상태 저장
            CurrentBoxDamaged =
                true;

            CurrentTapeOpened =
                false;
        }


        // =====================================================
        // 정상 미개봉
        //
        // 박스 정상
        // 테이프 정상
        // → Unopened
        // =====================================================

        else
        {
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(closeTapes);

            answer =
                InspectionResult.Unopened;


            // 현재 상태 저장
            CurrentBoxDamaged =
                false;

            CurrentTapeOpened =
                false;
        }


        // =====================================================
        // Holder 생성
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
        // Zoom 초기화
        // =====================================================

        currentZoomScale =
            1f;

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


        // 환경설정에서 스크롤 반전 체크했으면 방향 반전
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


        if (Keyboard.current.upArrowKey.isPressed)
        {
            verticalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.downArrowKey.isPressed)
        {
            verticalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.rightArrowKey.isPressed)
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

            currentHolder = null;
            currentBox = null;
            currentTape = null;

            currentZoomScale = 1f;
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