
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class ItemStack
    {
        public ItemData Item;
        public int Quantity;

        public ItemStack(ItemData item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField]
        private List<ItemStack> items = new List<ItemStack>();

        public IReadOnlyList<ItemStack> Items => items;

        public int GetQuantity(ItemData item)
        {
            ItemStack stack = items.Find(x => x.Item == item);

            return stack != null ? stack.Quantity : 0;
        }

        public bool HasItem(ItemData item, int quantity = 1)
        {
            return GetQuantity(item) >= quantity;
        }

        public void AddItem(ItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return;

            ItemStack stack = items.Find(x => x.Item == item);

            if (stack != null)
            {
                stack.Quantity += quantity;
            }
            else
            {
                items.Add(new ItemStack(item, quantity));
            }
        }

        public bool RemoveItem(ItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return false;

            ItemStack stack = items.Find(x => x.Item == item);

            if (stack == null || stack.Quantity < quantity)
                return false;

            stack.Quantity -= quantity;

            if (stack.Quantity <= 0)
                items.Remove(stack);

            return true;
        }
    }
}
