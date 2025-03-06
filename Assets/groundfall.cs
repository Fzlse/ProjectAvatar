using UnityEngine;

public class groundfall : MonoBehaviour
{
    bool playerOnGround = false;
    public float fallSpeed = 1f; // Speed at which the ground falls
    public float fallDistance = 0.5f; // Distance the ground falls when player steps on it
    public player player; // Reference to the player object

    void Update()
    {
        if (playerOnGround)
        {
            FallGround();
        }
    }

    void FallGround()
    {
        Vector2 pos = transform.position;
        pos.y -= fallSpeed * Time.deltaTime;

        // Move player along with ground if player exists
        if (player != null)
        {
            Vector2 playerpos = player.transform.position;
            playerpos.y -= fallSpeed * Time.deltaTime;
            player.transform.position = playerpos;
        }

        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnGround = false;
        }
    }
}
