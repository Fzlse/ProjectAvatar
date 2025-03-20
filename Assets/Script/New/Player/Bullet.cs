using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet mengenai: " + collision.gameObject.name); // Debugging

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Enemy terkena peluru!");
            Destroy(collision.gameObject); // Hancurkan enemy
            Destroy(gameObject); // Hancurkan peluru
        }
    }
}
