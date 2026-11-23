
using BattleSystem;

namespace Items
{
    public class ItemAction : BattleAction
    {
        private ItemUseSystem itemUseSystem;
        private ItemData item;

        public ItemAction(
            BattleUnit user,
            ItemUseSystem itemUseSystem,
            ItemData item
        ) : base(user)
        {
            this.itemUseSystem = itemUseSystem;
            this.item = item;
        }

        public override bool Execute(BattleController battle)
        {
            if (User == null || !User.IsAlive)
                return false;

            if (itemUseSystem == null || item == null)
                return false;

            if (!itemUseSystem.UseInBattle(item, User))
                return false;

            battle.NotifyMessage(
                User,
                User.UnitName + " used " + item.ItemName + "!"
            );

            return true;
        }
    }
}
