using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 25f;          // Speed during dash
    public float dashDuration = 0.15f;     // How long the dash lasts (very short)
    public float dashCooldown = 0.8f;      // Cooldown between dashes
    public float dashEnergyCost = 30f;     // Energy cost per dash

    [Header("References")]
    public Rigidbody2D rb;
    public PlayerMovement playerMovement;
    public EnergyManager energyManager;
    public Animator anim;

    private float cooldownTimer = 0f;
    private bool isDashing = false;

    // Layer indices for phase-through
    private int playerLayer;
    private int enemyLayer;

    private void Start()
    {
        playerLayer = gameObject.layer;
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void Update()
    {
        // Countdown cooldown
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Listen for Dash input
        if (Input.GetButtonDown("Dash"))
        {
            TryDash();
        }
    }

    /// <summary>
    /// Attempt to perform a dash. Can also be called from a UI Button.
    /// </summary>
    public void TryDash()
    {
        if (isDashing) return;
        if (cooldownTimer > 0f) return;

        // Block dash when knocked back
        if (playerMovement != null && playerMovement.isKnockedback) return;

        // Check energy
        if (energyManager != null && !energyManager.UseEnergy(dashEnergyCost)) return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("PlayerDash");

        // Tell PlayerMovement to stop normal movement
        if (playerMovement != null)
        {
            playerMovement.isDashing = true;
        }

        // Calculate dash direction: use input direction, fall back to facing direction
        Vector2 dashDirection = GetDashDirection();

        // --- Enable i-frames ---
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.isInvincible = true;
        }

        // --- Enable phase-through enemies ---
        if (enemyLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }

        // --- Trigger dash animation (if available) ---
        if (anim != null)
        {
            anim.SetBool("isDashing", true);
        }

        // --- Apply dash velocity ---
        rb.linearVelocity = dashDirection * dashSpeed;

        // --- Wait for dash duration ---
        yield return new WaitForSeconds(dashDuration);

        // --- End dash ---
        rb.linearVelocity = Vector2.zero;

        // Disable i-frames
        if (playerHealth != null)
        {
            playerHealth.isInvincible = false;
        }

        // Disable phase-through
        if (enemyLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }

        // Stop dash animation
        if (anim != null)
        {
            anim.SetBool("isDashing", false);
        }

        // Restore movement
        if (playerMovement != null)
        {
            playerMovement.isDashing = false;
        }

        isDashing = false;

        // Start cooldown
        cooldownTimer = dashCooldown;
    }

    /// <summary>
    /// Determine the dash direction from current input, falling back to facing direction.
    /// </summary>
    private Vector2 GetDashDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(h, v);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            return inputDir.normalized;
        }

        // No input — use facing direction (horizontal only)
        if (playerMovement != null)
        {
            return new Vector2(playerMovement.facingDirection, 0f).normalized;
        }

        return Vector2.right;
    }
}
