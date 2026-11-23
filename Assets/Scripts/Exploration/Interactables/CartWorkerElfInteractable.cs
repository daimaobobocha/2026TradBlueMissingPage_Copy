
using UnityEngine;

namespace Exploration
{
    public class CartWorkerElfInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueManager dialogueManager;
        private bool talkedTo = false;

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
            if (!talkedTo)
            {
                dialogueManager.ShowDialogue(
                    "Orange Hat Elf",
                    "Im the elf in charge of the candy cart. | Have you taken a piece yet? | If not you should its delicious, and magical."
                );
                talkedTo = true;
                return;
            }
            dialogueManager.ShowDialogue(
             "Orange Hat Elf",
                "Although yesterday while I was working the strangest thing happened. | I walked past a candle, and I swear the note on the cart had extra words. | Im not sure why that would be though."
                );
            GameProgression.Instance.LearnAboutSecretNote();
        }
    }
}
