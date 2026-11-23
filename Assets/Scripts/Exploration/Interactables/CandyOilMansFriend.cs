
using UnityEngine;

namespace Exploration
{
    public class CandyOilMansFriend : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueManager dialogueManager;

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Elf is missing its DialogueManager reference!");
                return;
            }

            if (GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return;
            }

            if (!GameProgression.Instance.KnowAboutCandyOilMan == true)
            {
                dialogueManager.ShowDialogue("Snowy", "Hmm I wonder what he means by that.");
                dialogueManager.ShowChoiceDialogue(
                    "Snowy",
                    "Hey you! | Tall guy can you keep a secret.",
                    "Yes",
                    "My friend pinky is working on something special, but he wont tell me what it is. | Its really frsutrating I wish he'd just tell me. | Also dont tell him I told you.",
                    "No",
                    "Nobody Likes a tattleTale.",
                    OnKnowledgeChoiceSelected
                    );
                return;
            }
            dialogueManager.ShowDialogue("Snowy", "Hmm I wonder what he means by that.");


        }

        private void OnKnowledgeChoiceSelected(int choice)
        {
                if(choice == 0)
            {
                GameProgression.Instance.LearnAboutCandyOilMan();
            }

        }

    }
}
