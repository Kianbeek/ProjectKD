using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Grounding")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Input (Input System)")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [SerializeField] private Rigidbody2D body;

    private Vector2 _moveInput;
    private bool _jumpQueued;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += QueueJump;
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= QueueJump;
            jumpAction.action.Disable();
        }

        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (moveAction != null)
        {
            _moveInput = moveAction.action.ReadValue<Vector2>();
        }
        else
        {
            _moveInput = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        var velocity = body.linearVelocity;
        velocity.x = _moveInput.x * moveSpeed;

        if (_jumpQueued && IsGrounded())
        {
            velocity.y = 0f;
            body.linearVelocity = velocity;
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            body.linearVelocity = velocity;
        }

        _jumpQueued = false;
    }

    private void QueueJump(InputAction.CallbackContext context)
    {
        _jumpQueued = true;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
