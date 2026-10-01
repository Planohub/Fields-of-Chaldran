using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float sprintMultiplier = 1.5f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;
    public InputActionReference interactAction;
    public InputActionReference attackAction;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private float currentMoveSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = baseMoveSpeed;
    }

    void OnEnable()
    {
        // Enable inputs when script is active
        moveAction.action.Enable();
        sprintAction.action.Enable();
        interactAction.action.Enable();
        attackAction.action.Enable();

        // Subscribe to action events
        interactAction.action.performed += OnInteract;
        attackAction.action.performed += OnAttack;
    }

    void OnDisable()
    {
        // Remove the same delegates that OnEnable registered.
        interactAction.action.performed -= OnInteract;
        attackAction.action.performed -= OnAttack;

        // Disable inputs when script is deactivated
        moveAction.action.Disable();
        sprintAction.action.Disable();
        interactAction.action.Disable();
        attackAction.action.Disable();
    }

    void Update()
    {
        // Read movement values (WASD)
        movementInput = Vector2.ClampMagnitude(moveAction.action.ReadValue<Vector2>(), 1f);

        // Check if sprint is held down
        if (sprintAction.action.IsPressed())
        {
            currentMoveSpeed = baseMoveSpeed * sprintMultiplier;
        }
        else
        {
            currentMoveSpeed = baseMoveSpeed;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementInput * currentMoveSpeed * Time.fixedDeltaTime);
    }

    private void Interact() { Debug.Log("Interact with object/NPC"); }
    private void Attack() { Debug.Log("Attack Executed"); }
    private void OnInteract(InputAction.CallbackContext context) { Interact(); }
    private void OnAttack(InputAction.CallbackContext context) { Attack(); }
}
