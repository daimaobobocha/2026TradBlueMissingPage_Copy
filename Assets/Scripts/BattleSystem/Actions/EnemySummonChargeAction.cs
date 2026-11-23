
namespace BattleSystem
{
    public class EnemySummonChargeAction : BattleAction
    {
        public EnemySummonChargeAction(BattleUnit user)
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

            enemyAI.BeginSummonAttackCharge();

            battle.NotifyMessage(
                User,
                User.UnitName + " is calling for reinforcements!"
            );

            return true;
        }
    }
}
