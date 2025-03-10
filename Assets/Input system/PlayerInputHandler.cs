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

    private InputAction moveAction;
    private InputAction jumpAction;

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
            Debug.LogError("ActionMap with name " + actionMapName + " not found!");
            return;
        }

        moveAction = actionMap.FindAction(move);
        jumpAction = actionMap.FindAction(jump);

        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        if (moveAction != null)
        {
            moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
            moveAction.canceled += context => MoveInput = Vector2.zero;
        }

        if (jumpAction != null)
        {
            jumpAction.performed += context => JumpTriggered = true;
            jumpAction.canceled += context => JumpTriggered = false;
        }
    }

    public void EnableInput()
    {
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
    }

    public void DisableInput()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();
}
