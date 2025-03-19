using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    private bool isFalling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            rb.velocity = new Vector2(0, -10f);
            isFalling = true;
        }
    }

    private void Update()
    {
        if (isFalling && transform.position.y < -10f)
        {
            Debug.Log("Player jatuh terlalu jauh!");
        }
    }
}
