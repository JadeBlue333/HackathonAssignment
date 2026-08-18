using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    // =====================================================
    // Tutorial Panels
    // =====================================================

    [Header("Tutorial Panels")]
    [SerializeField] private GameObject[] tutorialPanels;


    // =====================================================
    // Tutorial Texts
    // =====================================================

    [Header("Tutorial Texts")]
    [SerializeField] private TMP_Text[] tutorialTexts;


    // =====================================================
    // Typewriter
    // =====================================================

    [Header("Typewriter")]
    [SerializeField] private float typingSpeed = 0.03f;


    // =====================================================
    // Display Anchor
    // =====================================================

    [Header("Display Anchor")]
    [SerializeField] private Transform boxDisplayAnchor;
    [SerializeField] private Transform motorDisplayAnchor;


    // =====================================================
    // Display Prefab
    // =====================================================

    [Header("Display Prefab")]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject motorPrefab;


    // =====================================================
    // Display Scale
    // =====================================================

    [Header("Display Scale")]
    [SerializeField] private Vector3 boxDisplayScale = Vector3.one;
    [SerializeField] private Vector3 motorDisplayScale = Vector3.one;


    // =====================================================
    // Rotation
    // =====================================================

    [Header("Rotation")]
    [SerializeField] private float mouseRotationSpeed = 0.5f;
    [SerializeField] private float fineRotationSpeed = 100f;


    // =====================================================
    // Sound
    // =====================================================

    [Header("Panel Change Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip panelChangeSound;

    [Range(0f, 1f)]
    [SerializeField] private float panelSoundVolume = 1f;


    // =====================================================
    // Scene Transition
    // =====================================================

    [Header("Scene Transition")]
    [SerializeField] private GoToThisScene goToThisScene;

    [SerializeField] private AudioClip sceneTransitionSound;

    [Range(0f, 1f)]
    [SerializeField] private float sceneSoundVolume = 1f;


    // =====================================================
    // Private - Tutorial
    // =====================================================

    private int currentPanelIndex = 0;

    private Coroutine typingCoroutine;

    private bool isTyping = false;

    private string currentFullText;


    // =====================================================
    // Private - Display Object
    // =====================================================

    private GameObject boxObject;
    private GameObject motorObject;

    private Transform boxHolder;
    private Transform motorHolder;


    // =====================================================
    // Start
    // =====================================================

    private void Start()
    {
        InitializeTutorial();

        CreateDisplayObjects();

        // 첫 번째 패널 효과음
        PlayPanelChangeSound();
    }


    // =====================================================
    // Tutorial 초기화
    // =====================================================

    private void InitializeTutorial()
    {
        // 모든 패널 비활성화
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            if (tutorialPanels[i] != null)
            {
                tutorialPanels[i].SetActive(false);
            }
        }


        // 첫 번째 패널 활성화
        if (tutorialPanels.Length > 0 &&
            tutorialPanels[0] != null)
        {
            tutorialPanels[0].SetActive(true);
        }


        currentPanelIndex = 0;


        // 첫 번째 텍스트 타이핑 시작
        StartTyping();
    }


    // =====================================================
    // 박스 / 모터 생성
    // =====================================================

    private void CreateDisplayObjects()
    {
        // -------------------------------------------------
        // 박스
        // -------------------------------------------------

        if (boxPrefab != null &&
            boxDisplayAnchor != null)
        {
            boxObject = Instantiate(
                boxPrefab,
                boxDisplayAnchor.position,
                boxDisplayAnchor.rotation,
                boxDisplayAnchor
            );

            boxHolder = boxObject.transform;

            boxHolder.localPosition = Vector3.zero;

            boxHolder.localRotation = Quaternion.identity;

            boxHolder.localScale = boxDisplayScale;
        }


        // -------------------------------------------------
        // 모터
        // -------------------------------------------------

        if (motorPrefab != null &&
            motorDisplayAnchor != null)
        {
            motorObject = Instantiate(
                motorPrefab,
                motorDisplayAnchor.position,
                motorDisplayAnchor.rotation,
                motorDisplayAnchor
            );

            motorHolder = motorObject.transform;

            motorHolder.localPosition = Vector3.zero;

            motorHolder.localRotation = Quaternion.identity;

            motorHolder.localScale = motorDisplayScale;
        }
    }


    // =====================================================
    // Update
    // =====================================================

    private void Update()
    {
        HandleTutorialInput();

        HandleMouseRotation();

        HandleFineRotation();
    }


    // =====================================================
    // Tutorial 우클릭 입력
    // =====================================================

    private void HandleTutorialInput()
    {
        if (Mouse.current == null)
            return;


        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleRightClick();
        }
    }


    // =====================================================
    // 우클릭 처리
    // =====================================================

    private void HandleRightClick()
    {
        // -----------------------------------------
        // 타이핑 중이면 전체 텍스트 즉시 표시
        // -----------------------------------------

        if (isTyping)
        {
            FinishTyping();
            return;
        }


        // -----------------------------------------
        // 마지막 패널이면 튜토리얼 종료
        // -----------------------------------------

        if (currentPanelIndex >= tutorialPanels.Length - 1)
        {
            GoToNextScene();
            return;
        }


        // -----------------------------------------
        // 다음 패널
        // -----------------------------------------

        NextPanel();
    }


    // =====================================================
    // 다음 패널
    // =====================================================

    private void NextPanel()
    {
        if (tutorialPanels[currentPanelIndex] != null)
        {
            tutorialPanels[currentPanelIndex].SetActive(false);
        }


        currentPanelIndex++;


        if (tutorialPanels[currentPanelIndex] != null)
        {
            tutorialPanels[currentPanelIndex].SetActive(true);
        }


        PlayPanelChangeSound();


        StartTyping();
    }


    // =====================================================
    // 타이핑 시작
    // =====================================================

    private void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);

            typingCoroutine = null;
        }


        if (tutorialTexts == null ||
            currentPanelIndex >= tutorialTexts.Length ||
            tutorialTexts[currentPanelIndex] == null)
        {
            isTyping = false;
            return;
        }


        TMP_Text currentText =
            tutorialTexts[currentPanelIndex];


        currentFullText =
            currentText.text;


        currentText.text = "";


        typingCoroutine =
            StartCoroutine(
                TypeText(
                    currentText,
                    currentFullText
                )
            );
    }


    // =====================================================
    // 타이핑 코루틴
    // =====================================================

    private IEnumerator TypeText(
        TMP_Text text,
        string fullText)
    {
        isTyping = true;


        for (int i = 0; i < fullText.Length; i++)
        {
            text.text += fullText[i];

            yield return new WaitForSeconds(
                typingSpeed
            );
        }


        isTyping = false;

        typingCoroutine = null;
    }


    // =====================================================
    // 타이핑 즉시 완료
    // =====================================================

    private void FinishTyping()
    {
        if (!isTyping)
            return;


        if (typingCoroutine != null)
        {
            StopCoroutine(
                typingCoroutine
            );

            typingCoroutine = null;
        }


        if (tutorialTexts != null &&
            currentPanelIndex < tutorialTexts.Length &&
            tutorialTexts[currentPanelIndex] != null)
        {
            tutorialTexts[currentPanelIndex].text =
                currentFullText;
        }


        isTyping = false;
    }


    // =====================================================
    // 마우스 회전
    // =====================================================

    private void HandleMouseRotation()
    {
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


        RotateObjects(
            horizontalRotation,
            verticalRotation
        );
    }


    // =====================================================
    // WASD 회전
    // =====================================================

    private void HandleFineRotation()
    {
        if (Keyboard.current == null)
            return;


        float verticalRotation = 0f;
        float horizontalRotation = 0f;


        if (Keyboard.current.wKey.isPressed)
        {
            verticalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.sKey.isPressed)
        {
            verticalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.aKey.isPressed)
        {
            horizontalRotation +=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (Keyboard.current.dKey.isPressed)
        {
            horizontalRotation -=
                fineRotationSpeed *
                Time.deltaTime;
        }


        if (verticalRotation != 0f ||
            horizontalRotation != 0f)
        {
            RotateObjects(
                horizontalRotation,
                verticalRotation
            );
        }
    }


    // =====================================================
    // 두 오브젝트 동시에 회전
    // =====================================================

    private void RotateObjects(
        float horizontalRotation,
        float verticalRotation)
    {
        RotateObject(
            boxHolder,
            horizontalRotation,
            verticalRotation
        );


        RotateObject(
            motorHolder,
            horizontalRotation,
            verticalRotation
        );
    }


    // =====================================================
    // 개별 오브젝트 회전
    // =====================================================

    private void RotateObject(
        Transform target,
        float horizontalRotation,
        float verticalRotation)
    {
        if (target == null)
            return;


        if (horizontalRotation != 0f)
        {
            target.Rotate(
                Vector3.up,
                horizontalRotation,
                Space.World
            );
        }


        if (verticalRotation != 0f)
        {
            target.Rotate(
                Vector3.left,
                verticalRotation,
                Space.World
            );
        }
    }


    // =====================================================
    // 패널 전환 효과음
    // =====================================================

    private void PlayPanelChangeSound()
    {
        if (audioSource == null)
            return;


        if (panelChangeSound == null)
            return;


        audioSource.PlayOneShot(
            panelChangeSound,
            panelSoundVolume
        );
    }


    // =====================================================
    // 튜토리얼 종료
    // =====================================================

    private void GoToNextScene()
    {
        // -------------------------------------------------
        // 씬 전환 효과음
        // -------------------------------------------------

        if (audioSource != null &&
            sceneTransitionSound != null)
        {
            audioSource.PlayOneShot(
                sceneTransitionSound,
                sceneSoundVolume
            );
        }


        // -------------------------------------------------
        // 튜토리얼 완료 처리
        // -------------------------------------------------

        if (PlayerStatus.Instance != null)
        {
            PlayerStatus.Instance.CompleteTutorial();

            Debug.Log(
                "튜토리얼 완료 처리 후 Progress로 이동합니다."
            );
        }


        // -------------------------------------------------
        // 기존 씬 전환
        // -------------------------------------------------

        if (goToThisScene != null)
        {
            goToThisScene.nextSceneButton();
        }
        else
        {
            Debug.LogWarning(
                "GoToThisScene이 연결되어 있지 않습니다."
            );
        }
    }
}