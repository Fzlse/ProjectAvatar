using UnityEngine;

public class LoopBG : MonoBehaviour
{
    public Transform cameraTransform; // Referensi ke kamera
    public float loopSpeed = 0.1f; // Kecepatan scroll background
    public float parallaxEffect = 0.5f; // Efek paralaks
    public Renderer bgRenderer; // Renderer dari background

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Camera belum diatur pada LoopBG!");
            return;
        }

        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        if (bgRenderer == null || cameraTransform == null)
            return;

        // Mengikuti pergerakan kamera
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y * parallaxEffect, 0);
        lastCameraPosition = cameraTransform.position;

        // Scroll texture untuk efek looping
        float offsetX = Time.time * loopSpeed;
        bgRenderer.material.mainTextureOffset = new Vector2(offsetX, 0f);
    }
}
