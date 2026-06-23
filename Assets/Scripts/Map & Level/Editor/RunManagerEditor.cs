using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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
        showLevelDebug = EditorGUILayout.Foldout(showLevelDebug, "🔍 Level Debug Info", true, EditorStyles.foldoutHeader);

        if (!showLevelDebug || rm.levelConfigs == null || rm.levelConfigs.Count == 0)
            return;

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Level", EditorStyles.boldLabel, GUILayout.Width(50));
        EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(120));
        EditorGUILayout.LabelField("Requires", EditorStyles.boldLabel, GUILayout.Width(100));
        EditorGUILayout.LabelField("Unlocked", EditorStyles.boldLabel, GUILayout.Width(70));
        EditorGUILayout.LabelField("Completed", EditorStyles.boldLabel, GUILayout.Width(80));
        EditorGUILayout.LabelField("Boss", EditorStyles.boldLabel, GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < rm.levelConfigs.Count; i++)
        {
            LevelConfig cfg = rm.levelConfigs[i];
            int levelNum = i + 1;

            bool isUnlocked  = rm.unlockAllLevels || levelNum <= unlockedLevel;
            bool isCompleted = PlayerPrefs.GetInt("CompletedLevel_" + levelNum, 0) == 1;

            // Màu nền theo trạng thái
            Color bgColor;
            if (isCompleted)
                bgColor = new Color(0.3f, 0.7f, 0.3f, 0.15f); // xanh lá nhạt
            else if (isUnlocked)
                bgColor = new Color(0.3f, 0.5f, 0.9f, 0.15f); // xanh dương nhạt
            else
                bgColor = new Color(0.5f, 0.5f, 0.5f, 0.10f); // xám

            Rect rect = EditorGUILayout.BeginHorizontal();
            EditorGUI.DrawRect(rect, bgColor);

            // Level number
            EditorGUILayout.LabelField($"{levelNum}", GUILayout.Width(50));

            // Name
            string name = (cfg != null) ? cfg.levelName : "(null)";
            EditorGUILayout.LabelField(name, GUILayout.Width(120));

            // Requires (level trước)
            string requires = (levelNum == 1) ? "—" : $"Level {levelNum - 1}";
            EditorGUILayout.LabelField(requires, GUILayout.Width(100));

            // Unlocked
            GUIStyle unlockStyle = new GUIStyle(EditorStyles.label);
            unlockStyle.normal.textColor = isUnlocked ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.2f, 0.2f);
            EditorGUILayout.LabelField(isUnlocked ? "✔ Yes" : "✘ No", unlockStyle, GUILayout.Width(70));

            // Completed
            GUIStyle completeStyle = new GUIStyle(EditorStyles.label);
            completeStyle.normal.textColor = isCompleted ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.6f, 0.6f, 0.6f);
            EditorGUILayout.LabelField(isCompleted ? "✔ Done" : "—", completeStyle, GUILayout.Width(80));

            // Boss
            if (cfg != null && cfg.hasBoss)
                EditorGUILayout.LabelField("👑", GUILayout.Width(40));
            else
                EditorGUILayout.LabelField("", GUILayout.Width(40));

            EditorGUILayout.EndHorizontal();
        }

        // ── Nút debug ──────────────────────────────────────────────────────
        EditorGUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("🔓 Unlock All (PlayerPrefs)", GUILayout.Height(25)))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 20);
            PlayerPrefs.Save();
            Debug.LogWarning("[RunManager Editor] All 20 levels unlocked in PlayerPrefs!");
        }

        if (GUILayout.Button("🔄 Reset Progress", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Reset Progress",
                "Xóa toàn bộ progress (levels, gold, exp, inventory)?", "Reset", "Cancel"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("UnlockedLevel", 1);
                PlayerPrefs.Save();
                Debug.LogWarning("[RunManager Editor] All progress reset!");
            }
        }

        EditorGUILayout.EndHorizontal();

        // Repaint liên tục khi đang play để cập nhật trạng thái real-time
        if (Application.isPlaying)
            Repaint();
    }
}
