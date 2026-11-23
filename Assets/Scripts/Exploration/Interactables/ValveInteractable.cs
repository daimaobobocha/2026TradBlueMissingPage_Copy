
using UnityEngine;

namespace Exploration
{
    public class ValveInteractable : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private Transform player;
        [SerializeField] private Transform destination;
        [SerializeField] private CameraFollow2D cameraFollow;
        [SerializeField] private CameraArea2D destinationCameraArea;

        [Header("Hatch Visuals")]
        [SerializeField] private GameObject closedHatchVisual;
        [SerializeField] private GameObject openHatchVisual;

        [Header("Candy Oil Dialogue")]
        [SerializeField] private string stuckMessage = "The hatch is stuck and won't turn.";
        [SerializeField] private string useOilQuestion = "The hatch looks incredibly rusty. Do you want to use your Candy Oil?";
        [SerializeField] private string useOilChoice = "Use the Candy Oil.";
        [SerializeField] private string useOilResponse = "You pour the Candy Oil over the hatch. | The slippery substance works its way into the mechanism. | With a little effort, you manage to turn the valve!";
        [SerializeField] private string keepOilChoice = "Save the Candy Oil.";
        [SerializeField] private string keepOilResponse = "You decide to save your Candy Oil for later.";

        [Header("Entry Dialogue")]
        [SerializeField] private string entryQuestion = "Do you want to enter? It looks dark down there.";
        [SerializeField] private string enterChoice = "Yes, enter.";
        [SerializeField] private string enterResponse = "You make your way inside.";
        [SerializeField] private string stayChoice = "No, stay here.";
        [SerializeField] private string stayResponse = "You decide to stay where you are.";

        private void Start()
        {
            UpdateValveVisuals();
        }

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Valve is missing its DialogueManager reference!");
                return;
            }

            if (GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return;
            }

            if (player == null || destination == null)
            {
                Debug.LogWarning("Valve is missing its Player or Destination reference!");
                return;
            }

            GameProgression progress = GameProgression.Instance;

            if (progress.OpenedValve)
            {
                AskToEnter();
                return;
            }

            if (!progress.HasCandyOil)
            {
                dialogueManager.ShowDialogue("Narrator", stuckMessage);
                return;
            }

            dialogueManager.ShowChoiceDialogue(
                "Narrator",
                useOilQuestion,
                useOilChoice,
                useOilResponse,
                keepOilChoice,
                keepOilResponse,
                OnOilChoiceSelected
            );
        }

        private void OnOilChoiceSelected(int choice)
        {
            if (choice != 0)
            {
                return;
            }

            GameProgression progress = GameProgression.Instance;

            if (progress == null || !progress.HasCandyOil)
            {
                return;
            }

            progress.UseCandyOil();
            progress.OpenUpValve();

            UpdateValveVisuals();

            Debug.Log("The Hatch has been opened!");
        }

        private void AskToEnter()
        {
            dialogueManager.ShowChoiceDialogue(
                "Narrator",
                entryQuestion,
                enterChoice,
                enterResponse,
                stayChoice,
                stayResponse,
                OnEntryChoiceSelected
            );
        }

        private void OnEntryChoiceSelected(int choice)
        {
            if (choice != 0)
            {
                return;
            }

            Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();

            if (playerBody != null)
            {
                playerBody.position = destination.position;
                playerBody.rotation = destination.eulerAngles.z;
                playerBody.linearVelocity = Vector2.zero;
            }
            else
            {
                player.SetPositionAndRotation(destination.position, destination.rotation);
            }

            if (cameraFollow != null && destinationCameraArea != null)
            {
                cameraFollow.SetCameraArea(destinationCameraArea);
            }
        }

        private void UpdateValveVisuals()
        {
            bool isOpen = GameProgression.Instance != null &&
                          GameProgression.Instance.OpenedValve;

            if (closedHatchVisual != null)
            {
                closedHatchVisual.SetActive(!isOpen);
            }

            if (openHatchVisual != null)
            {
                openHatchVisual.SetActive(isOpen);
            }
        }
    }
}
