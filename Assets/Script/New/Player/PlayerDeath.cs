using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public LayerMask obstacleLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            GameManager.Instance.GameOver();
        }
        else if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}
