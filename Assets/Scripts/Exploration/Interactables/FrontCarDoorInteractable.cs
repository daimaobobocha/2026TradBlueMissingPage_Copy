
using UnityEngine;

namespace Exploration
{
    public class FrontCarDoorInteractable : MonoBehaviour, IInteractable
    {
        [Header("Key Requirement")]
        [Tooltip("Disable temporarily to enter the front car without finding the key.")]
        [SerializeField] private bool requireFrontCarKey = true;

        [Header("References")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private Transform player;
        [SerializeField] private Transform destination;
        [SerializeField] private CameraFollow2D cameraFollow;
        [SerializeField] private CameraArea2D destinationCameraArea;

        [Header("Locked Dialogue")]
        [SerializeField]
        private string lockedMessage =
            "The door is locked. You'll need a key to get through.";

        [Header("Entry Dialogue")]
        [SerializeField]
        private string entryQuestion =
            "The door leads to the front of the train. Do you want to enter?";

        [SerializeField] private string enterChoice = "Yes, enter.";

        [SerializeField]
        private string enterResponse =
            "You unlock the door with your key and step inside.";

        [SerializeField] private string stayChoice = "No, stay here.";

        [SerializeField]
        private string stayResponse =
            "You decide to stay where you are.";

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Front Car Door is missing its DialogueManager reference!");
                return;
            }

            if (requireFrontCarKey && GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return;
            }

            if (player == null || destination == null)
            {
                Debug.LogWarning("Front Car Door is missing its Player or Destination reference!");
                return;
            }

            if (requireFrontCarKey && !GameProgression.Instance.HasFrontCarKey)
            {
                dialogueManager.ShowDialogue("Narrator", lockedMessage);
                return;
            }

            AskToEnter();
        }

        private void AskToEnter()
        {
            dialogueManager.ShowChoiceDialogue(
                "Narrator",
                entryQuestion,
                enterChoice,
                requireFrontCarKey
                    ? enterResponse
                    : "You open the door and step inside.",
                stayChoice,
                stayResponse,
                OnEntryChoiceSelected
            );
        }

        private void OnEntryChoiceSelected(int choice)
        {
            if (choice != 0)
                return;

            Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();

            if (playerBody != null)
            {
                playerBody.position = destination.position;
                playerBody.rotation = destination.eulerAngles.z;
                playerBody.linearVelocity = Vector2.zero;
            }
            else
            {
                player.SetPositionAndRotation(
                    destination.position,
                    destination.rotation
                );
            }

            if (cameraFollow != null && destinationCameraArea != null)
            {
                cameraFollow.SetCameraArea(destinationCameraArea);
            }
        }
    }
}
