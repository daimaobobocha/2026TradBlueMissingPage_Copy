
using UnityEngine;

namespace Exploration
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer;

        [Header("UI")]
        [SerializeField] private GameObject interactionIcon;

        private ExplorationInput input;
        private IInteractable currentInteractable;
        private bool hideIconUntilExit;

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        private void Awake()
        {
            input = GetComponent<ExplorationInput>();

            if (input == null)
                input = gameObject.AddComponent<ExplorationInput>();
        }

        private void Update()
        {
            bool grounded = IsGrounded();

            if (!grounded)
            {
                currentInteractable = null;

                if (interactionIcon != null)
                    interactionIcon.SetActive(false);

                return;
            }

            FindInteractable();

            if (currentInteractable != null &&
                !hideIconUntilExit &&
                input.InteractPressed)
            {
                hideIconUntilExit = true;

                if (interactionIcon != null)
                    interactionIcon.SetActive(false);

                currentInteractable.Interact();
            }
        }

        private void FindInteractable()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                interactionRadius,
                interactableLayer
            );

            currentInteractable = null;

            foreach (Collider2D hit in hits)
            {
                IInteractable interactable =
                    hit.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    currentInteractable = interactable;
                    break;
                }
            }

            if (currentInteractable == null)
                hideIconUntilExit = false;

            if (interactionIcon != null)
            {
                interactionIcon.SetActive(
                    currentInteractable != null &&
                    !hideIconUntilExit
                );
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);

            if (groundCheck != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}
