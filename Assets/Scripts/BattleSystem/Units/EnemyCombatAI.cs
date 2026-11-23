
using UnityEngine;

namespace BattleSystem
{
    public class EnemyCombatAI : MonoBehaviour
    {
        [Header("Special Attack")]
        [SerializeField] private int specialAttackChance = 0;
        [SerializeField] private int specialAttackPower = 30;

        [Header("Charged Attack")]
        [SerializeField] private int chargedAttackChance = 0;
        [SerializeField] private int chargedAttackPower = 300;

        [Header("Charge Tutorial")]
        [SerializeField] private bool showChargeTutorial;
        [SerializeField] private string chargeTutorialSpeaker = "Combat Tutorial";
        [SerializeField]
        private string chargeTutorialMessage =
            "The monster seems to be preparing a powerful attack! Try defending to reduce the damage when it releases a charged attack like this.";

        private bool powerfulAttackCharged;
        private bool hasShownChargeTutorial;

        [Header("Summon Attack")]
        [SerializeField] private int summonAttackChance = 0;
        [SerializeField] private GameObject summonEnemyPrefab;
        [SerializeField] private int summonCount = 2;

        private bool summonAttackCharged;


        public bool ShouldUseSpecialAttack()
        {
            if (specialAttackChance <= 0)
                return false;

            return Random.Range(1, 101) <= specialAttackChance;
        }

        public int GetSpecialAttackPower()
        {
            return specialAttackPower;
        }

        public bool ShouldChargePowerfulAttack()
        {
            if (chargedAttackChance <= 0)
                return false;

            return Random.Range(1, 101) <= chargedAttackChance;
        }

        public void BeginPowerfulAttackCharge()
        {
            powerfulAttackCharged = true;
        }

        public bool IsPowerfulAttackCharged()
        {
            return powerfulAttackCharged;
        }

        public int ReleasePowerfulAttack()
        {
            powerfulAttackCharged = false;
            return chargedAttackPower;
        }

        public bool TryGetChargeTutorial(out string speaker, out string message)
        {
            speaker = chargeTutorialSpeaker;
            message = chargeTutorialMessage;

            if (!showChargeTutorial || hasShownChargeTutorial)
                return false;

            hasShownChargeTutorial = true;
            return true;
        }

        public bool ShouldChargeSummonAttack()
        {
            if (summonAttackChance <= 0)
                return false;

            return Random.Range(1, 101) <= summonAttackChance;
        }

        public void BeginSummonAttackCharge()
        {
            summonAttackCharged = true;
        }

        public bool IsSummonAttackCharged()
        {
            return summonAttackCharged;
        }

        public void ReleaseSummonAttack()
        {
            summonAttackCharged = false;
        }

        public GameObject GetSummonEnemyPrefab()
        {
            return summonEnemyPrefab;
        }

        public int GetSummonCount()
        {
            return summonCount;
        }

    }
}
