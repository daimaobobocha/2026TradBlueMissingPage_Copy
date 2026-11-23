
using System;

namespace BattleSystem
{
    public class BattleEvents
    {
        public event Action<BattleUnit> OnTurnStarted;
        public event Action<BattleUnit> OnTurnEnded;

        public event Action<
            BattleUnit,
            BattleUnit,
            int> OnDamageDealt;

        public event Action<
            BattleUnit,
            BattleUnit,
            int> OnHealed;

        public event Action<BattleUnit> OnEnemyAdded;


        public event Action<
            BattleUnit,
            Spell,
            BattleUnit,
            int> OnSpellCast;

        public event Action<BattleUnit> OnDefended;

        public event Action<
            BattleUnit,
            string> OnActionFailed;

        public event Action<
            BattleUnit,
            string> OnMessage;

        public event Action<BattleUnit> OnEnemyChargeStarted;

        public event Action<BattleResult> OnBattleEnded;

        public void TurnStarted(BattleUnit unit)
        {
            OnTurnStarted?.Invoke(unit);
        }

        public void TurnEnded(BattleUnit unit)
        {
            OnTurnEnded?.Invoke(unit);
        }

        public void DamageDealt(
            BattleUnit attacker,
            BattleUnit target,
            int damage)
        {
            OnDamageDealt?.Invoke(attacker, target, damage);
        }

        public void Healed(
            BattleUnit user,
            BattleUnit target,
            int amount)
        {
            OnHealed?.Invoke(user, target, amount);
        }

        public void SpellCast(
            BattleUnit user,
            BattleUnit target,
            Spell spell,
            int damage)
        {
            OnSpellCast?.Invoke(
                user,
                spell,
                target,
                damage
            );
        }

        public void Defended(BattleUnit unit)
        {
            OnDefended?.Invoke(unit);
        }

        public void ActionFailed(
            BattleUnit unit,
            string reason)
        {
            OnActionFailed?.Invoke(unit, reason);
        }

        public void Message(BattleUnit unit, string message)
        {
            OnMessage?.Invoke(unit, message);
        }

        public void EnemyChargeStarted(BattleUnit unit)
        {
            OnEnemyChargeStarted?.Invoke(unit);
        }

        public void BattleEnded(BattleResult result)
        {
            OnBattleEnded?.Invoke(result);
        }

        public void EnemyAdded(BattleUnit enemy)
        {
            OnEnemyAdded?.Invoke(enemy);
        }

    }
}
