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

        Debug.Log("PlayerCollisionHandler initialized.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player collided with an obstacle!");

            if (playerController != null)
            {
                playerController.enabled = false;
                Debug.Log("PlayerController disabled.");
            }

            rb.velocity = new Vector2(0, -10f);
            isFalling = true;
            Debug.Log("Player set to falling state with velocity (-10f).");
        }
    }

    private void Update()
    {
        if (isFalling)
        {
            Debug.Log("Player is currently falling...");

            if (transform.position.y < -10f)
            {
                Debug.LogWarning("Player jatuh terlalu jauh!");
            }
        }
    }
}
