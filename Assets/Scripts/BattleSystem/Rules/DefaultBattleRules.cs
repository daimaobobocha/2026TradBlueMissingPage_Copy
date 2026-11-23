using System.Collections.Generic;

namespace BattleSystem
{
    public class DefaultBattleRules : BattleRules
    {
        public override bool CheckBattleEnd(
            BattleController battle,
            List<BattleUnit> players,
            List<BattleUnit> enemies)
        {
            bool playersAlive = false;
            bool enemiesAlive = false;

            foreach (BattleUnit player in players)
            {
                if (player.IsAlive)
                {
                    playersAlive = true;
                    break;
                }
            }

            foreach (BattleUnit enemy in enemies)
            {
                if (enemy.IsAlive)
                {
                    enemiesAlive = true;
                    break;
                }
            }

            if (!playersAlive)
            {
                battle.SetResult(BattleResult.Defeat);
                return true;
            }

            if (!enemiesAlive)
            {
                battle.SetResult(BattleResult.Victory);
                return true;
            }

            return false;
        }
    }
}