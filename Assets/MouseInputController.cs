using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class MouseInputController : MonoBehaviour
{
    [Header("Thresholds")]
    public float pitchUpThreshold = 0.5f; // Mouse movement up threshold
    public float pitchDownThreshold = -0.5f; // Mouse movement down threshold
    public float rollRightThreshold = 0.5f; // Mouse movement right threshold

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Ensure Rigidbody2D is attached
    }

    void Update()
    {
        ReadMouseMovement();
        HandleJump();
        HandleDash();
    }

    void ReadMouseMovement()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float moveDirection = mouseDelta.x > rollRightThreshold ? 1 : 0;

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);

        if (isGrounded) canDash = true;

        if (mouseMovedUp() && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        if (mouseMovedDown() && !isGrounded && canDash)
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

    bool mouseMovedUp()
    {
        return Mouse.current != null && Mouse.current.delta.y.ReadValue() > pitchUpThreshold;
    }

    bool mouseMovedDown()
    {
        return Mouse.current != null && Mouse.current.delta.y.ReadValue() < pitchDownThreshold;
    }
}