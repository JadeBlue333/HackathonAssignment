using UnityEngine;
using UnityEditor;
using TMPro;

public class ReplaceTMPFont : EditorWindow
{
    private TMP_FontAsset oldFont;
    private TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Font")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceTMPFont>("Replace TMP Font");
    }

    private void OnGUI()
    {
        GUILayout.Label("TMP Font 일괄 교체", EditorStyles.boldLabel);

        oldFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "기존 Font",
            oldFont,
            typeof(TMP_FontAsset),
            false
        );

        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "새 Font",
            newFont,
            typeof(TMP_FontAsset),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("현재 씬 전체 폰트 교체"))
        {
            ReplaceInCurrentScene();
        }
    }

    private void ReplaceInCurrentScene()
    {
        if (oldFont == null || newFont == null)
        {
            Debug.LogWarning("기존 Font와 새 Font를 모두 지정하세요.");
            return;
        }

        TMP_Text[] texts = FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        int count = 0;

        foreach (TMP_Text text in texts)
        {
            if (text.font == oldFont)
            {
                Undo.RecordObject(text, "Replace TMP Font");

                text.font = newFont;

                EditorUtility.SetDirty(text);

                count++;
            }
        }

        Debug.Log($"폰트 교체 완료: {count}개");
    }
}