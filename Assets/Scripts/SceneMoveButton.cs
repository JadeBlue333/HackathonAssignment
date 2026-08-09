using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneName;

    public void MoveScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("이동할 Scene Name이 비어있습니다.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}