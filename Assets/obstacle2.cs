using UnityEngine;

public class obstacle2 : MonoBehaviour
{
    public LayerMask playerLayer;
    public float detectionDistance = 1f;
    public float heightOffset = 0.5f; // Adjust this value as needed

    private void Update()
    {
        // Cast rays in all eight directions to detect the player
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + heightOffset), Vector2.left, detectionDistance, playerLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + heightOffset), Vector2.right, detectionDistance, playerLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + heightOffset), Vector2.up, detectionDistance, playerLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, detectionDistance, playerLayer); // Keep this one at the base level
        RaycastHit2D hitDiagonalUpLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + heightOffset), new Vector2(-1, 1).normalized, detectionDistance, playerLayer);
        RaycastHit2D hitDiagonalUpRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + heightOffset), new Vector2(1, 1).normalized, detectionDistance, playerLayer);
        RaycastHit2D hitDiagonalDownLeft = Physics2D.Raycast(transform.position, new Vector2(-1, -1).normalized, detectionDistance, playerLayer); // Keep this one at the base level
        RaycastHit2D hitDiagonalDownRight = Physics2D.Raycast(transform.position, new Vector2(1, -1).normalized, detectionDistance, playerLayer); // Keep this one at the base level

        // If any ray hits something...
        if (hitLeft.collider != null && hitLeft.collider.CompareTag("Player") ||
            hitRight.collider != null && hitRight.collider.CompareTag("Player") ||
            hitUp.collider != null && hitUp.collider.CompareTag("Player") ||
            // Do not check for down collision if you want to allow jumping over
            //hitDown.collider != null && hitDown.collider.CompareTag("Player") ||
            hitDiagonalUpLeft.collider != null && hitDiagonalUpLeft.collider.CompareTag("Player") ||
            hitDiagonalUpRight.collider != null && hitDiagonalUpRight.collider.CompareTag("Player"))
            // Do not check for diagonal down collisions if you want to allow jumping over
            //hitDiagonalDownLeft.collider != null && hitDiagonalDownLeft.collider.CompareTag("Player") ||
            //hitDiagonalDownRight.collider != null && hitDiagonalDownRight.collider.CompareTag("Player"))
        {
            // Get the player script from the first hit collider
            player playerScript = hitLeft.collider ? hitLeft.collider.GetComponent<player>() :
                                  hitRight.collider ? hitRight.collider.GetComponent<player>() :
                                  hitUp.collider ? hitUp.collider.GetComponent<player>() :
                                  //hitDown.collider ? hitDown.collider.GetComponent<player>() :
                                  hitDiagonalUpLeft.collider ? hitDiagonalUpLeft.collider.GetComponent<player>() :
                                  hitDiagonalUpRight.collider.GetComponent<player>();

            if (playerScript != null)
            {
                // Stop the player's horizontal movement
                playerScript.velocity.x = 0;

                // Allow the player to jump by not affecting the vertical velocity
                // The jumping logic should be handled in the player script

                // Here you can add more effects like playing an animation, sound, etc.
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw debug lines to visualize the raycasts in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + heightOffset), new Vector3(transform.position.x + Vector3.left.x * detectionDistance, transform.position.y + heightOffset));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + heightOffset), new Vector3(transform.position.x + Vector3.right.x * detectionDistance, transform.position.y + heightOffset));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + heightOffset), new Vector3(transform.position.x + Vector3.up.x * detectionDistance, transform.position.y + heightOffset));
        // Do not draw a down ray if you want to allow jumping over
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.left + Vector3.up).normalized * detectionDistance);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right + Vector3.up).normalized * detectionDistance);
        // Do not draw diagonal down
    }
    }