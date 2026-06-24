using UnityEngine;

public class Pawn_Combat : MonoBehaviour
{
    public float damage = 2f;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce = 4f;
    public float stunTime = 0.15f;
    public LayerMask playerLayer;

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
            hits[0].GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, stunTime);
        }
    }
}
