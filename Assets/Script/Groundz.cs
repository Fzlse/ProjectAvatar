using UnityEngine;

public class Groundz : MonoBehaviour
{
    public float speed = 5f; // Matches player speed
    public float resetPosition = -10f; // When to move ground back
    public float startPosition = 10f; // Where to reset the ground to

    void Update()
    {
        // Move ground opposite to player direction
        transform.position += Vector3.left * speed * Time.deltaTime;

        // If the ground moves too far back, reposition it forward
        if (transform.position.x <= resetPosition)
        {
            transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
        }
    }
}
