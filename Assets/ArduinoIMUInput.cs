using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ArduinoIMUInput : MonoBehaviour
{
    public string portName = "COM4"; // Adjust to match your Arduino port
    public int baudRate = 115200;

    private SerialPort serialPort;
    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Thresholds")]
    public float pitchUpThreshold = 15f;
    public float pitchDownThreshold = -15f;
    public float rollRightThreshold = 15f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float pitch, roll, yaw;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Ensure Rigidbody2D is attached

        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 50;
        serialPort.Open();
    }

    void Update()
    {
        ReadArduinoData();
        HandleJump();
        HandleDash();
    }
    
    void ReadArduinoData()
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        try
        {
            string data = serialPort.ReadLine(); // Expected format: "pitch,roll,yaw"
            ParseData(data);
            Debug.Log($"Pitch: {pitch}, Roll: {roll}, Yaw: {yaw}");

            SimulateInputs();
        }
        catch { }
    }
    void ParseData(string raw)
    {
        string[] parts = raw.Split(' ');
        foreach (var part in parts)
        {
            if (part.StartsWith("P:")) pitch = float.Parse(part.Substring(2));
            if (part.StartsWith("R:")) roll = float.Parse(part.Substring(2));
            if (part.StartsWith("Y:")) yaw = float.Parse(part.Substring(2));
        }
    }

    void SimulateInputs()
    {
        if (isDashing) return;

        float moveDirection = roll > rollRightThreshold ? 1 : 0;
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);

        if (isGrounded) canDash = true;

        if (pitch > pitchUpThreshold && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        if (pitch < pitchDownThreshold && !isGrounded && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float dashDirection = Mathf.Sign(rb.velocity.x);
        if (dashDirection == 0) dashDirection = 1;

        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}