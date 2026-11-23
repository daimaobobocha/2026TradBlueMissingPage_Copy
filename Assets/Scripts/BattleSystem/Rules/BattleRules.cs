using System.Collections.Generic;

namespace BattleSystem
{
    public abstract class BattleRules
    {
        public virtual void OnBattleStart(
            BattleController battle)
        {
        }

        public virtual bool CanUseAction(
            BattleController battle,
            BattleAction action)
        {
            return true;
        }

        public virtual void OnTurnStart(
            BattleController battle,
            BattleUnit unit)
        {
        }

        public virtual void OnTurnEnd(
            BattleController battle,
            BattleUnit unit)
        {
        }

        public virtual void OnBattleEnd(
            BattleController battle,
            BattleResult result)
        {
        }

        public virtual bool CheckBattleEnd(
            BattleController battle,
            List<BattleUnit> players,
            List<BattleUnit> enemies)
        {
            return false;
        }
    }
}