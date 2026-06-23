using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance { get; private set; }

    [Header("UI Components")]
    public Slider healthSlider;
    public TMP_Text healthText;
    public Animator healthTextAnim;
    public PlayerHealth playerHealth;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (healthSlider != null)
        {
            healthSlider.interactable = false;
        }
    }


    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        healthText.text = "HP : " + Mathf.RoundToInt(currentHealth) + " / " + maxHealth;

        healthTextAnim.Play("TextUpdate", -1, 0f); 
    }
}