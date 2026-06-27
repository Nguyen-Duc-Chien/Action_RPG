using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public int facingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

    [HideInInspector] public bool isKnockedback;
    public bool isShooting;
    [HideInInspector] public bool isDashing;

    public Player_Combat player_Combat;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && player_Combat.enabled == true)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);

            player_Combat.Attack();
            //Debug.Log("Left Mouse pressed! Attack animation should play!");
        }
    }

    // Fixed Update is called 50x second
    void FixedUpdate()
    {
        if (isKnockedback == false && !isDashing)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (isShooting == false)
            {
                if (horizontal > 0 && transform.localScale.x < 0 ||
                    horizontal < 0 && transform.localScale.x > 0)
                {
                    Flip();
                }
            }

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");

            float currentSpeed = StatsManager.Instance.speed;

            if (isShooting)
            {
                currentSpeed *= 0.3f;
            }

            Vector2 moveDirection = new Vector2(moveH, moveV);

            if (moveDirection.magnitude > 1f)
            {
                moveDirection = moveDirection.normalized;
            }

            rb.linearVelocity = moveDirection * currentSpeed;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3 (transform.localScale.x * -1, 
                                             transform.localScale.y, 
                                             transform.localScale.z);
    }

    public void ForceFlip(int direction)
    {
        if (direction != facingDirection)
        {
            facingDirection = direction;
            transform.localScale = new Vector3(transform.localScale.x * -1,
                                                 transform.localScale.y,
                                                 transform.localScale.z);
        }
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Confiner"))
        {
            // Đảm bảo tâm của người chơi thực sự nằm trong ranh giới phòng mới
            // Ngăn chặn trường hợp Trigger phụ (tầm đánh, tương tác) chạm nhầm vào phòng bên cạnh
            if (other.OverlapPoint(transform.position))
            {
                ConfinerFinder cameraFinder = FindAnyObjectByType<ConfinerFinder>();

                if (cameraFinder != null)
                {
                    cameraFinder.UpdateCameraBounds(other);
                }
            }
        }
    }
}
