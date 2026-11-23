
namespace BattleSystem
{
    public class EnemyChargeAction : BattleAction
    {
        public EnemyChargeAction(BattleUnit user)
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

            enemyAI.BeginPowerfulAttackCharge();

            battle.NotifyMessage(
                User,
                User.UnitName + " is gathering power for a devastating attack!"
            );

            battle.NotifyEnemyChargeStarted(User);

            return true;
        }
    }
}
