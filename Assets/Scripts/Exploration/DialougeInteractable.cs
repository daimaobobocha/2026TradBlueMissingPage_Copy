
using UnityEngine;

namespace Exploration
{
    public class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private string speakerName;
        [TextArea(3, 10)]
        [SerializeField] private string dialogue;

        public void Interact()
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning("Dialogue Manager is not assigned!");
                return;
            }

            dialogueManager.ShowDialogue(speakerName, dialogue);
        }
    }
}
