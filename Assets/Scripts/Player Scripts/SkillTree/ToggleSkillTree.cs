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
        skillTreeOpen = false;
    }
    public void Update()
    {
        if(Input.GetButtonDown("ToggleSkillTree"))
        {
            if(skillTreeOpen)
            {
                Time.timeScale = 1;
                skillsCanvas.alpha = 0;
                skillsCanvas.blocksRaycasts = false;
                skillTreeOpen = false;
                Debug.Log("Closed Skill Tree");
            }
            else if (!skillTreeOpen)
            {
                if (statsUI != null && statsUI.IsStatsOpen)
                {
                    Debug.Log("Cannot open Skill Tree while Stats UI is open!");
                    return;
                }

                // Open Skill Tree -> Stop game
                Time.timeScale = 0;
                skillsCanvas.alpha = 1;
                skillsCanvas.blocksRaycasts = true;
                skillTreeOpen = true;
                Debug.Log("Opened Skill Tree");
            }
        }
    }
}
