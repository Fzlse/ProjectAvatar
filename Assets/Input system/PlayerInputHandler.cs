
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Names")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string dash = "Dash";
    [SerializeField] private string retry = "Retry";
    [SerializeField] private string shoot = "Shoot";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction retryAction;
    private InputAction shootAction;

    private Vector2 moveInput;
    private bool jumpTriggered;
    private bool dashTriggered;
    private bool shootTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerControls == null)
        {
            Debug.LogError("PlayerControls is not assigned!");
            return;
        }

        var actionMap = playerControls.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"ActionMap '{actionMapName}' not found!");
            return;
        }

        moveAction = actionMap.FindAction(move);
        jumpAction = actionMap.FindAction(jump);
        dashAction = actionMap.FindAction(dash);
        retryAction = actionMap.FindAction(retry);
        shootAction = actionMap.FindAction(shoot);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        if (moveAction != null)
        {
            moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            moveAction.canceled += ctx => moveInput = Vector2.zero;
        }
        if (jumpAction != null)
        {
            jumpAction.performed += ctx => jumpTriggered = true;
            jumpAction.canceled += ctx => jumpTriggered = false;
        }
        if (dashAction != null)
        {
            dashAction.performed += ctx => dashTriggered = true;
            dashAction.canceled += ctx => dashTriggered = false;
        }
        if (shootAction != null)
        {
            shootAction.performed += ctx => shootTriggered = true;
            shootAction.canceled += ctx => shootTriggered = false;
        }
    }

    // Getter Methods for Accessibility
    public Vector2 GetMoveInput() => moveInput;
    public bool GetJumpTriggered() => jumpTriggered;
    public bool GetDashTriggered() => dashTriggered;
    public bool GetShootTriggered() => shootTriggered;

    // Setter Methods to Modify Inputs
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetJumpTriggered(bool value) => jumpTriggered = value;
    public void SetDashTriggered(bool value) => dashTriggered = value;
    public void SetShootTriggered(bool value) => shootTriggered = value;

    public void EnableInput()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        dashAction?.Enable();
        retryAction?.Enable();
        shootAction?.Enable();
    }

    public void DisableInput()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        dashAction?.Disable();
        retryAction?.Disable();
        shootAction?.Disable();
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();
}