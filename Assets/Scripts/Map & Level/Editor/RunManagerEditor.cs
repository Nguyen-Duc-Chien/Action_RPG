using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RunManager))]
public class RunManagerEditor : Editor
{
    private bool showLevelDebug = true;

    public override void OnInspectorGUI()
    {
        // Vẽ Inspector mặc định
        DrawDefaultInspector();

        RunManager rm = (RunManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ── Foldout cho bảng debug ──────────────────────────────────────────
        showLevelDebug = EditorGUILayout.Foldout(showLevelDebug, "Level Unlock Debug", true, EditorStyles.foldoutHeader);

        if (!showLevelDebug || rm.levelConfigs == null || rm.levelConfigs.Count == 0)
            return;

        for (int i = 0; i < rm.levelConfigs.Count; i++)
        {
            LevelConfig cfg = rm.levelConfigs[i];
            int levelNum = i + 1;

            bool isUnlocked = rm.IsLevelUnlocked(levelNum);

            EditorGUILayout.BeginHorizontal();

            // Checkbox để người dùng tự tick chọn (bật/tắt)
            bool toggleState = EditorGUILayout.Toggle(isUnlocked, GUILayout.Width(20));
            if (toggleState != isUnlocked)
            {
                PlayerPrefs.SetInt("LevelUnlocked_" + levelNum, toggleState ? 1 : 0);
                PlayerPrefs.Save();
            }

            // Hiển thị tên level
            string name = (cfg != null && !string.IsNullOrEmpty(cfg.levelName)) ? cfg.levelName : $"(Level {levelNum})";
            EditorGUILayout.LabelField($"Lv {levelNum}: {name}");

            EditorGUILayout.EndHorizontal();
        }

        // ── Nút debug ──────────────────────────────────────────────────────
        EditorGUILayout.Space(8);
        if (GUILayout.Button("Reset Progress", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Reset Progress",
                "Delete progress (levels, gold, exp, inventory, health)?", "Reset", "Cancel"))
            {
                if (Application.isPlaying && RunManager.Instance != null)
                {
                    RunManager.Instance.ResetAllProgress();
                }
                else
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("UnlockedLevel", 1);
                PlayerPrefs.Save();
                }
                Debug.LogWarning("[RunManager Editor] All progress reset!");
            }
        }

        // Repaint liên tục khi đang play để cập nhật trạng thái real-time
        if (Application.isPlaying)
            Repaint();
    }
}
