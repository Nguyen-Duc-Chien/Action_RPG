using UnityEngine;

public class ToggleSkillTree : MonoBehaviour
{
    public CanvasGroup skillsCanvas;

    private bool skillTreeOpen = false;

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
                Time.timeScale = 0;
                skillsCanvas.alpha = 1;
                skillsCanvas.blocksRaycasts = true;
                skillTreeOpen = true;
                Debug.Log("Opened Skill Tree");
            }
        }
    }
}
