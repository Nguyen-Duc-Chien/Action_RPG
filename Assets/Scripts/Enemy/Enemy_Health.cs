using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int expReward = 3;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    public int currentHealth;
    public int maxHealth;

    private Animator anim;
    private Collider2D enemyCollider;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log("Enemy health changed by: " + amount + ", current health: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;

        if (OnMonsterDefeated != null) OnMonsterDefeated(expReward);
        if (anim != null) anim.SetTrigger("dieTrig");
        if (enemyCollider != null) enemyCollider.enabled = false;

        Enemy_Movement meleeMovement = GetComponent<Enemy_Movement>();
        if (meleeMovement != null) meleeMovement.enabled = false;

        Enemy_Ranged_Movement rangedMovement = GetComponent<Enemy_Ranged_Movement>();
        if (rangedMovement != null) rangedMovement.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(gameObject, 1f);
    }
}
