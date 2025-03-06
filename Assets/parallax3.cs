using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax3 : MonoBehaviour
{
    public float depth = 1;
    public LayerMask playerLayer;
    public float detectionDistance = 1;
    public float raySpacing = 0.30f; // Spacing between rays, in meters (1 cm)

player player;
    private void Awake()
    {
        player = GameObject.Find("player").GetComponent<player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
void FixedUpdate()
{
    // Calculate the number of rays horizontally and vertically
    int horizontalRays = Mathf.CeilToInt(transform.localScale.x / raySpacing);
    int verticalRays = Mathf.CeilToInt(transform.localScale.y / raySpacing);

    // Cast rays in a grid pattern
    for (int i = 0; i < horizontalRays; i++)
    {
        for (int j = 0; j < verticalRays; j++)
        {
            // Calculate the origin for this ray
            Vector2 origin = new Vector2(
                transform.position.x - transform.localScale.x / 2 + i * raySpacing,
                transform.position.y - transform.localScale.y / 2 + j * raySpacing
            );

            // Cast the ray
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, detectionDistance, playerLayer);

            // Check if the ray hit something
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // Handle collision with player (e.g., stop movement, play animation)
                // ...
            }
          {
        float realvelocity = player.velocity.x / depth;
        Vector2 pos = transform.position;

         pos.x -= realvelocity * Time.fixedDeltaTime;

        if (pos.x <= -49)
        pos.x = 234;
        transform.position = pos;
    }
        
        }
    }
}
}