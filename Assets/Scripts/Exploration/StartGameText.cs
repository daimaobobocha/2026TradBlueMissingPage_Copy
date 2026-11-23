
using UnityEngine;

namespace Exploration
{
    public class StartGameText : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private string speakerName = "Narrator";

        private DialogueManager dialogueManager;

        private void Start()
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();

            if (dialogueManager == null)
            {
                Debug.LogWarning("Could not find DialogueManager in this scene!");
                return;
            }

            dialogueManager.ShowDialogue(
                speakerName,
                "Something has gone wrong in the library. Pages from the books have been disappearing, and the stories within them are beginning to unravel. Restoring the books to their former glory and finding the missing pages is now your top priority.|Keep an eye out for any strange-looking books. They may hold the key to repairing the stories.|A quick Note: this game is played entirely using WASD, E, and your mouse.|If you participated in the previous playtest, not much has changed in the first portion of the game. However, the second portion has received some major upgrades!|Take some time to investigate the library, find the glowing book, and click on it to begin. Enjoy the second playtest!"
            );
        }
    }
}
