using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CeoDialogue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue")]
    [SerializeField] private List<string> lines = new();

    [Header("Choice")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;

    [Header("Choice Text")]
    [SerializeField] private string choice1Text = "선택지 1";
    [SerializeField] private string choice2Text = "선택지 2";

    [Header("Last Dialogue")]
    [TextArea]
    [SerializeField] private string lastDialogue1;

    [TextArea]
    [SerializeField] private string lastDialogue2;

    [Header("Scene")]
    [SerializeField] private GoToThisScene goToThisScene;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    private int currentLine = 0;

    private bool isTyping = false;
    private bool dialogueFinished = false;
    private bool choiceSelected = false;

    private string selectedLastDialogue;
    private GoToThisScene selectedScene;

    private Coroutine typingCoroutine;


    private void Start()
    {
        // 선택지 패널 숨기기
        if (choicePanel != null)
            choicePanel.SetActive(false);

        // 버튼 연결
        if (choiceButton1 != null)
            choiceButton1.onClick.AddListener(SelectChoice1);

        if (choiceButton2 != null)
            choiceButton2.onClick.AddListener(SelectChoice2);

        // 첫 대사 출력
        if (lines.Count > 0)
        {
            ShowLine();
        }
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleSpace();
        }
    }


    private void HandleSpace()
    {
        // 타이핑 중이면 → 타이핑 즉시 완료
        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        // 아직 일반 대사가 남아있으면 → 다음 대사
        if (!dialogueFinished)
        {
            if (currentLine < lines.Count - 1)
            {
                currentLine++;
                ShowLine();
            }
            else
            {
                // 일반 대사를 모두 출력했으면 선택지 표시
                dialogueFinished = true;
                ShowChoices();
            }

            return;
        }

        // 선택지를 골랐고 마지막 대사까지 출력했다면
        if (choiceSelected)
        {
            Debug.Log("마지막 대사 이후 Space 입력됨");

            if (goToThisScene != null)
            {
                Debug.Log("현재 씬 이름: " + goToThisScene.sceneName);

                goToThisScene.nextSceneButton();
            }
            else
            {
                Debug.LogError("goToThisScene이 연결되지 않았음!");
            }
        }
    }


    private void ShowLine()
    {
        if (dialogueText == null)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(lines[currentLine]));
    }


    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i + 1;

            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueText.maxVisibleCharacters = text.Length;

        isTyping = false;
        typingCoroutine = null;
    }


    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.maxVisibleCharacters = dialogueText.text.Length;

        isTyping = false;
    }


    private void ShowChoices()
    {
        if (choicePanel != null)
            choicePanel.SetActive(true);

        if (choiceButton1 != null)
        {
            choiceButton1.gameObject.SetActive(true);

            TMP_Text buttonText = choiceButton1.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = choice1Text;
        }

        if (choiceButton2 != null)
        {
            choiceButton2.gameObject.SetActive(true);

            TMP_Text buttonText = choiceButton2.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = choice2Text;
        }
    }


    public void SelectChoice1()
    {
        if (choiceSelected)
            return;

        choiceSelected = true;

        selectedLastDialogue = lastDialogue1;
        goToThisScene.sceneName = "Ending1";

        HideChoices();

        ShowLastDialogue();
    }


    public void SelectChoice2()
    {
        if (choiceSelected)
            return;

        choiceSelected = true;

        selectedLastDialogue = lastDialogue2;

        if (PlayerStatus.Instance.isHuman)
            goToThisScene.sceneName = "Date";
        else
            goToThisScene.sceneName = "Ending2";

        HideChoices();

        ShowLastDialogue();
    }


    private void HideChoices()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }


    private void ShowLastDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(selectedLastDialogue));
    }


    private void OnDestroy()
    {
        if (choiceButton1 != null)
            choiceButton1.onClick.RemoveListener(SelectChoice1);

        if (choiceButton2 != null)
            choiceButton2.onClick.RemoveListener(SelectChoice2);
    }
}