using UnityEngine;
using TMPro;
using UnityEngine.UI;        // Using this for TextMeshPro


public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator healthTextAnim;
    public Slider healthSlider; 

    public void Start()
    {
        //healthText.text = "HP : " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        UpdateHealthUI();
    }

    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        healthTextAnim.Play("TextUpdate");
        UpdateHealthUI();

        if (StatsManager.Instance.currentHealth <= 0)
        {
            gameObject.SetActive(false);

            /*
            // Optionally, you can add more logic here for when the player dies
            transform.position = new Vector3(0, 10, 0);
            // Move player to a safe position
            currentHealth = maxHealth;
            healthText.text = "HP : " + currentHealth + " / " + maxHealth;
            gameObject.SetActive(true);
            */
        }
    }

    public void UpdateHealthUI()
    {
        healthSlider.maxValue = StatsManager.Instance.maxHealth;
        healthSlider.value = StatsManager.Instance.currentHealth;
        healthText.text = "HP : " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }
}
