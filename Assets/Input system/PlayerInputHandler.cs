using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; } // Singleton

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Names")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string retry = "Retry"; // Tambahkan input Retry

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction retryAction;

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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
        retryAction = actionMap.FindAction(retry); // Ambil action retry

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
        if (retryAction != null) // Tambahkan event untuk retry
        {
            retryAction.performed += ctx =>
            {
                if (GameManager.Instance.IsGameOver()) return;
                {
                    GameManager.Instance.RetryGame();
                }
            };
        }
    }

    public void EnableInput()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        retryAction?.Enable(); // Pastikan input retry juga di-enable
    }

    public void DisableInput()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        retryAction?.Disable();
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();
}
