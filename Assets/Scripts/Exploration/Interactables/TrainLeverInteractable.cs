
using UnityEngine;

namespace Exploration
{
    public class TrainLeverInteractable : MonoBehaviour, IInteractable
    {
        [Header("Enemy Requirement")]
        [Tooltip("Disable temporarily to test the train lever without the boss battle.")]
        [SerializeField] private bool requireRobberOfJoyDefeated = true;
        [SerializeField] private string robberOfJoyEncounterId = "RobberOfJoy";

        [Header("Dialogue")]
        [SerializeField] private string speakerName = "Train Lever";

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
                    "The lever is already pulled. The train has stopped at the North Pole!"
                );
                return;
            }

            if (requireRobberOfJoyDefeated &&
                !EnemyEncounter.HasDefeated(robberOfJoyEncounterId))
            {
                dialogueManager.ShowDialogue(
                    speakerName,
                    "The lever controls the train's emergency brakes. You shouldn't pull it while there's still danger aboard!"
                );
                return;
            }

            progression.MarkTrainLeverPulled();

            dialogueManager.ShowDialogue(
                speakerName,
                "You pull the lever!|The train begins to slow down as the brakes engage.|After a few moments, the train comes to a complete stop.|You've stopped the train at the North Pole!"
            );
        }
    }
}
