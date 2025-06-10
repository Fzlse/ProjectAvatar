using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine.InputSystem;


public class ArduinoIMUInput : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 115200;
    private SerialPort serialPort;

    public float pitch, roll, yaw;
    private string serialData = ""; // Store incoming data to process in Update()

    [Header("Thresholds")]
    public float pitchUpThreshold = 15f;
    public float pitchDownThreshold = -15f;
    public float rollRightThreshold = 15f;

    [Header("Player Reference")]
    public GameObject player;
    private PlayerController playerController;

    // Input Actions
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction moveRightAction;

    void Awake()
    {
        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        dashAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/leftShift");
        moveRightAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/d");

        jumpAction.Enable();
        dashAction.Enable();
        moveRightAction.Enable();
    }

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 20,
                DtrEnable = true,
                RtsEnable = true
            };
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");
        }
        catch (UnauthorizedAccessException)
        {
            Debug.LogError("Access denied! Ensure no other program (like Arduino IDE) is using " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController script not found on assigned GameObject!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned!");
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialData = serialPort.ReadLine().Trim(); // Store the data, process it in Update()
                ProcessData(serialData);
            }
            catch (TimeoutException) { } // Ignore timeout errors
            catch (Exception e)
            {
                Debug.LogError("Error reading serial data: " + e.Message);
            }
        }
    }

    void ProcessData(string data)
    {
        if (!data.Contains(",")) return;

        string[] values = data.Split(',');

        if (values.Length >= 3 && float.TryParse(values[0], out pitch) && float.TryParse(values[1], out roll) && float.TryParse(values[2], out yaw))
        {
            SimulateInputs();
        }
    }

    void SimulateInputs()
    {
        if (playerController == null) return;

        if (pitch > pitchUpThreshold)
        {
            playerController.HandleJump();
            if (jumpAction.WasPressedThisFrame()) Debug.Log("Jump input detected.");
        }

        if (pitch < pitchDownThreshold)
        {
            playerController.HandleDash();
            if (dashAction.WasPressedThisFrame()) Debug.Log("Dash input detected.");
        }

        if (roll > rollRightThreshold)
        {
            playerController.HandleMovement();
            if (moveRightAction.WasPressedThisFrame()) Debug.Log("Move Right input detected.");
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}