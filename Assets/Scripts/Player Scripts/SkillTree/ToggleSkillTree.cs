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
        skillsCanvas.alpha = 0;
        skillsCanvas.blocksRaycasts = false;
        skillsCanvas.interactable = false;
        skillTreeOpen = false;
    }
    public void OpenPanel()
    {
        if (skillTreeOpen) return;

        if (statsUI != null && statsUI.IsStatsOpen)
        {
            // Không mở nếu bảng Stats đang mở
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
        skillsCanvas.alpha = 0;
        skillsCanvas.blocksRaycasts = false;
        skillsCanvas.interactable = false;
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
        skillTreeOpen = false;
    }
}
