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
    // 감도 공통 설정
    // =========================================================

    public const float MinRotationSensitivity = 0.2f;
    public const float MaxRotationSensitivity = 20f;


    // =========================================================
    // 마우스 회전
    // =========================================================

    [Header("마우스 회전")]

    [Tooltip("기본 좌클릭 드래그 회전 속도")]
    [SerializeField]
    private float baseMouseRotationSpeed = 0.2f;

    [Tooltip("환경설정 값이 없을 때 사용할 기본 마우스 회전 감도")]
    [Range(MinRotationSensitivity, MaxRotationSensitivity)]
    [SerializeField]
    private float defaultObjectRotationSensitivity = 1f;

    public const string ObjectRotationSensitivityKey =
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
    // 방향키 회전
    // =========================================================

    [Header("방향키 회전")]

    [Tooltip("기본 방향키 회전 속도")]
    [SerializeField]
    private float baseKeyboardRotationSpeed = 30f;

    [Tooltip("환경설정 값이 없을 때 사용할 기본 방향키 회전 감도")]
    [Range(MinRotationSensitivity, MaxRotationSensitivity)]
    [SerializeField]
    private float defaultKeyboardRotationSensitivity = 1f;

    public const string KeyboardRotationSensitivityKey =
        "KeyboardRotationSensitivity";


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

        ValidateSavedSensitivity();

        LoadPrefabs();
    }


    // =========================================================
    // 저장된 감도 값 보정
    // =========================================================

    private void ValidateSavedSensitivity()
    {
        float mouseSensitivity =
            PlayerPrefs.GetFloat(
                ObjectRotationSensitivityKey,
                defaultObjectRotationSensitivity
            );

        mouseSensitivity =
            Mathf.Clamp(
                mouseSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );

        PlayerPrefs.SetFloat(
            ObjectRotationSensitivityKey,
            mouseSensitivity
        );


        float keyboardSensitivity =
            PlayerPrefs.GetFloat(
                KeyboardRotationSensitivityKey,
                defaultKeyboardRotationSensitivity
            );

        keyboardSensitivity =
            Mathf.Clamp(
                keyboardSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );

        PlayerPrefs.SetFloat(
            KeyboardRotationSensitivityKey,
            keyboardSensitivity
        );

        PlayerPrefs.Save();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (!isReady || testFinished)
            return;


        // 팝업이 하나라도 열려 있으면
        // 박스 회전 / 확대축소 / 방향키 회전만 차단
        if (
            PopupManager.Instance != null &&
            PopupManager.Instance.HasOpenPopup()
        )
        {
            return;
        }


        HandleMouseRotation();

        HandleMouseZoom();

        HandleKeyboardRotation();
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
        // =====================================================

        if (randomValue < safeOpenChance)
        {
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(openTapes);

            answer =
                InspectionResult.Opened;

            CurrentBoxDamaged = false;
            CurrentTapeOpened = true;
        }


        // =====================================================
        // Damage Box
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

            CurrentBoxDamaged = true;
            CurrentTapeOpened = false;
        }


        // =====================================================
        // 정상 미개봉
        // =====================================================

        else
        {
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(closeTapes);

            answer =
                InspectionResult.Unopened;

            CurrentBoxDamaged = false;
            CurrentTapeOpened = false;
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


        float objectRotationSensitivity =
            PlayerPrefs.GetFloat(
                ObjectRotationSensitivityKey,
                defaultObjectRotationSensitivity
            );


        objectRotationSensitivity =
            Mathf.Clamp(
                objectRotationSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
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
    // 방향키 회전
    // =========================================================

    private void HandleKeyboardRotation()
    {
        if (currentHolder == null)
            return;

        if (Keyboard.current == null)
            return;


        float keyboardSensitivity =
            PlayerPrefs.GetFloat(
                KeyboardRotationSensitivityKey,
                defaultKeyboardRotationSensitivity
            );


        keyboardSensitivity =
            Mathf.Clamp(
                keyboardSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        float finalRotationSpeed =
            baseKeyboardRotationSpeed *
            keyboardSensitivity;


        float verticalRotation = 0f;
        float horizontalRotation = 0f;


        // 위
        if (Keyboard.current.upArrowKey.isPressed)
        {
            verticalRotation +=
                finalRotationSpeed *
                Time.deltaTime;
        }


        // 아래
        if (Keyboard.current.downArrowKey.isPressed)
        {
            verticalRotation -=
                finalRotationSpeed *
                Time.deltaTime;
        }


        // 왼쪽
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalRotation +=
                finalRotationSpeed *
                Time.deltaTime;
        }


        // 오른쪽
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalRotation -=
                finalRotationSpeed *
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


        baseMouseRotationSpeed =
            Mathf.Max(
                0f,
                baseMouseRotationSpeed
            );


        defaultObjectRotationSensitivity =
            Mathf.Clamp(
                defaultObjectRotationSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
            );


        baseKeyboardRotationSpeed =
            Mathf.Max(
                0f,
                baseKeyboardRotationSpeed
            );


        defaultKeyboardRotationSensitivity =
            Mathf.Clamp(
                defaultKeyboardRotationSensitivity,
                MinRotationSensitivity,
                MaxRotationSensitivity
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