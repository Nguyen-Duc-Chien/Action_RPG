using UnityEngine;
using TMPro;
using UnityEngine.UI;        // Using this for TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator healthTextAnim;
    public Slider healthSlider;
    public ExpManager expManager;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverExpText;

    private Vector3 startPosition; 

    public void Start()
    {
        startPosition = transform.position;

        UpdateHealthUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ChangeHealth(int amount)
    {
        Debug.Log("Changing health by: " + amount);
        StatsManager.Instance.currentHealth += amount;

        if (StatsManager.Instance.currentHealth < 0)
            StatsManager.Instance.currentHealth = 0;

        if (healthTextAnim != null) healthTextAnim.Play("TextUpdate");
        UpdateHealthUI();

        if (StatsManager.Instance.currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        healthSlider.maxValue = StatsManager.Instance.maxHealth;
        healthSlider.value = StatsManager.Instance.currentHealth;
        healthText.text = "HP : " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }

    private void Die()
    {
        Debug.Log("Player has died, GameOverPanel set active!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverExpText != null)
            {
                gameOverExpText.text = "Score: " + expManager.currentExp;
            }
        }

        Time.timeScale = 0f;
    }

    public void MainMenuButton()
    {
        Debug.Log("Main Menu button clicked — restarting run or reloading scene.");
        Time.timeScale = 1f; 

        if (GameManager.Instance != null)
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            GameManager.Instance.RestartRun();
            return;
        }

        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        UpdateHealthUI();
        transform.position = startPosition;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    }

    public void ResetPlayer(Vector3 spawnPosition)
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        UpdateHealthUI();

        transform.position = spawnPosition;
        startPosition = spawnPosition;

        if (expManager != null)
            expManager.currentExp = 0;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Debug.Log("Player stats reset and teleported to Spawn Point: " + spawnPosition);
    }
}
