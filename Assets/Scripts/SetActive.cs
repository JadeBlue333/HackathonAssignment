using UnityEngine;

public class SetActive : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject targetObject;

    /// <summary>
    /// 체크 여부에 따라 SetActive를 설정하는 함수
    /// </summary>
    /// <param name="isActive">true면 활성화, false면 비활성화</param>
    public void SetObjectActive(bool isActive)
    {
        if (targetObject != null)
            targetObject.SetActive(isActive);
    }

    /*
    
    //지금은 불필요한 부분일듯

    /// <summary>
    /// 활성화
    /// </summary>
    public void SetTrue()
    {
        SetObjectActive(true);
    }

    /// <summary>
    /// 비활성화
    /// </summary>
    public void SetFalse()
    {
        SetObjectActive(false);
    }
    */
}