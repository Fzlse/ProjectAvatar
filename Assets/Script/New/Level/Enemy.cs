using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float destroyDistance = 25f;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (playerTransform != null && transform.position.x < playerTransform.position.x - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
