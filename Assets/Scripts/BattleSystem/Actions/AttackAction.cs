
namespace BattleSystem
{
    public class AttackAction : BattleAction
    {
        private readonly BattleUnit target;

        private const int ManaRestored = 5;

        public AttackAction(BattleUnit user, BattleUnit target)
            : base(user)
        {
            this.target = target;
        }

        public override bool Execute(BattleController battle)
        {
            if (!User.IsAlive || target == null || !target.IsAlive)
                return false;

            int damage = User.AttackPower;

            target.TakeDamage(damage);

            battle.NotifyDamageDealt(User, target, damage);


            if (User.Team == BattleTeam.Player)
                User.RestoreMana(ManaRestored);

            return true;
        }
    }
}
