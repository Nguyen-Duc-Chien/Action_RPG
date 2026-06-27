using UnityEngine;

public class Torch_Combat : MonoBehaviour
{
    public float damage = 2f;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce = 5f;
    public float stunTime = 0.2f;
    public LayerMask playerLayer;

    [Header("Torch Flame Settings")]
    public float burnDuration = 2f;
    public float burnDamagePerSecond = 1f;

    public void Attack()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("TorchAttack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage); //[cite: 3]
            hits[0].GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, stunTime); //[cite: 3]

            Player_DebuffManager debuffManager = hits[0].GetComponent<Player_DebuffManager>();
            if (debuffManager != null)
            {
                debuffManager.ApplyBurnDebuff(burnDuration, burnDamagePerSecond);
            }
        }
    }
}