
using UnityEngine;

namespace BattleSystem
{
    public class SpecialAttackAction : BattleAction
    {
        private readonly BattleUnit target;
        private readonly int damageMultiplier;

        public SpecialAttackAction(
            BattleUnit user,
            BattleUnit target,
            int damageMultiplier)
            : base(user)
        {
            this.target = target;
            this.damageMultiplier = damageMultiplier;
        }

        public override bool Execute(BattleController battle)
        {
            if (User == null || !User.IsAlive ||
                target == null || !target.IsAlive)
            {
                return false;
            }

            int damage = Mathf.Max(
                1,
                Mathf.RoundToInt(
                    User.AttackPower * (damageMultiplier / 100f)
                )
            );

            target.TakeDamage(damage);

            battle.NotifyDamageDealt(User, target, damage);

            return true;
        }
    }
}
