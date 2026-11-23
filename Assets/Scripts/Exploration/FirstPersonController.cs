
using UnityEngine;
using UnityEngine.InputSystem;

namespace Exploration
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private float gravity = -9.81f;

        private CharacterController controller;
        private DialogueManager dialogueManager;

        private float verticalVelocity;
        private float cameraPitch;

        private bool wasDialogueActive;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            if (playerCamera == null)
                playerCamera = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            wasDialogueActive =
                dialogueManager != null &&
                dialogueManager.IsDialogueActive;

            UpdateCursor();
        }

        private void Update()
        {
            bool dialogueActive =
                dialogueManager != null &&
                dialogueManager.IsDialogueActive;

            if (dialogueActive)
            {
                verticalVelocity = 0f;
                UpdateCursor();
                wasDialogueActive = true;
                return;
            }

            if (wasDialogueActive)
            {
                UpdateCursor();
                wasDialogueActive = false;
            }

            Move();
            Look();
        }

        private void Move()
        {
            Vector2 input = Vector2.zero;

            if (Keyboard.current != null)
            {
                input.x = Keyboard.current.aKey.isPressed ? -1f : 0f;
                input.x += Keyboard.current.dKey.isPressed ? 1f : 0f;

                input.y = Keyboard.current.sKey.isPressed ? -1f : 0f;
                input.y += Keyboard.current.wKey.isPressed ? 1f : 0f;
            }

            input = Vector2.ClampMagnitude(input, 1f);

            Vector3 move =
                transform.right * input.x +
                transform.forward * input.y;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;

            controller.Move(velocity * Time.deltaTime);
        }

        private void Look()
        {
            if (Mouse.current == null)
                return;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            float mouseX = mouseDelta.x * lookSpeed * Time.deltaTime;
            float mouseY = mouseDelta.y * lookSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseX);

            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -89f, 89f);

            playerCamera.transform.localRotation =
                Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        private void UpdateCursor()
        {
            bool dialogueActive =
                dialogueManager != null &&
                dialogueManager.IsDialogueActive;

            Cursor.lockState = dialogueActive
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            Cursor.visible = dialogueActive;
        }
    }
}
