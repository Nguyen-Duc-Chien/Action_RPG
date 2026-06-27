using UnityEngine;

public class ToggleSkillTree : MonoBehaviour
{
    public CanvasGroup skillsCanvas;
    // Reference to the StatsUI script to check if the stats UI is open
    public StatsUI statsUI;

    private bool skillTreeOpen = false;

    // Getter to check if the skill tree is currently open
    public bool IsSkillTreeOpen => skillTreeOpen;

    public void Start()
    {
        EnsureCanvasRef();
        if (skillsCanvas != null)
        {
            skillsCanvas.alpha = 0;
            skillsCanvas.blocksRaycasts = false;
            skillsCanvas.interactable = false;
        }
        skillTreeOpen = false;
    }

    public void OpenPanel()
    {
        if (skillTreeOpen) return;

        if (statsUI != null && statsUI.IsStatsOpen)
        {
            Debug.LogWarning("[ToggleSkillTree] Cannot open — StatsUI is currently open! Close Stats first.");
            return;
        }

        EnsureCanvasRef();
        if (skillsCanvas == null)
        {
            Debug.LogError("[ToggleSkillTree] skillsCanvas is missing or destroyed! Cannot open.");
            return;
        }

        // Open Skill Tree -> Stop game
        Time.timeScale = 0;
        skillsCanvas.alpha = 1;
        skillsCanvas.blocksRaycasts = true;
        skillsCanvas.interactable = true;
        skillTreeOpen = true;
    }

    public void ClosePanel()
    {
        if (!skillTreeOpen) return;

        Time.timeScale = 1;

        EnsureCanvasRef();
        if (skillsCanvas != null)
        {
            skillsCanvas.alpha = 0;
            skillsCanvas.blocksRaycasts = false;
            skillsCanvas.interactable = false;
        }

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
        skillTreeOpen = false;
    }

    /// <summary>
    /// Re-tìm CanvasGroup nếu reference cũ đã bị destroy (sau scene reload).
    /// Ưu tiên tìm qua SkillTreeManager (vì nó nằm trên cùng Canvas chứa buttons).
    /// </summary>
    private void EnsureCanvasRef()
    {
        // Reference vẫn còn sống → không cần tìm
        if (skillsCanvas != null) return;

        // 1. Tìm qua SkillTreeManager — vì nó nằm cùng Canvas chứa skill buttons
        SkillTreeManager stm = FindAnyObjectByType<SkillTreeManager>(FindObjectsInactive.Include);
        if (stm != null)
        {
            skillsCanvas = stm.GetComponentInParent<CanvasGroup>();
            if (skillsCanvas == null)
                skillsCanvas = stm.GetComponent<CanvasGroup>();
        }

        // 2. Fallback: tìm trên chính hierarchy của ToggleSkillTree
        if (skillsCanvas == null)
            skillsCanvas = GetComponent<CanvasGroup>();
        if (skillsCanvas == null)
            skillsCanvas = GetComponentInChildren<CanvasGroup>(true);
        if (skillsCanvas == null)
            skillsCanvas = GetComponentInParent<CanvasGroup>();

        if (skillsCanvas != null)
            Debug.Log($"[ToggleSkillTree] CanvasGroup re-found on: {skillsCanvas.gameObject.name}");
        else
            Debug.LogError("[ToggleSkillTree] Cannot find CanvasGroup anywhere!");

        // Cũng re-find StatsUI nếu bị mất
        if (statsUI == null)
            statsUI = FindAnyObjectByType<StatsUI>(FindObjectsInactive.Include);
    }
}
