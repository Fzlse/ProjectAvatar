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
    [SerializeField] private string shoot = "Shoot"; // Aksi menembak

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction shootAction; // Aksi menembak

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool DashTriggered { get; private set; }
    public bool ShootTriggered { get; private set; }

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
        shootAction = actionMap.FindAction(shoot); // aksi menembak

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        if (moveAction != null)
        {
            moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            moveAction.canceled += ctx => MoveInput = Vector2.zero;
        }
        if (jumpAction != null)
        {
            jumpAction.performed += ctx => JumpTriggered = true;
            jumpAction.canceled += ctx => JumpTriggered = false;
        }
        if (dashAction != null)
        {
            dashAction.performed += ctx => DashTriggered = true;
            dashAction.canceled += ctx => DashTriggered = false;
        }
        if (shootAction != null)
        {
            shootAction.performed += ctx => ShootTriggered = true;
            shootAction.canceled += ctx => ShootTriggered = false;
        }
    }

    public void EnableInput()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        dashAction?.Enable();
        shootAction?.Enable();
    }

    public void DisableInput()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        dashAction?.Disable();
        shootAction?.Disable();
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();
}
