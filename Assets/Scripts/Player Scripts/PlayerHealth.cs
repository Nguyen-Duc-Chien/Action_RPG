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

    public void ChangeHealth(float amount)
    {
        //Debug.Log("Changing health by: " + amount);
        float targetHealth = StatsManager.Instance.currentHealth + amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(targetHealth, 0f, StatsManager.Instance.maxHealth);

        if (healthTextAnim != null) healthTextAnim.Play("TextUpdate");
        UpdateHealthUI();

        if (StatsManager.Instance.currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = StatsManager.Instance.maxHealth;
        healthSlider.value = StatsManager.Instance.currentHealth;
        healthText.text = "HP : " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;

        //Debug.Log($"[UI Check] Slider Value: {healthSlider.value}, Text: {healthText.text}", healthSlider);
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
        //Debug.Log("Main Menu button clicked — restarting run or reloading scene.");
        Time.timeScale = 1f;

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        }
        UpdateHealthUI(); 

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartRun();
            return;
        }

        transform.position = startPosition;
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
