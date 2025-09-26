using UnityEngine;
using TMPro;        // Using this for TextMeshPro


public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public Animator healthTextAnim;

    public void Start()
    {
        healthText.text = "HP : " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        healthText.text = "HP : " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        healthTextAnim.Play("TextUpdate");

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
}
