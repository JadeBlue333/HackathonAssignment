using UnityEngine;
using UnityEngine.UI;

public class HumanPartVisualChanger : MonoBehaviour
{
    // =========================================================
    // Head
    // =========================================================

    [Header("Head")]

    [SerializeField]
    private Image headTargetImage;

    [Tooltip("머리 미보유 상태 이미지")]
    [SerializeField]
    private Sprite headDefaultSprite;

    [Tooltip("머리 보유 상태 이미지")]
    [SerializeField]
    private Sprite headOwnedSprite;


    // =========================================================
    // Body
    // =========================================================

    [Header("Body")]

    [SerializeField]
    private Image bodyTargetImage;

    [Tooltip("몸 미보유 상태 이미지")]
    [SerializeField]
    private Sprite bodyDefaultSprite;

    [Tooltip("몸 보유 상태 이미지")]
    [SerializeField]
    private Sprite bodyOwnedSprite;


    // =========================================================
    // Heart
    // =========================================================

    [Header("Heart")]

    [SerializeField]
    private Image heartTargetImage;

    [Tooltip("심장 미보유 상태 이미지")]
    [SerializeField]
    private Sprite heartDefaultSprite;

    [Tooltip("심장 보유 상태 이미지")]
    [SerializeField]
    private Sprite heartOwnedSprite;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        Refresh();
    }


    // =========================================================
    // OnEnable
    // =========================================================

    private void OnEnable()
    {
        Refresh();
    }


    // =========================================================
    // Refresh All
    // =========================================================

    public void Refresh()
    {
        if (PlayerStatus.Instance == null)
            return;


        RefreshHead();

        RefreshBody();

        RefreshHeart();
    }


    // =========================================================
    // Head
    // =========================================================

    private void RefreshHead()
    {
        if (headTargetImage == null)
            return;


        if (PlayerStatus.Instance.humanHead)
        {
            if (headOwnedSprite != null)
            {
                headTargetImage.sprite =
                    headOwnedSprite;
            }
        }
        else
        {
            if (headDefaultSprite != null)
            {
                headTargetImage.sprite =
                    headDefaultSprite;
            }
        }
    }


    // =========================================================
    // Body
    // =========================================================

    private void RefreshBody()
    {
        if (bodyTargetImage == null)
            return;


        if (PlayerStatus.Instance.humanBody)
        {
            if (bodyOwnedSprite != null)
            {
                bodyTargetImage.sprite =
                    bodyOwnedSprite;
            }
        }
        else
        {
            if (bodyDefaultSprite != null)
            {
                bodyTargetImage.sprite =
                    bodyDefaultSprite;
            }
        }
    }


    // =========================================================
    // Heart
    // =========================================================

    private void RefreshHeart()
    {
        if (heartTargetImage == null)
            return;


        if (PlayerStatus.Instance.humanHeart)
        {
            if (heartOwnedSprite != null)
            {
                heartTargetImage.sprite =
                    heartOwnedSprite;
            }
        }
        else
        {
            if (heartDefaultSprite != null)
            {
                heartTargetImage.sprite =
                    heartDefaultSprite;
            }
        }
    }
}