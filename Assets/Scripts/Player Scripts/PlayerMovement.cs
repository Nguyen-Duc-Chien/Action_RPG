using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int facingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockedback;
    public bool isShooting;

    public Player_Combat player_Combat;

    private void Update()
    {
        if(Input.GetButtonDown("Slash") && player_Combat.enabled == true)
        {
            player_Combat.Attack();
            //Debug.Log("Slash button pressed! Attack animation should play!");
        }
    }

    // Fixed Update is called 50x second
    void FixedUpdate()
    {
        if (isKnockedback == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal > 0 && transform.localScale.x < 0 ||
                horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");
            rb.linearVelocity = new Vector2(moveH, moveV) * StatsManager.Instance.speed;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3 (transform.localScale.x * -1, 
                                             transform.localScale.y, 
                                             transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedback = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedback = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Confiner"))
        {
            ConfinerFinder cameraFinder = FindAnyObjectByType<ConfinerFinder>();

            if (cameraFinder != null)
            {
                cameraFinder.UpdateCameraBounds(other);
            }
        }
    }
}
