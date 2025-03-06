using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f; // Adjustable speed
    public bool useRigidbody = true; // Toggle between Rigidbody and Transform movement
    public bool allowXMovement = true;
    public bool allowZMovement = true;

    [Header("Jump Settings")]
    public bool allowJump = true;
    public float jumpForce = 5f; // Jump power
    public float gravity = 9.81f; // Gravity strength
    public float fallMultiplier = 2.5f; // Speed multiplier when falling
    public int maxJumpCount = 2; // Maximum number of jumps (for double jump)
    public float groundCheckDistance = 0.1f; // Distance for raycast to check if grounded

    [Header("Stamina Settings")]
    public float maxStamina = 100f; // Max stamina value
    public float stamina = 100f; // Current stamina
    public float staminaDrainRate = 10f; // How much stamina is drained per jump
    public float staminaRegenRate = 5f; // How much stamina regenerates per second

    [Header("UI Settings")]
    public UnityEngine.UI.Text staminaText; // Reference to the Text UI element for stamina

    private Rigidbody rb;
    private Vector3 moveDirection;
    private int jumpCount; // Tracks the number of jumps
    private GameManager gameManager;
    private float holdTime = 0f; // Track how long the jump button is held down

    void Start()
    {
        if (useRigidbody)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
                UnityEngine.Debug.LogError("Rigidbody is missing from the Player!");
            else
                rb.useGravity = false; // Custom gravity handling
        }

        if (staminaText == null)
        {
            UnityEngine.Debug.LogError("Stamina Text UI is missing!");
        }

        // Get the GameManager reference
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        GetInput();
        UpdateStaminaUI();
    }

    void FixedUpdate()
    {
        MovePlayer();
        HandleJump();
        RegenerateStamina();

        // Check if the player falls below a certain height (game over condition)
        if (transform.position.y < -10f)  // Example: if the player falls below y = -10
        {
            gameManager.EndGame();  // Call the EndGame function from GameManager
        }
    }

    void GetInput()
    {
        float moveX = allowXMovement ? Input.GetAxis("Horizontal") : 0f;
        float moveZ = allowZMovement ? Input.GetAxis("Vertical") : 0f;

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
    }

    void MovePlayer()
    {
        if (useRigidbody && rb != null)
        {
            Vector3 velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
            rb.velocity = velocity;
        }
        else
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    void HandleJump()
    {
        if (!allowJump || !useRigidbody) return;

        // Check if the jump button is pressed on either keyboard (space) or controller (A button)
        bool jumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);

        // Check if the jump button is being held down
        if (Input.GetButton("Jump") || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
        {
            // Increase hold time while the button is being held
            holdTime += Time.deltaTime;
        }

        // Check if grounded using raycast to avoid delay
        bool isGrounded = IsGrounded();

        // Jump if grounded and have enough stamina
        if (jumpPressed && isGrounded && stamina >= staminaDrainRate)
        {
            // Calculate the final jump force based on how long the button was held
            float finalJumpForce = jumpForce + (holdTime * 2f); // The longer you hold, the higher the jump
            rb.velocity = new Vector3(rb.velocity.x, finalJumpForce, rb.velocity.z);

            // Reset jump count for first jump
            jumpCount = 1;

            // Drain stamina based on hold time
            stamina -= staminaDrainRate + (holdTime * 2f); // Drain more stamina for longer hold
            stamina = Mathf.Max(stamina, 0); // Ensure stamina doesn't go below 0

            // Reset hold time after jump
            holdTime = 0f;
        }
        // Double jump if not grounded and second jump is available, with enough stamina
        else if (jumpPressed && jumpCount < maxJumpCount && stamina >= staminaDrainRate)
        {
            // Use the same logic for double jump, including hold time
            float finalJumpForce = jumpForce + (holdTime * 2f);
            rb.velocity = new Vector3(rb.velocity.x, finalJumpForce, rb.velocity.z);

            jumpCount++; // Increment jump count for double jump
            stamina -= staminaDrainRate + (holdTime * 2f); // Drain stamina for double jump
            stamina = Mathf.Max(stamina, 0); // Ensure stamina doesn't go below 0

            holdTime = 0f; // Reset hold time after double jump
        }

        // Apply custom gravity
        if (!isGrounded)
        {
            float gravityForce = gravity * (rb.velocity.y < 0 ? fallMultiplier : 1);
            rb.velocity += Vector3.down * gravityForce * Time.fixedDeltaTime;
        }
    }


    // Raycast to check if the player is grounded
    bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
    }

    // Regenerate stamina over time
    void RegenerateStamina()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.fixedDeltaTime; // Regenerate stamina
            stamina = Mathf.Min(stamina, maxStamina); // Ensure stamina doesn't exceed max
        }
    }

    // Update the stamina text UI element
    void UpdateStaminaUI()
    {
        if (staminaText != null)
        {
            staminaText.text = "Stamina: " + Mathf.RoundToInt(stamina).ToString(); // Update UI with current stamina
        }
    }
}
