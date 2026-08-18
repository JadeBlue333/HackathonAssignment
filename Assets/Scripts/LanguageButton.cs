using UnityEngine;
using TMPro;

public class LanguageButton : MonoBehaviour
{
    // =========================================================
    // Language Button Texts
    // =========================================================

    [Header("Korean Canvas Button Texts")]

    [SerializeField]
    private TMP_Text koreanTextKR;

    [SerializeField]
    private TMP_Text englishTextKR;


    [Header("English Canvas Button Texts")]

    [SerializeField]
    private TMP_Text koreanTextEN;

    [SerializeField]
    private TMP_Text englishTextEN;


    // =========================================================
    // Text Colors
    // =========================================================

    [Header("Text Colors")]

    [Tooltip("현재 선택된 언어 텍스트 색상")]
    [SerializeField]
    private Color selectedColor =
        new Color(1f, 1f, 1f, 1f);

    [Tooltip("선택되지 않은 언어 텍스트 색상")]
    [SerializeField]
    private Color unselectedColor =
        new Color(1f, 1f, 1f, 0.35f);


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        UpdateLanguageTexts();
    }

    public void SelectKorean()
    {
        LanguageManager.Instance.SetKorean();
        UpdateLanguageTexts();
    }

    public void SelectEnglish()
    {
        LanguageManager.Instance.SetEnglish();
        UpdateLanguageTexts();
    }

    // =========================================================
    // Update Language Texts
    // =========================================================

    public void UpdateLanguageTexts()
    {
        bool isEnglish = LanguageManager.Instance.isEnglish;

        if (isEnglish)
        {
            // -----------------------------------------------
            // 영어 선택
            // -----------------------------------------------

            if (koreanTextKR != null)
                koreanTextKR.color = unselectedColor;

            if (englishTextKR != null)
                englishTextKR.color = selectedColor;

            if (koreanTextEN != null)
                koreanTextEN.color = unselectedColor;

            if (englishTextEN != null)
                englishTextEN.color = selectedColor;
        }
        else
        {
            // -----------------------------------------------
            // 한국어 선택
            // -----------------------------------------------

            if (koreanTextKR != null)
                koreanTextKR.color = selectedColor;

            if (englishTextKR != null)
                englishTextKR.color = unselectedColor;

            if (koreanTextEN != null)
                koreanTextEN.color = selectedColor;

            if (englishTextEN != null)
                englishTextEN.color = unselectedColor;
        }
    }
}