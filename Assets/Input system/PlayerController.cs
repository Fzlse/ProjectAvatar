using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Kecepatan tetap
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveDirection = 1; // Selalu bergerak ke kanan (Endless Runner)
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        // Jika stuck, dorong ke atas sedikit
        if (isGrounded && rb.velocity.x == 0)
        {
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }
    }

    void HandleJump()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);

        if (PlayerInputHandler.Instance.JumpTriggered && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
