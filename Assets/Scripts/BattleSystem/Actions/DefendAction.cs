
namespace BattleSystem
{
    public class DefendAction : BattleAction
    {
        private const int ManaRestored = 5;

        public DefendAction(BattleUnit user)
            : base(user)
        {
        }

        public override bool Execute(BattleController battle)
        {
            if (!User.IsAlive)
                return false;

            User.SetDefending(true);


            if (User.Team == BattleTeam.Player)
                User.RestoreMana(ManaRestored);

            battle.NotifyDefended(User);

            return true;
        }
    }
}
