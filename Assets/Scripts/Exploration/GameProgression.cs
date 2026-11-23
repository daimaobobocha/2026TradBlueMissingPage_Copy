
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;

namespace Exploration
{
    public class GameProgression : MonoBehaviour
    {
        public static GameProgression Instance { get; private set; }

        public bool HasCandyOil { get; private set; }
        public bool HasSecretSyrup { get; private set; }
        public bool HasImprovedCandyOil { get; private set; }

        public bool HasFrontCarKey { get; private set; }
        public bool HasAOESpell { get; private set; }

        public bool OpenedValve { get; private set; }
        public bool PulledTrainLever { get; private set; }

        public bool KnowAboutCandyOilMan { get; private set; }
        public bool KnowAboutSecretNote { get; private set; }
        public bool ReadSecretNote { get; private set; }
        public bool TakenCandy { get; private set; }

        private List<int> openedChestIDs = new List<int>();
        private List<int> defeatedEnemyIDs = new List<int>();
        // Car 1 is the player's starting carriage. Other cars unlock on arrival.
        private readonly HashSet<int> visitedTrainCars = new HashSet<int> { 1 };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Chest Progression

        public bool IsChestOpened(int chestID)
        {
            return openedChestIDs.Contains(chestID);
        }

        public void MarkChestOpened(int chestID)
        {
            if (IsChestOpened(chestID))
                return;

            openedChestIDs.Add(chestID);
            Debug.Log("Chest " + chestID + " has been opened!");
        }

        // Enemy Progression

        public bool IsEnemyDefeated(int enemyID)
        {
            return defeatedEnemyIDs.Contains(enemyID);
        }

        public void MarkEnemyDefeated(int enemyID)
        {
            if (IsEnemyDefeated(enemyID))
                return;

            defeatedEnemyIDs.Add(enemyID);
            Debug.Log("Enemy " + enemyID + " has been defeated!");
        }

        // Candy Oil Quest

        public void GiveCandyOil()
        {
            HasCandyOil = true;
            Debug.Log("Received Candy Oil!");
        }

        public void GiveSecretSyrup()
        {
            if (HasSecretSyrup || HasImprovedCandyOil)
                return;

            HasSecretSyrup = true;
            Debug.Log("Received Secret Syrup!");
        }

        public bool MakeImprovedCandyOil(CharacterCombatData player)
        {
            if (!HasSecretSyrup || HasImprovedCandyOil)
                return false;

            if (player == null)
            {
                Debug.LogWarning("Could not improve Candy Oil: Player CharacterCombatData was not found!");
                return false;
            }

            HasSecretSyrup = false;
            HasImprovedCandyOil = true;

            player.IncreaseAttackPower(4);

            Debug.Log("Created Improved Candy Oil!");

            return true;
        }

        // Other Rewards

        public void GiveFrontCarKey()
        {
            HasFrontCarKey = true;
            Debug.Log("Received front car key!");
        }

        public void UnlockAOESpell(
            PlayerCombatProgression player,
            Spell spell)
        {
            HasAOESpell = true;

            if (player == null || spell == null)
            {
                Debug.LogWarning("Could not teach AOE spell: player or spell is missing!");
                return;
            }

            player.LearnSpell(spell);
            Debug.Log("Unlocked AOE spell: " + spell.SpellName);
        }

        // Train Progression

        public bool HasVisitedTrainCar(int carNumber)
        {
            return visitedTrainCars.Contains(carNumber);
        }

        public void MarkTrainCarVisited(int carNumber)
        {
            if (carNumber > 0)
                visitedTrainCars.Add(carNumber);
        }

        public void MarkTrainLeverPulled()
        {
            if (PulledTrainLever)
                return;

            PulledTrainLever = true;
            Debug.Log("The train has been stopped at the North Pole!");
        }

        // Existing Story Progression

        public void LearnAboutCandyOilMan()
        {
            KnowAboutCandyOilMan = true;
            Debug.Log("Learned About Candy Oil Man!");
        }

        public void LearnAboutSecretNote()
        {
            KnowAboutSecretNote = true;
        }

        public void ReadTheSecretNote()
        {
            ReadSecretNote = true;
        }

        public void TakePieceOfCandy()
        {
            TakenCandy = true;
        }

        public void OpenUpValve()
        {
            OpenedValve = true;
        }

        public void UseCandyOil()
        {
            HasCandyOil = false;
            Debug.Log("Candy Oil was used!");
        }

        // Demo Reset

        public void ResetProgression()
        {
            HasCandyOil = false;
            HasSecretSyrup = false;
            HasImprovedCandyOil = false;

            HasFrontCarKey = false;
            HasAOESpell = false;

            OpenedValve = false;
            PulledTrainLever = false;

            KnowAboutCandyOilMan = false;
            KnowAboutSecretNote = false;
            ReadSecretNote = false;
            TakenCandy = false;

            openedChestIDs.Clear();
            defeatedEnemyIDs.Clear();
            visitedTrainCars.Clear();
            visitedTrainCars.Add(1);

            Debug.Log("Game progression has been reset!");
        }
    }
}
