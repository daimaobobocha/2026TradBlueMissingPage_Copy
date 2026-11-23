namespace BattleSystem
{
    public abstract class BattleAction
    {
        public BattleUnit User { get; }

        protected BattleAction(BattleUnit user)
        {
            User = user;
        }

        public abstract bool Execute(
            BattleController battle);
    }
}