using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxMatchManager : MonoBehaviour
{
    [Header("Resources 폴더 내부 경로")]
    [Tooltip("Assets/Resources를 제외한 Boxes 폴더 경로")]
    [SerializeField] private string boxesResourcePath = "Box+Tape/Boxes";

    [Tooltip("Assets/Resources를 제외한 Tapes 폴더 경로")]
    [SerializeField] private string tapesResourcePath = "Box+Tape/Tapes";


    [Header("생성 위치")]
    [SerializeField] private Vector3 boxPosition = Vector3.zero;
    [SerializeField] private Vector3 tapePosition = Vector3.zero;


    [Header("생성 회전")]
    [SerializeField] private Vector3 boxRotation = Vector3.zero;
    [SerializeField] private Vector3 tapeRotation = Vector3.zero;


    [Header("생성 크기")]
    [SerializeField] private Vector3 boxScale = Vector3.one;
    [SerializeField] private Vector3 tapeScale = Vector3.one;


    [Header("결과 확률")]
    [Tooltip("Open 테이프가 선택될 확률입니다.")]
    [Range(0f, 100f)]
    [SerializeField] private float openChance = 30f;

    [Tooltip("Demage 박스가 선택될 확률입니다.")]
    [Range(0f, 100f)]
    [SerializeField] private float demageChance = 30f;


    [Header("입력 설정")]
    [SerializeField] private bool useMouseClick = true;
    [SerializeField] private bool useTouchInput = true;


    private readonly List<GameObject> normalBoxes = new();
    private readonly List<GameObject> demageBoxes = new();

    private readonly List<GameObject> closeTapes = new();
    private readonly List<GameObject> openTapes = new();


    private GameObject currentBox;
    private GameObject currentTape;

    private bool isReady;


    private void Start()
    {
        LoadPrefabs();
    }


    private void Update()
    {
        if (!isReady)
            return;

        bool mouseClicked = false;
        bool screenTouched = false;

        if (useMouseClick && Mouse.current != null)
        {
            mouseClicked =
                Mouse.current.leftButton.wasPressedThisFrame;
        }

        if (useTouchInput && Touchscreen.current != null)
        {
            screenTouched =
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        }

        if (mouseClicked || screenTouched)
        {
            CreateRandomMatch();
        }
    }


    private void LoadPrefabs()
    {
        normalBoxes.Clear();
        demageBoxes.Clear();
        closeTapes.Clear();
        openTapes.Clear();

        GameObject[] allBoxes =
            Resources.LoadAll<GameObject>(boxesResourcePath);

        GameObject[] allTapes =
            Resources.LoadAll<GameObject>(tapesResourcePath);


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


        Debug.Log(
            $"일반 박스: {normalBoxes.Count}개 / " +
            $"Demage 박스: {demageBoxes.Count}개 / " +
            $"Close 테이프: {closeTapes.Count}개 / " +
            $"Open 테이프: {openTapes.Count}개"
        );


        isReady = ValidatePrefabLists();

        if (isReady)
        {
            Debug.Log("박스 조합 준비 완료. Game 화면을 클릭하세요.");
        }
    }


    private bool ValidatePrefabLists()
    {
        bool valid = true;

        if (normalBoxes.Count == 0)
        {
            Debug.LogError(
                $"일반 박스를 찾지 못했습니다. " +
                $"경로 확인: Resources/{boxesResourcePath}"
            );

            valid = false;
        }

        if (demageBoxes.Count == 0)
        {
            Debug.LogError(
                $"이름에 demage 또는 damage가 포함된 박스를 " +
                $"찾지 못했습니다. 경로 확인: " +
                $"Resources/{boxesResourcePath}"
            );

            valid = false;
        }

        if (closeTapes.Count == 0)
        {
            Debug.LogError(
                $"Close 테이프를 찾지 못했습니다. " +
                $"이름에 open이 없는 테이프가 필요합니다. " +
                $"경로 확인: Resources/{tapesResourcePath}"
            );

            valid = false;
        }

        if (openTapes.Count == 0)
        {
            Debug.LogError(
                $"이름에 open이 포함된 테이프를 찾지 못했습니다. " +
                $"경로 확인: Resources/{tapesResourcePath}"
            );

            valid = false;
        }

        return valid;
    }


    public void CreateRandomMatch()
    {
        if (!isReady)
            return;

        RemoveCurrentMatch();


        float safeOpenChance =
            Mathf.Clamp(openChance, 0f, 100f);

        float safeDemageChance =
            Mathf.Clamp(
                demageChance,
                0f,
                100f - safeOpenChance
            );


        float randomValue =
            Random.Range(0f, 100f);


        GameObject selectedBoxPrefab;
        GameObject selectedTapePrefab;

        bool result;


        if (randomValue < safeOpenChance)
        {
            // Open 테이프 조합
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(openTapes);

            result = true;
        }
        else if (
            randomValue <
            safeOpenChance + safeDemageChance
        )
        {
            // Demage 박스 조합
            selectedBoxPrefab =
                GetRandomPrefab(demageBoxes);

            selectedTapePrefab =
                GetRandomPrefab(closeTapes);

            result = true;
        }
        else
        {
            // 정상 박스 + 닫힌 테이프 조합
            selectedBoxPrefab =
                GetRandomPrefab(normalBoxes);

            selectedTapePrefab =
                GetRandomPrefab(closeTapes);

            result = false;
        }


        currentBox = Instantiate(
            selectedBoxPrefab,
            boxPosition,
            Quaternion.Euler(boxRotation)
        );

        currentTape = Instantiate(
            selectedTapePrefab,
            tapePosition,
            Quaternion.Euler(tapeRotation)
        );


        currentBox.transform.localScale = boxScale;
        currentTape.transform.localScale = tapeScale;


        // 클릭 한 번당 한 번만 출력
        Debug.Log(result);
    }


    private GameObject GetRandomPrefab(
        List<GameObject> prefabList
    )
    {
        int randomIndex =
            Random.Range(0, prefabList.Count);

        return prefabList[randomIndex];
    }


    private void RemoveCurrentMatch()
    {
        if (currentBox != null)
        {
            Destroy(currentBox);
            currentBox = null;
        }

        if (currentTape != null)
        {
            Destroy(currentTape);
            currentTape = null;
        }
    }


    private void OnValidate()
    {
        openChance =
            Mathf.Clamp(openChance, 0f, 100f);

        demageChance =
            Mathf.Clamp(
                demageChance,
                0f,
                100f - openChance
            );
    }
}