
using UnityEngine;
using Exploration;

namespace BattleSystem
{
    public class EnemySummonAction : BattleAction
    {
        public EnemySummonAction(BattleUnit user)
            : base(user)
        {
        }

        public override bool Execute(BattleController battle)
        {
            if (User == null || !User.IsAlive)
                return false;

            EnemyCombatAI enemyAI = battle.GetEnemyAI(User);

            if (enemyAI == null)
                return false;

            GameObject summonPrefab = enemyAI.GetSummonEnemyPrefab();
            int summonCount = enemyAI.GetSummonCount();

            if (summonPrefab == null || summonCount <= 0)
            {
                Debug.LogError(
                    User.UnitName + " attempted to summon enemies, " +
                    "but the summon prefab or count is invalid."
                );

                return false;

            }

            enemyAI.ReleaseSummonAttack();


            for (int i = 0; i < summonCount; i++)
            {

                GameObject summonedObject = UnityEngine.Object.Instantiate(summonPrefab);

                CharacterCombatData characterData =
                    summonedObject.GetComponent<CharacterCombatData>();

                if (characterData == null)
                {
                    Debug.LogError(
                        "Summoned enemy prefab is missing CharacterCombatData."
                    );

                    UnityEngine.Object.Destroy(summonedObject);
                    continue;
                }

                EnemyCombatAI summonedAI =
                    summonedObject.GetComponent<EnemyCombatAI>();

                BattleUnit summonedUnit = new BattleUnit(
                    characterData.CharacterName,
                    BattleTeam.Enemy,
                    characterData.MaxHP,
                    characterData.MaxMana,
                    characterData.AttackPower,
                    characterData.DefensePower
                );

                battle.AddEnemy(summonedUnit, summonedAI);
            }

            battle.NotifyMessage(
                User,
                User.UnitName + " summoned reinforcements!"
            );

            return true;
        }
    }
}
