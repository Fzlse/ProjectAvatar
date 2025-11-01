using UnityEngine;

public class LoopBG : MonoBehaviour
{
    public Transform cameraTransform;
    public float loopSpeed = 0.1f;
    public float parallaxEffect = 0.5f;
    public Renderer bgRenderer;

    private Vector3 lastCameraPosition;

    // --- VARIABEL BARU ---
    private float baseOffsetX = 0f;       // offset tersimpan / start offset
    private float scrollStartUnscaled = 0f; // timestamp unscaled saat baseOffsetX ditetapkan

    void Start()
    {
        if (!cameraTransform)
        {
            Debug.LogError("Camera belum diatur pada LoopBG!");
            return;
        }

        lastCameraPosition   = cameraTransform.position;
        // Inisialisasi titik start scrolling untuk run baru
        baseOffsetX          = GetCurrentOffsetX();
        scrollStartUnscaled  = Time.unscaledTime;
    }

    void Update()
    {
        if (!bgRenderer || !cameraTransform) return;

        // Parallax mengikuti kamera
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect,
                                          deltaMovement.y * parallaxEffect, 0f);
        lastCameraPosition = cameraTransform.position;

        // SCROLL TEKSTUR BERDASARKAN base + deltaWaktu
        float dtUnscaled = Time.unscaledTime - scrollStartUnscaled;
        float offsetX    = baseOffsetX + dtUnscaled * loopSpeed;
        SetOffsetX(offsetX);
    }

    // ===== Helpers Save/Load =====
    public float GetCurrentOffsetX()
    {
        return bgRenderer ? bgRenderer.material.mainTextureOffset.x : 0f;
    }

    public void SetOffsetX(float offsetX)
    {
        if (!bgRenderer) return;
        var off = bgRenderer.material.mainTextureOffset;
        off.x = offsetX;
        bgRenderer.material.mainTextureOffset = off;
    }

    public void SetWorldPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SyncCameraRef(Transform cam)
    {
        cameraTransform = cam;
        if (cameraTransform) lastCameraPosition = cameraTransform.position;
    }

    /// <summary>
    /// Terapkan state hasil load + sinkron baseline agar tidak “lompat”.
    /// </summary>
    public void ApplyState(float offsetX, Vector3 worldPos, Transform cam)
    {
        SetWorldPosition(worldPos);
        SetOffsetX(offsetX);

        // Set sebagai base dan mulai hitung waktu dari sekarang (supaya lanjut mulus)
        baseOffsetX         = offsetX;
        scrollStartUnscaled = Time.unscaledTime;

        // Sinkron referensi kamera agar delta parallax tidak meledak
        SyncCameraRef(cam);
    }
}
