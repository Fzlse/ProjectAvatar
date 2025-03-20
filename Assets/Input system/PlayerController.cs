using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public LayerMask groundLayer;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;  // Prefab peluru
    public Transform firePoint;      // Titik keluarnya peluru
    public float bulletSpeed = 10f;  // Kecepatan peluru
    public float fireRate = 0.5f;    // Jeda antar tembakan

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing = false;
    private float nextFireTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
        HandleShooting();
    }

    void HandleMovement()
    {
        if (isDashing) return;

        float moveDirection = 1;
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);

        if (isGrounded)
        {
            canDash = true;
        }

        if (PlayerInputHandler.Instance.JumpTriggered && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        if (PlayerInputHandler.Instance.DashTriggered && !isGrounded && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float dashDirection = Mathf.Sign(rb.velocity.x);
        if (dashDirection == 0) dashDirection = 1;

        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void HandleShooting()
    {
        if (PlayerInputHandler.Instance.ShootTriggered && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;  // Atur jeda tembakan
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = new Vector2(bulletSpeed, 0f); // Peluru bergerak ke kanan

        // Mencegah bullet menembus dengan Continuous Collision Detection
        bulletRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
}
