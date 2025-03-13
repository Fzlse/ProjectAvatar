/*using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = new Vector2(rb.velocity.x * 0f, -10f);
            // Mengurangi kecepatan horizontal & paksa turun
        }
    }
}*/
