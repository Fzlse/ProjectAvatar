using UnityEngine;
using System.IO.Ports;

public class ArduinoIMUInput : MonoBehaviour
{
    public string portName = "COM4"; // Ganti sesuai port Arduino Anda
    public int baudRate = 115200;

    private SerialPort serialPort;
    public float pitch, roll, yaw;

    [Header("Thresholds")]
    public float pitchUpThreshold = 15f;
    public float pitchDownThreshold = -15f;
    public float rollRightThreshold = 15f;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 50;
        serialPort.Open();
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine(); // format: "pitch,roll,yaw"
                string[] values = data.Split(',');

                if (values.Length >= 3)
                {
                    pitch = float.Parse(values[0]);
                    roll = float.Parse(values[1]);
                    yaw = float.Parse(values[2]);

                    SimulateInputs();
                }
            }
            catch { }
        }
    }

    void SimulateInputs()
    {
        // Reset
        PlayerInputHandler.Instance.MoveInput = Vector2.zero;
        PlayerInputHandler.Instance.JumpTriggered = false;
        PlayerInputHandler.Instance.DashTriggered = false;

        if (pitch > pitchUpThreshold)
        {
            // W → ke atas (jump)
            PlayerInputHandler.Instance.JumpTriggered = true;
        }

        if (pitch < pitchDownThreshold)
        {
            // SPACE → bisa di-trigger lewat dash atau flag lain
            PlayerInputHandler.Instance.DashTriggered = true;
        }

        if (roll > rollRightThreshold)
        {
            // D → jalan kanan
            PlayerInputHandler.Instance.MoveInput = new Vector2(1, 0);
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
