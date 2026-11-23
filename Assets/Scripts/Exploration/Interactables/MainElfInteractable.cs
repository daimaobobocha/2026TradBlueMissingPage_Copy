
using UnityEngine;

namespace Exploration
{
    public class MainElfInteractable : MonoBehaviour, IInteractable
    {
        [Header("Dialogue")]
        [SerializeField] private string speakerName = "Main Elf";

        private DialogueManager dialogueManager;

        private void Start()
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }

        public void Interact()
        {
            if (dialogueManager == null)
                dialogueManager = FindFirstObjectByType<DialogueManager>();

            if (dialogueManager == null || GameProgression.Instance == null)
                return;

            GameProgression progression = GameProgression.Instance;

            if (progression.PulledTrainLever)
            {
                dialogueManager.ShowDialogue(
                    speakerName,
                    "You did it! The train is finally stopping at the North Pole!|Thank you for saving Christmas. | That concludes this demo!| We hope you enjoyed it."
                );
                return;
            }

            if (progression.HasFrontCarKey)
            {
                dialogueManager.ShowDialogue(
                    speakerName,
                    "You found the backup key! Thank goodness.|Please head to the conductor's compartment and see what's going on. We need to get this train back on course!"
                );
                return;
            }

            dialogueManager.ShowDialogue(
                speakerName,
                "Something's wrong! The train should have stopped at the North Pole by now, but it hasn't slowed down at all.|I'm worried something has happened to the conductor, but I don't have a key to get into the conductor's compartment.|My friend wearing a light blue shirt is in charge of holding onto a spare. | Go see if you can find him."
            );
        }
    }
}
