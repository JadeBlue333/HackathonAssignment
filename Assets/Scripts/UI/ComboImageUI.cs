using UnityEngine;
using UnityEngine.UI;

public class ComboImageUI : MonoBehaviour
{
    // =========================================================
    // Target
    // =========================================================

    [Header("Target")]

    [Tooltip("콤보 상태에 따라 이미지가 변경될 UI Image")]
    [SerializeField]
    private Image targetImage;


    // =========================================================
    // Combo Sprites
    // =========================================================

    [Header("Combo Sprites")]

    [Tooltip("0콤보일 때 기본 이미지")]
    [SerializeField]
    private Sprite defaultSprite;

    [Tooltip("1콤보일 때 이미지")]
    [SerializeField]
    private Sprite combo1Sprite;

    [Tooltip("2콤보일 때 이미지")]
    [SerializeField]
    private Sprite combo2Sprite;

    [Tooltip("3콤보 이상일 때 이미지")]
    [SerializeField]
    private Sprite combo3Sprite;


    // =========================================================
    // Runtime
    // =========================================================

    private int lastCombo = -1;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        Refresh();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        int currentCombo =
            PlayerStatus.Instance.comboNumber;


        // 콤보가 바뀌었을 때만 이미지 갱신
        if (currentCombo != lastCombo)
        {
            Refresh();
        }
    }


    // =========================================================
    // Refresh
    // =========================================================

    public void Refresh()
    {
        if (PlayerStatus.Instance == null)
            return;

        if (targetImage == null)
            return;


        int combo =
            PlayerStatus.Instance.comboNumber;


        lastCombo =
            combo;


        // =====================================================
        // 0 Combo
        // =====================================================

        if (combo <= 0)
        {
            if (defaultSprite != null)
            {
                targetImage.sprite =
                    defaultSprite;
            }

            return;
        }


        // =====================================================
        // 1 Combo
        // =====================================================

        if (combo == 1)
        {
            if (combo1Sprite != null)
            {
                targetImage.sprite =
                    combo1Sprite;
            }

            return;
        }


        // =====================================================
        // 2 Combo
        // =====================================================

        if (combo == 2)
        {
            if (combo2Sprite != null)
            {
                targetImage.sprite =
                    combo2Sprite;
            }

            return;
        }


        // =====================================================
        // 3 Combo 이상
        // =====================================================

        if (combo3Sprite != null)
        {
            targetImage.sprite =
                combo3Sprite;
        }
    }
}