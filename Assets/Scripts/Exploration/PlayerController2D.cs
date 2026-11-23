
using UnityEngine;

namespace Exploration
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 25f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpBufferTime = 0.15f;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Facing Direction")]
        [SerializeField] private Transform characterVisual;

        [Header("Animation")]
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite[] walkingFrames = new Sprite[3];
        [SerializeField] private float animationSpeed = 0.15f;

        private Rigidbody2D rb;
        private ExplorationInput input;
        private DialogueManager dialogueManager;
        private SpriteRenderer spriteRenderer;

        private float coyoteCounter;
        private float jumpBufferCounter;
        private float animationTimer;
        private int currentWalkingFrame;

        private bool IsDialogueActive =>
            dialogueManager != null && dialogueManager.IsDialogueActive;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            input = GetComponent<ExplorationInput>();

            if (input == null)
                input = gameObject.AddComponent<ExplorationInput>();

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            dialogueManager = FindFirstObjectByType<DialogueManager>();

            if (characterVisual != null)
                spriteRenderer = characterVisual.GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            SetIdleAnimation();
        }

        private void Update()
        {
            if (dialogueManager == null)
                dialogueManager = FindFirstObjectByType<DialogueManager>();

            if (IsDialogueActive)
            {
                jumpBufferCounter = 0f;
                SetIdleAnimation();
                return;
            }

            HandleFacingDirection();
            HandleAnimation();

            if (input.JumpPressed)
                jumpBufferCounter = jumpBufferTime;
            else
                jumpBufferCounter -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (IsDialogueActive)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                return;
            }

            HandleMovement();
            HandleJumping();
        }

        private void HandleMovement()
        {
            bool grounded = IsGrounded();
            float speedMultiplier = grounded && input.SprintHeld ? 2f : 1f;
            float targetSpeed = input.Move.x * moveSpeed * speedMultiplier;

            // Sprint is ground-only, including when walking off a platform.
            float currentSpeed = grounded
                ? rb.linearVelocity.x
                : Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed);

            float rate = Mathf.Abs(targetSpeed) > 0.01f
                ? acceleration
                : deceleration;

            float newSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                rate * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector2(
                newSpeed,
                rb.linearVelocity.y
            );
        }

        private void HandleFacingDirection()
        {
            if (input.Move.x > 0.01f)
            {
                transform.localScale = new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
            else if (input.Move.x < -0.01f)
            {
                transform.localScale = new Vector3(
                    -Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }

        private void HandleAnimation()
        {
            if (spriteRenderer == null)
                return;

            bool grounded = IsGrounded();

            bool moving = Mathf.Abs(input.Move.x) > 0.01f;

            if (!grounded || !moving || walkingFrames == null || walkingFrames.Length == 0)
            {
                SetIdleAnimation();
                return;
            }

            animationTimer += Time.deltaTime;

            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;

                currentWalkingFrame++;

                if (currentWalkingFrame >= walkingFrames.Length)
                    currentWalkingFrame = 0;

                if (walkingFrames[currentWalkingFrame] != null)
                    spriteRenderer.sprite = walkingFrames[currentWalkingFrame];
            }
        }

        private void SetIdleAnimation()
        {
            animationTimer = 0f;
            currentWalkingFrame = 0;

            if (spriteRenderer != null && idleSprite != null)
                spriteRenderer.sprite = idleSprite;
        }

        private void HandleJumping()
        {
            bool grounded = IsGrounded();

            if (grounded)
                coyoteCounter = coyoteTime;
            else
                coyoteCounter = Mathf.Max(0f, coyoteCounter - Time.fixedDeltaTime);

            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed),
                    jumpForce
                );

                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
            }
        }

        private bool IsGrounded()
        {
            return groundCheck != null && Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}
