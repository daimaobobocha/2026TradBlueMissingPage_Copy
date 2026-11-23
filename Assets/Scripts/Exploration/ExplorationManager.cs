
using UnityEngine;

namespace Exploration
{
    public class ExplorationManager : MonoBehaviour
    {
        [SerializeField] private GameObject introductionUI;
        [SerializeField] private PlayerController2D player;

        private bool introductionActive;
        private DialogueManager dialogueManager;

        public BattleInputActions Actions { get; private set; }

        private void Awake()
        {
            Actions = new BattleInputActions();
        }

        private void OnEnable()
        {
            Actions.Enable();
        }

        private void OnDisable()
        {
            Actions.Disable();
        }

        private void OnDestroy()
        {
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded -= FinishIntroduction;
            }

            Actions.Dispose();
        }

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            introductionActive = true;

            if (player != null)
            {
                player.enabled = false;
            }

            dialogueManager = FindFirstObjectByType<DialogueManager>();

            if (dialogueManager == null)
            {
                Debug.LogWarning("Could not find DialogueManager for train opening narration!");
                FinishIntroduction();
                return;
            }

            // Subscribe before displaying the dialogue.
            dialogueManager.OnDialogueEnded += FinishIntroduction;

            dialogueManager.ShowDialogue(
                "Narrator",
                "A small train carries a group of elves on their journey to the North Pole. But something is terribly wrong.|The elves seem confused, and the train shows no signs of slowing down. It should have stopped at the North Pole long ago, yet it continues along its endless journey.|Something must be preventing the train from reaching its destination. Perhaps the missing page is somewhere aboard.|There is one elf who might know more than the others: Patches, the elf in the green shirt. He's the main character of this story, and if anyone knows what's happening, it's him.|Find Patches and speak with him. He may know how to get this story back on track."
            );
        }

        private void FinishIntroduction()
        {
            if (!introductionActive)
                return;

            introductionActive = false;

            if (introductionUI != null)
            {
                introductionUI.SetActive(false);
            }

            if (player != null)
            {
                player.enabled = true;
            }

            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded -= FinishIntroduction;
            }
        }
    }
}
