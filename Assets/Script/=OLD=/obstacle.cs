using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            // Stop the player's horizontal movement
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            // Here you can add more effects like playing an animation, sound, etc.
        }
    }
}
