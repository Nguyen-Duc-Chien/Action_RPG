using UnityEngine;
using TMPro; // Using this for TextMeshPro

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;
    // Reference to the ToggleSkillTree script to check if the skill tree is open
    public ToggleSkillTree toggleSkillTree;

    private bool statsOpen = false;

    // Getter to check if the stats UI is currently open
    public bool IsStatsOpen => statsOpen;

    public void Start()
    {
        UpdateAllStats();
        statsCanvas.alpha = 0; // Start with stats UI hidden
        statsCanvas.blocksRaycasts = false; // Disable interaction when closed
        statsOpen = false;
    }

    public void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
            if (statsOpen)
            {// Close stats UI
                Time.timeScale = 1;
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false; // Disable interaction when closed
                statsOpen = false;
                Debug.Log("Stats UI closed!");
            }
            else
            {
                if (toggleSkillTree != null && toggleSkillTree.IsSkillTreeOpen)
                {
                    Debug.Log("Cannot open Stats UI while Skill Tree is open!");
                    return;
                }

                // Open stats UI -> Stop the game
                Time.timeScale = 0;
                UpdateAllStats();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsOpen = true;
                Debug.Log("Stats UI opened!");
            }
    }

    public void UpdateHealth()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Health: " + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;
    }

    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdateMeleeDamage()
    {
        statsSlots[2].GetComponentInChildren<TMP_Text>().text = "Melee Damage: " + StatsManager.Instance.meleeDamage;
    }

    public void UpdateRangeDamage()
    {
        statsSlots[3].GetComponentInChildren<TMP_Text>().text = "Range Damage: " + StatsManager.Instance.rangeDamage;
    }

    public void UpdateAllStats()
    {
        UpdateHealth();
        UpdateSpeed();

        UpdateMeleeDamage();
        UpdateRangeDamage();
    }
}
