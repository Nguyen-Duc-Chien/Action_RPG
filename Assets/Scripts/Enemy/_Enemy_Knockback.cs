using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Torch_Movement torchMovement;
    private Archer_Movement archerMovement;
    private Barrel_Suicide barielSuicide;
    private Frostbite_Archer_Movement frostbiteArcherMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        torchMovement = GetComponent<Torch_Movement>();
        archerMovement = GetComponent<Archer_Movement>();
        barielSuicide = GetComponent<Barrel_Suicide>();
        frostbiteArcherMovement = GetComponent<Frostbite_Archer_Movement>();
    }

    public void Knockback(Transform forceTransform, float knockbackForce, float knockbackRange, float stunTime)
    {
        if (torchMovement != null) torchMovement.ChangeState(EnemyState.Knockback);
        if (archerMovement != null) archerMovement.ChangeState(EnemyState.Knockback);
        if (barielSuicide != null) barielSuicide.ChangeState(BarrelState.Knockback);
        if (frostbiteArcherMovement != null) frostbiteArcherMovement.ChangeState(EnemyState.Knockback);

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

        if (torchMovement != null) torchMovement.ChangeState(EnemyState.Idle);
        if (archerMovement != null) archerMovement.ChangeState(EnemyState.Idle);
        if (barielSuicide != null) barielSuicide.ChangeState(BarrelState.Awakening);
        if (frostbiteArcherMovement != null) frostbiteArcherMovement.ChangeState(EnemyState.Idle);
    }
}
