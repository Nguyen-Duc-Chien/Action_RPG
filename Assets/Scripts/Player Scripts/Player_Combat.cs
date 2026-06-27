using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;
    //public StatsUI statsUI;
    public Animator anim;

    public float cooldown;
    private float timer;

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
    public void Attack()
    {
        if(timer <= 0)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("PlayerMelee");
            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
    }

    public void DealDamage()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        int dir = (movement != null) ? movement.facingDirection : 1;

        Vector3 attackPosition = transform.position + new Vector3(0.5f * dir, 0f, 0f);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition, StatsManager.Instance.weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.meleeDamage);
            enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
        }
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    private void OnDrawGizmos()
    {
        if (StatsManager.Instance == null) return;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        int dir = (movement != null) ? movement.facingDirection : 1;

        Vector3 attackPosition = transform.position + new Vector3(0.5f * dir, 0f, 0f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPosition, StatsManager.Instance.weaponRange);
    }
}
