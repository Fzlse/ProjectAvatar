using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 3f; // Jarak gerak platform
    public float moveSpeed = 2f; // Kecepatan gerak
    private Vector3 startPos;
    private bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float move = moveSpeed * Time.deltaTime;

        if (movingRight)
        {
            transform.position += Vector3.right * move;
            if (transform.position.x >= startPos.x + moveDistance)
                movingRight = false;
        }
        else
        {
            transform.position -= Vector3.right * move;
            if (transform.position.x <= startPos.x - moveDistance)
                movingRight = true;
        }
    }
}