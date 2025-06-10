
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseIMUInput : MonoBehaviour
{
    public float pitch, roll, yaw;
    private Vector2 mouseDelta;

    [Header("Inspection Value")]
    public float inspectionThreshold = 10f; // Change dynamically in Inspector

    [Header("Player Reference")]
    public GameObject player;
    private PlayerController playerController;

    // Input Actions
    private InputAction mouseMoveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction moveRightAction;

    void Awake()
    {
        mouseMoveAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta");
        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        dashAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/leftShift");
        moveRightAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/d");

        mouseMoveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
        moveRightAction.Enable();
    }

    void Start()
    {
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
        mouseDelta = mouseMoveAction.ReadValue<Vector2>();

        pitch += mouseDelta.y * 0.1f;
        roll += mouseDelta.x * 0.1f;
        yaw += mouseDelta.x * 0.05f;

        SimulateInputs();
    }

    void SimulateInputs()
    {
        if (playerController == null) return;

        if (pitch > inspectionThreshold)
        {
            playerController.HandleJump();
            if (jumpAction.WasPressedThisFrame()) Debug.Log("Jump input detected.");
        }

        if (pitch < -inspectionThreshold)
        {
            playerController.HandleDash();
            if (dashAction.WasPressedThisFrame()) Debug.Log("Dash input detected.");
        }

        if (roll > inspectionThreshold)
        {
            playerController.HandleMovement();
            if (moveRightAction.WasPressedThisFrame()) Debug.Log("Move Right input detected.");
        }
    }
}