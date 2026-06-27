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
        statsCanvas.interactable = false;
        statsOpen = false;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (statsOpen)
            {
                CloseStatsUI();
            }
            else
            {
                if (toggleSkillTree != null && toggleSkillTree.IsSkillTreeOpen)
                {
                    //Debug.Log("Cannot open Stats UI while Skill Tree is open!");
                    return;
                }

                // Prevent opening if the game is already paused by another panel (like Minimap or Pause Menu)
                if (Time.timeScale == 0f) return;

                // Open stats UI -> Stop the game
                Time.timeScale = 0;
                UpdateAllStats();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsCanvas.interactable = true;
                statsOpen = true;
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("UIClick");
                //Debug.Log("Stats UI opened!");
            }
        }
    }

    public void CloseStatsUI()
    {
        if (statsOpen)
        {
            Time.timeScale = 1;
            UpdateAllStats();
            statsCanvas.alpha = 0;
            statsCanvas.blocksRaycasts = false; // Disable interaction when closed
            statsCanvas.interactable = false;
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
            statsOpen = false;
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("UIClick");
            //Debug.Log("Stats UI closed!");
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

    // Sự kiện cho thanh Slider điều chỉnh âm thanh
    public void OnVolumeChanged(float volume)
    {
        // Thay đổi âm lượng tổng của toàn bộ game (0.0 đến 1.0)
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    // Sự kiện cho nút Main Menu trong Stats Panel
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Bỏ pause game trước khi chuyển scene
        
        // Cố gắng tìm PlayerHealth để dùng hàm reset toàn diện như khi chết
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.MainMenuButton();
        }
        else
        {
            // Nếu không tìm thấy thì cứ load thẳng
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        CloseStatsUI();
    }
}
