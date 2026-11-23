
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(
        fileName = "NewItem",
        menuName = "Items/Item"
    )]
    public class ItemData : ScriptableObject
    {
        [Header("Item Information")]
        public string ItemName;
        [TextArea] public string Description;

        [SerializeField] private Sprite itemIcon;

        public Sprite ItemIcon => itemIcon;


        [Header("Consumable Effects")]
        public int HPRestore;
        public int BattleMPRestore;
    }
}
