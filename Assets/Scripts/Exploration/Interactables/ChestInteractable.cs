
using UnityEngine;
using Items;
using BattleSystem;

namespace Exploration
{
    public class ChestInteractable : MonoBehaviour, IInteractable
    {
        private enum ChestReward
        {
            None,
            FrontCarKey,
            AOESpell,
            CandyOil,
            SecretSyrup,
            Item
        }

        [Header("Chest Settings")]
        [SerializeField] private int chestID = 1;
        [SerializeField] private GameObject closedChestVisual;
        [SerializeField] private GameObject openedChestVisual;

        [Header("Enemy Requirement")]
        [SerializeField] private bool requiresEnemyDefeated;
        [SerializeField] private string requiredEncounterId;

        [Header("Reward")]
        [SerializeField] private ChestReward reward;
        [SerializeField] private string rewardName = "a mysterious item";

        [Header("Item Reward")]
        [SerializeField] private ItemData itemReward;
        [SerializeField] private int itemQuantity = 1;

        [Header("Spell Reward")]
        [SerializeField] private Spell spellReward;

        [Header("Dialogue")]
        [SerializeField] private string speakerName = "Narrator";

        [SerializeField]
        private string guardedMessage =
            "The monster seems to be protecting this chest. You'll have to defeat it first.";

        [SerializeField]
        private string openedMessage =
            "This chest has already been opened.";

        [SerializeField]
        private string chestOpenedMessage =
            "You open the chest and find ";

        [SerializeField]
        private DialogueManager dialogueManager;

        private void Start()
        {
            UpdateChestVisuals();
        }

        public void Interact()
        {
            if (GameProgression.Instance == null)
            {
                Debug.LogWarning("GameProgression instance was not found!");
                return;
            }

            GameProgression progress = GameProgression.Instance;

            if (progress.IsChestOpened(chestID))
            {
                dialogueManager?.ShowDialogue(speakerName, openedMessage);
                return;
            }

            if (dialogueManager == null)
            {
                Debug.LogWarning("Chest is missing its DialogueManager reference!");
                return;
            }

            if (requiresEnemyDefeated &&
                !EnemyEncounter.HasDefeated(requiredEncounterId))
            {
                dialogueManager.ShowDialogue(speakerName, guardedMessage);
                return;
            }

            OpenChest(progress);
        }

        private void OpenChest(GameProgression progress)
        {
            if (!CanGiveReward())
                return;

            if (!GiveReward(progress))
                return;

            progress.MarkChestOpened(chestID);

            UpdateChestVisuals();

            dialogueManager.ShowDialogue(
                speakerName,
                chestOpenedMessage + rewardName + "!"
            );
        }

        private bool CanGiveReward()
        {
            if (reward == ChestReward.Item && itemReward == null)
            {
                Debug.LogWarning("This chest has Item selected but no Item Reward assigned!");
                return false;
            }

            if (reward == ChestReward.AOESpell && spellReward == null)
            {
                Debug.LogWarning("This chest has AOESpell selected but no Spell Reward configured!");
                return false;
            }

            if (reward == ChestReward.Item && itemQuantity <= 0)
            {
                Debug.LogWarning("Chest item quantity must be greater than zero!");
                return false;
            }

            return true;
        }

        private bool GiveReward(GameProgression progress)
        {
            switch (reward)
            {
                case ChestReward.FrontCarKey:
                    progress.GiveFrontCarKey();
                    return true;

                case ChestReward.AOESpell:
                    PlayerCombatProgression player =
                        FindFirstObjectByType<PlayerCombatProgression>();

                    if (player == null)
                    {
                        Debug.LogWarning("Could not find the player's PlayerCombatProgression!");
                        return false;
                    }

                    progress.UnlockAOESpell(player, spellReward);
                    return true;

                case ChestReward.CandyOil:
                    progress.GiveCandyOil();
                    return true;

                case ChestReward.SecretSyrup:
                    progress.GiveSecretSyrup();
                    return true;

                case ChestReward.Item:
                    PlayerInventory inventory =
                        FindFirstObjectByType<PlayerInventory>();

                    if (inventory == null)
                    {
                        Debug.LogWarning("Could not find the player's PlayerInventory!");
                        return false;
                    }

                    inventory.AddItem(itemReward, itemQuantity);
                    return true;
            }

            return true;
        }

        private void UpdateChestVisuals()
        {
            bool isOpened = GameProgression.Instance != null &&
                            GameProgression.Instance.IsChestOpened(chestID);

            if (closedChestVisual != null)
                closedChestVisual.SetActive(!isOpened);

            if (openedChestVisual != null)
                openedChestVisual.SetActive(isOpened);
        }
    }
}
