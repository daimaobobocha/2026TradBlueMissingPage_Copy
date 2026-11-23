
using UnityEngine;

namespace Exploration
{
    public class FluidElfInteractableTest : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueManager dialogueManager;

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Pinky is missing its DialogueManager reference!");
                return;
            }

            if (GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return;
            }

            GameProgression progress = GameProgression.Instance;

            if (progress.HasImprovedCandyOil)
            {
                dialogueManager.ShowDialogue(
                    "Pinky",
                    "Heehee! I hope that improved Candy Oil is treating you well!"
                );
                return;
            }

            // Secret Syrup is enough to make Improved Candy Oil.
            if (progress.HasSecretSyrup)
            {
                PlayerCombatProgression playerProgression =
                    FindFirstObjectByType<PlayerCombatProgression>();

                CharacterCombatData player = playerProgression != null
                    ? playerProgression.GetComponent<CharacterCombatData>()
                    : null;

                if (player == null)
                {
                    Debug.LogWarning("Could not find the player's CharacterCombatData!");
                    return;
                }

                if (progress.MakeImprovedCandyOil(player))
                {
                    dialogueManager.ShowDialogue(
                        "Pinky",
                        "You found it! The secret ingredient! | " +
                        "Heehee! Let me mix that right in... | " +
                        "There we go! One batch of improved Candy Oil! | " +
                        "You drink the improved Candy Oil. It tastes even sweeter than before! | " +
                        "You feel a surge of strength flowing through your body!"
                    );
                }

                return;
            }

            if (progress.HasCandyOil)
            {
                dialogueManager.ShowDialogue(
                    "Pinky",
                    "I hope that oil proves tasty! | " +
                    "Although... I wish I had a secret ingredient to make it even better."
                );
                return;
            }

            if (!progress.KnowAboutCandyOilMan)
            {
                dialogueManager.ShowDialogue(
                    "Pinky",
                    "Who are you? | Don't pay attention to me. Pretend I'm not here."
                );
                return;
            }

            dialogueManager.ShowChoiceDialogue(
                "Pinky",
                "Who are you?",
                "I'm just passing through.",
                "Then get out of here. | Don't pay attention to me. Pretend I'm not here.",
                "I hear you have something special. Can I have some?",
                "Oh, not many elves notice what I'm doing up here. | Even still, I can only give you some if you know the special password.",
                OnOpeningChoiceSelected
            );
        }

        private void OnOpeningChoiceSelected(int choice)
        {
            if (choice == 1)
                ShowPasswordChoices();
        }

        private void ShowPasswordChoices()
        {
            GameProgression progress = GameProgression.Instance;

            if (!progress.ReadSecretNote)
            {
                dialogueManager.ShowChoiceDialogue(
                    "Pinky",
                    "So, do you know the password?",
                    "Uhhh... Snowball.",
                    "You really think I'd make the password Snowball? | Come back if you actually know the password.",
                    "It's obviously Christmas cheer!",
                    "Not this time, but I admire your spirit."
                );
                return;
            }

            dialogueManager.ShowChoiceDialogue(
                "Pinky",
                "So, do you know the password?",
                "Uhhh... Snowball.",
                "You really think I'd make the password Snowball? | Come back if you actually know the password.",
                "The Silver Moon.",
                "Heehee! You got it! All right, now you can see!",
                OnPasswordChoiceSelected
            );
        }

        private void OnPasswordChoiceSelected(int choice)
        {
            if (choice != 1) return;

            ShowCandyOilExplanation();
        }

        private void ShowCandyOilExplanation()
        {
            dialogueManager.ShowDialogue(
                "Pinky",
                "I've been working on a secret candy. | " +
                "I call it Candy Oil. | " +
                "It's incredibly sweet, but also incredibly slippery."
            );

            GameProgression.Instance.GiveCandyOil();
        }
    }
}
