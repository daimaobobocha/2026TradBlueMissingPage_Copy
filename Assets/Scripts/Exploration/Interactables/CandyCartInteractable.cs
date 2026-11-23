
using UnityEngine;

namespace Exploration
{
    public class CandyCartInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueManager dialogueManager;

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Candy Cart is missing its DialogueManager reference!");
                return;
            }

            if (GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return;
            }

            GameProgression progress = GameProgression.Instance;

            // The player has already revealed the secret message
            if (progress.ReadSecretNote)
            {
                if (progress.TakenCandy)
                {
                    dialogueManager.ShowDialogue(
                        "Narrator",
                        "The hidden message is still visible on the note. | You've already taken a piece of candy from the cart."
                    );
                }
                else
                {
                    ShowCandyChoices(false);
                }

                return;
            }

            // The player has already taken candy
            if (progress.TakenCandy)
            {
                if (progress.KnowAboutSecretNote)
                {
                    ShowSecondCandyChoices();
                }
                else
                {
                    dialogueManager.ShowDialogue(
                        "Narrator",
                        "You've already taken a piece of candy. | Surely one is enough?"
                    );
                }

                return;
            }

            // First interaction with the cart
            ShowCandyChoices(progress.KnowAboutSecretNote);
        }

        // First interaction with the cart
        private void ShowCandyChoices(bool knowsAboutNote)
        {
            if (knowsAboutNote)
            {
                dialogueManager.ShowChoiceDialogue(
                    "Narrator",
                    "Looking at the cart, you see it's covered in layers of different types of candy. | There's a note sitting on top of the cart.",
                    "Take a piece of candy.",
                    "You reach into the cart and take a piece of candy.",
                    "Cast Fireball beside the note.",
                    "You carefully cast Fireball beside the note. Strange writing begins to appear in the paper.",
                    OnFirstInteractionChoice
                );
            }
            else
            {
                dialogueManager.ShowChoiceDialogue(
                    "Narrator",
                    "Looking at the cart, you see it's covered in layers of different types of candy. | There's a note sitting on top of the cart. | It reads: Take One!",
                    "Yes, I'll take a piece.",
                    "You reach into the cart and take a piece of candy.",
                    "No, I'll leave it.",
                    "You decide to leave the candy where it is.",
                    OnCandyChoiceSelected
                );
            }
        }

        // Handles the first interaction when the player knows about the note
        private void OnFirstInteractionChoice(int choice)
        {
            if (choice == 0)
            {
                TakeCandy();
            }
            else if (choice == 1)
            {
                RevealSecretMessage();
            }
        }

        // Handles the normal candy choices
        private void OnCandyChoiceSelected(int choice)
        {
            if (choice == 0)
            {
                TakeCandy();
            }
        }

        // Interaction after the player has already taken candy
        private void ShowSecondCandyChoices()
        {
            dialogueManager.ShowChoiceDialogue(
                "Narrator",
                "You look over the cart again. | You've already taken a piece of candy, but there's still plenty left.",
                "Try to take another piece.",
                "As you reach for another piece, you notice a small message written on the cart: ONE IS ENOUGH. | Perhaps you should leave the rest alone.",
                "Cast Fireball beside the note.",
                "You carefully cast Fireball beside the note. Strange writing begins to appear in the paper.",
                OnSecondInteractionChoice
            );
        }

        private void OnSecondInteractionChoice(int choice)
        {
            if (choice == 0)
            {
                dialogueManager.ShowDialogue(
                    "Narrator",
                    "You already have a piece of candy. | The cart owner clearly intended for everyone to take just one."
                );
            }
            else if (choice == 1)
            {
                RevealSecretMessage();
            }
        }

        // Records that the player has taken candy
        private void TakeCandy()
        {
            GameProgression.Instance.TakePieceOfCandy();

            // TODO: Add one piece of candy to the player's consumable inventory.

            Debug.Log("Player took a piece of candy!");
        }

        // Reveals the hidden password
        private void RevealSecretMessage()
        {
            GameProgression.Instance.ReadTheSecretNote();

            dialogueManager.ShowDialogue(
                "Narrator",
                "As the flames dance beside the note, strange writing begins to appear. | The hidden message reads: If candy is what you seek look to THE SILVER MOON."
            );
        }
    }
}
