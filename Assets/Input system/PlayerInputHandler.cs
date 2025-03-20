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
    [SerializeField] private string shoot = "Shoot"; // Tambahkan aksi menembak

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction retryAction;
    private InputAction shootAction; // Tambahkan aksi menembak

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool DashTriggered { get; private set; }
    public bool ShootTriggered { get; private set; } // Tambahkan aksi menembak

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
        shootAction = actionMap.FindAction(shoot); // Tambahkan aksi menembak

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
        if (shootAction != null) // Tambahkan aksi menembak
        {
            shootAction.performed += ctx => ShootTriggered = true;
            shootAction.canceled += ctx => ShootTriggered = false;
        }
        if (retryAction != null)
        {
            retryAction.performed += ctx =>
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                {
                    GameManager.Instance.BackToMainMenu();
                }
            };
        }
    }

    public void EnableInput()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        dashAction?.Enable();
        retryAction?.Enable();
        shootAction?.Enable(); // Tambahkan aksi menembak
    }

    public void DisableInput()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        dashAction?.Disable();
        retryAction?.Disable();
        shootAction?.Disable(); // Tambahkan aksi menembak
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();
}
