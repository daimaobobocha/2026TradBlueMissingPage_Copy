
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using BattleSystem;

namespace Exploration
{
    public class EnemyEncounter : MonoBehaviour, IInteractable
    {
        [Header("Encounter")]
        [SerializeField] private string battleSceneName = "BattleScene";
        [SerializeField] private string encounterId = "ForestEnemy1";
        [SerializeField] private SpriteRenderer battleBackground;

        [Header("Dialogue")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private string enemySpeakerName = "Enemy";
        [SerializeField]
        private string introductionDialogue =
            "You there! Prepare yourself!";
        [SerializeField]
        private string fightResponse =
            "Then let's settle this!";
        [SerializeField]
        private string leaveResponse =
            "Leaving so soon?";

        private bool encounterTriggered;

        public static string PreviousSceneName { get; private set; }

        private static bool battleLoading;

        private static readonly HashSet<string> defeatedEncounters =
            new HashSet<string>();

        private void Start()
        {
            if (defeatedEncounters.Contains(encounterId))
                gameObject.SetActive(false);
        }

        public void Interact()
        {
            if (encounterTriggered || battleLoading)
                return;

            if (SceneManager.GetSceneByName(battleSceneName).isLoaded)
                return;

            if (dialogueManager == null)
            {
                Debug.LogError(
                    "EnemyEncounter is missing its DialogueManager reference."
                );

                return;
            }

            dialogueManager.ShowChoiceDialogue(
                enemySpeakerName,
                introductionDialogue,
                "Fight",
                fightResponse,
                "Leave",
                leaveResponse,
                HandleDialogueChoice
            );
        }

        private void HandleDialogueChoice(int choice)
        {
            if (choice != 0)
                return;

            StartBattle();
        }

        private void StartBattle()
        {
            if (encounterTriggered || battleLoading)
                return;

            // Validate before locking encounters or storing battle data.
            if (!Application.CanStreamedLevelBeLoaded(battleSceneName))
            {
                Debug.LogError(
                    "Cannot start battle: enable scene '" + battleSceneName +
                    "' in the Build Profiles scene list."
                );
                return;
            }

            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
            {
                Debug.LogError("Could not find the Player.");
                return;
            }

            CharacterCombatData playerData =
                playerObject.GetComponent<CharacterCombatData>();

            PlayerCombatProgression playerProgression =
                playerObject.GetComponent<PlayerCombatProgression>();

            CharacterCombatData enemyData =
                GetComponent<CharacterCombatData>();

            EnemyCombatAI enemyAI =
                GetComponent<EnemyCombatAI>();

            SpriteRenderer playerRenderer =
                playerObject.GetComponent<SpriteRenderer>();

            SpriteRenderer enemyRenderer =
                GetComponent<SpriteRenderer>();

            if (playerData == null || playerProgression == null)
            {
                Debug.LogError(
                    "Player is missing combat components."
                );

                return;
            }

            if (enemyData == null || playerRenderer == null ||
                enemyRenderer == null || battleBackground == null)
            {
                Debug.LogError(
                    "EnemyEncounter is missing a required component or reference."
                );

                return;
            }

            encounterTriggered = true;
            battleLoading = true;

            PreviousSceneName =
                SceneManager.GetActiveScene().name;

            BattleEncounterData.SetEncounter(
                playerProgression,
                enemyData,
                enemyAI,
                playerRenderer.sprite,
                playerData.BattleAttackSprite,
                enemyRenderer.sprite,
                battleBackground.sprite,
                encounterId
            );

            Debug.Log(
                "Starting battle against " + enemyData.CharacterName
            );

            StartCoroutine(LoadBattleScene());
        }

        private IEnumerator LoadBattleScene()
        {
            AsyncOperation operation = null;

            try
            {
                operation = SceneManager.LoadSceneAsync(
                    battleSceneName,
                    LoadSceneMode.Additive
                );
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "Could not load battle scene '" + battleSceneName +
                    "': " + exception.Message
                );
            }

            if (operation == null)
            {
                Debug.LogError(
                    "Could not load battle scene: " + battleSceneName
                );

                ResetFailedBattle();
                yield break;
            }

            while (!operation.isDone)
                yield return null;

            Scene battleScene =
                SceneManager.GetSceneByName(battleSceneName);

            if (battleScene.IsValid() && battleScene.isLoaded)
            {
                SceneManager.SetActiveScene(battleScene);
                Debug.Log("Battle scene loaded successfully.");
            }
            else
            {
                Debug.LogError(
                    "Battle scene failed to load correctly."
                );

                ResetFailedBattle();
                yield break;
            }

            battleLoading = false;
        }

        private void ResetFailedBattle()
        {
            battleLoading = false;
            encounterTriggered = false;
            PreviousSceneName = null;
            BattleEncounterData.Clear();
        }

        public static bool HasDefeated(string id)
        {
            return !string.IsNullOrEmpty(id) &&
                   defeatedEncounters.Contains(id);
        }

        public static void MarkDefeated(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            defeatedEncounters.Add(id);

            EnemyEncounter[] encounters =
                FindObjectsByType<EnemyEncounter>(
                    FindObjectsSortMode.None
                );

            foreach (EnemyEncounter encounter in encounters)
            {
                if (encounter.encounterId == id)
                    encounter.gameObject.SetActive(false);
            }

            Debug.Log("Encounter defeated: " + id);
        }
    }
}
