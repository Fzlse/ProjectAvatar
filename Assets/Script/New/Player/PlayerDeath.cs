using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public LayerMask obstacleLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckDeath(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CheckDeath(other.gameObject);
    }

    private void CheckDeath(GameObject other)
    {
        // kalau GameManager belum siap atau sudah game over, gak usah lanjut
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
            return;

        // cek tag dulu
        if (other.CompareTag("DeathZone") || other.CompareTag("Enemy"))
        {
            GameManager.Instance.GameOver();
            return;
        }

        // kalau dia bukan pakai tag tapi kamu set layer di inspector
        if (((1 << other.layer) & obstacleLayer) != 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}
