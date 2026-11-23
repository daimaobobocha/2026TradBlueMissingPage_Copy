
using UnityEngine;

namespace Exploration
{
    public class LoadingZone : MonoBehaviour, IInteractable
    {
        [System.Serializable]
        private class CarDestination
        {
            public int carNumber;
            public Transform arrivalPoint;
            public CameraArea2D cameraArea;
        }

        private enum LockRequirement
        {
            None,
            ValveOpened,
            FrontCarKey
        }

        [Header("References")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private Transform player;
        [SerializeField] private Transform destination;
        [SerializeField] private CameraFollow2D cameraFollow;
        [SerializeField] private CameraArea2D destinationCameraArea;

        [Header("Train Car Selection")]
        [SerializeField] private int currentCarNumber;
        [SerializeField] private CarDestination[] carDestinations = new CarDestination[0];

        [Header("Lock Settings")]
        [SerializeField] private LockRequirement lockRequirement;
        [SerializeField] private string lockedMessage = "The door is locked.";

        [Header("Dialogue")]
        [SerializeField] private string entryQuestion = "Do you want to enter?";
        [SerializeField] private string enterChoice = "Yes, enter.";
        [SerializeField] private string enterResponse = "You make your way inside.";
        [SerializeField] private string stayChoice = "No, stay here.";
        [SerializeField] private string stayResponse = "You decide to stay where you are.";

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Loading Zone is missing its DialogueManager reference!");
                return;
            }

            if (player == null || (carDestinations.Length == 0 && destination == null))
            {
                Debug.LogWarning("Loading Zone is missing its Player or Destination reference!");
                return;
            }

            if (!CanEnter())
            {
                dialogueManager.ShowDialogue("Narrator", lockedMessage);
                return;
            }

            if (carDestinations.Length == 2)
            {
                foreach (CarDestination car in carDestinations)
                {
                    if (car.arrivalPoint == null || car.cameraArea == null)
                    {
                        Debug.LogWarning("Train car destination is missing its arrival point or camera area!");
                        return;
                    }
                }

                ShowCarChoices();
                return;
            }

            dialogueManager.ShowChoiceDialogue(
                "Narrator",
                entryQuestion,
                enterChoice,
                enterResponse,
                stayChoice,
                stayResponse,
                OnChoiceSelected
            );
        }

        private bool CanEnter()
        {
            if (GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return false;
            }

            GameProgression progress = GameProgression.Instance;

            switch (lockRequirement)
            {
                case LockRequirement.ValveOpened:
                    return progress.OpenedValve;

                case LockRequirement.FrontCarKey:
                    return progress.HasFrontCarKey;

                default:
                    return true;
            }
        }

        private void OnChoiceSelected(int choice)
        {
            if (choice != 0)
            {
                return;
            }

            TravelTo(destination, destinationCameraArea);
        }

        private void ShowCarChoices()
        {
            CarDestination nextCar = null;
            CarDestination fastTravelCar = null;
            foreach (CarDestination car in carDestinations)
            {
                // Keep the original destination of this particular connector.
                if (car.cameraArea == destinationCameraArea)
                    nextCar = car;
                else if (GameProgression.Instance.HasVisitedTrainCar(car.carNumber))
                    fastTravelCar = car;
            }

            if (nextCar == null)
            {
                Debug.LogWarning("Train connector is missing its adjacent car destination!");
                return;
            }

            string question = "You are in Car " + currentCarNumber + ". Where would you like to go?";
            if (fastTravelCar == null)
            {
                dialogueManager.ShowChoiceDialogue(
                    "Narrator", question,
                    "Next car", "You make your way to the next car.",
                    stayChoice, stayResponse,
                    choice => { if (choice == 0) EnterCar(nextCar); }
                );
                return;
            }

            dialogueManager.ShowThreeChoiceDialogue(
                "Narrator", question,
                "Next car", "You make your way to the next car.",
                "Car " + fastTravelCar.carNumber,
                "You make your way to Car " + fastTravelCar.carNumber + ".",
                stayChoice, stayResponse,
                choice =>
                {
                    if (choice == 0)
                        EnterCar(nextCar);
                    else if (choice == 1 && GameProgression.Instance.HasVisitedTrainCar(fastTravelCar.carNumber))
                        EnterCar(fastTravelCar);
                }
            );
        }

        private void EnterCar(CarDestination car)
        {
            TravelTo(car.arrivalPoint, car.cameraArea);
            GameProgression.Instance.MarkTrainCarVisited(car.carNumber);
        }

        private void TravelTo(Transform arrivalPoint, CameraArea2D cameraArea)
        {
            Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();

            if (playerBody != null)
            {
                playerBody.position = arrivalPoint.position;
                playerBody.rotation = arrivalPoint.eulerAngles.z;
                playerBody.linearVelocity = Vector2.zero;
            }
            else
            {
                player.SetPositionAndRotation(arrivalPoint.position, arrivalPoint.rotation);
            }

            if (cameraFollow != null && cameraArea != null)
            {
                cameraFollow.SetCameraArea(cameraArea);
            }
        }
    }
}
