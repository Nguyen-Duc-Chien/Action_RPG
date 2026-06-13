using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement meleeMovement;
    private Enemy_Ranged_Movement rangedMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeMovement = GetComponent<Enemy_Movement>();
        rangedMovement = GetComponent<Enemy_Ranged_Movement>();
    }

    public void Knockback(Transform forceTransform, float knockbackForce, float knockbackRange, float stunTime)
    {
        if (meleeMovement != null) meleeMovement.ChangeState(EnemyState.Knockback);
        if (rangedMovement != null) rangedMovement.ChangeState(EnemyState.Knockback);

        StartCoroutine(StunTimer(knockbackRange, stunTime)); 
        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.linearVelocity = direction * knockbackForce;
        //Debug.Log("Knockback applied.");
    }

    IEnumerator StunTimer(float knockbackRange, float stunTime)
    {
        yield return new WaitForSeconds(knockbackRange);
        rb.linearVelocity = Vector2.zero; 
        yield return new WaitForSeconds(stunTime);

        if (meleeMovement != null) meleeMovement.ChangeState(EnemyState.Idle);
        if (rangedMovement != null) rangedMovement.ChangeState(EnemyState.Idle);
    }
}
