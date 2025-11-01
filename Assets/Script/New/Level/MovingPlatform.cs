using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 3f; // jarak gerak (dari startPos.x ke kiri-kanan)
    public float moveSpeed = 2f;    // kecepatan
    [HideInInspector] public Vector3 startPos;
    public bool movingRight = true;

    void Start()
    {
        // kalau startPos belum diset dari restore, pakai posisi saat Start
        if (startPos == default(Vector3)) startPos = transform.position;
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

    // --- Helper untuk apply/capture state ---

    public PlatformState CaptureState()
    {
        return new PlatformState
        {
            position = transform.position,
            isMoving = true,
            moveDistance = moveDistance,
            moveSpeed = moveSpeed,
            movingRight = movingRight,
            startPosX = startPos.x
        };
    }

    public void ApplyState(PlatformState st)
    {
        moveDistance = st.moveDistance;
        moveSpeed    = st.moveSpeed;
        movingRight  = st.movingRight;
        startPos     = new Vector3(st.startPosX, transform.position.y, transform.position.z);
        transform.position = st.position; // pos terkini saat di-save
    }
}
