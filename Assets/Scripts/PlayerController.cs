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

    void Start()
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
        interactAction.action.performed += ctx => Interact();
        attackAction.action.performed += ctx => Attack();
    }

    void OnDisable()
    {
        // Disable inputs when script is deactivated
        moveAction.action.Disable();
        sprintAction.action.Disable();
        interactAction.action.Disable();
        attackAction.action.Disable();

        // Unsubscribe to prevent memory leaks
        interactAction.action.performed -= ctx => Interact();
        attackAction.action.performed -= ctx => Attack();
    }

    void Update()
    {
        // Read movement values (WASD)
        movementInput = moveAction.action.ReadValue<Vector2>();

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
}