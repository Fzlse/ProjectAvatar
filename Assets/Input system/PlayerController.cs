using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; 
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    public ParticleSystem dust;

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
        float moveDirection = 1;
        dust.Play();
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);

        if (PlayerInputHandler.Instance.JumpTriggered && isGrounded)
        {
            dust.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
