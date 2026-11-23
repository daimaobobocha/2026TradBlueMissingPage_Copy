
using UnityEngine;
using BattleSystem;
using Exploration;

namespace Items
{
    public class ItemUseSystem : MonoBehaviour
    {
        private PlayerInventory inventory;
        private PlayerCombatProgression progression;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            progression = GetComponent<PlayerCombatProgression>();
        }

        public bool UseInExploration(ItemData item)
        {
            if (item == null || inventory == null || progression == null)
                return false;

            if (!inventory.HasItem(item))
                return false;

            if (item.HPRestore <= 0 ||
                progression.CurrentHP >= progression.MaxHP)
                return false;

            if (!inventory.RemoveItem(item))
                return false;

            progression.Heal(item.HPRestore);

            return true;
        }

        public bool UseInBattle(ItemData item, BattleUnit target)
        {
            if (item == null || inventory == null || target == null)
                return false;

            if (target.Team != BattleTeam.Player || !target.IsAlive)
                return false;

            if (!inventory.HasItem(item))
                return false;

            bool canRestoreHP =
                item.HPRestore > 0 && target.HP < target.MaxHP;

            bool canRestoreMP =
                item.BattleMPRestore > 0 && target.Mana < target.MaxMana;

            if (!canRestoreHP && !canRestoreMP)
                return false;

            if (!inventory.RemoveItem(item))
                return false;

            if (canRestoreHP)
                target.Heal(item.HPRestore);

            if (canRestoreMP)
                target.RestoreMana(item.BattleMPRestore);

            return true;
        }
    }
}
