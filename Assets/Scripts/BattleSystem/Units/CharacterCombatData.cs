
using UnityEngine;

namespace Exploration
{
    public class CharacterCombatData : MonoBehaviour
    {
        [SerializeField]
        private string characterName = "Hero";

        [SerializeField]
        private int maxHP = 100;

        [SerializeField]
        private int maxMana = 50;

        [SerializeField]
        private int attackPower = 20;

        [SerializeField]
        private int defensePower = 10;

        [Header("Battle Sprites")]
        [SerializeField]
        private Sprite battleAttackSprite;

        public string CharacterName => characterName;
        public int MaxHP => maxHP;
        public int MaxMana => maxMana;
        public int AttackPower => attackPower;
        public int DefensePower => defensePower;

        public Sprite BattleAttackSprite => battleAttackSprite;

        public void IncreaseAttackPower(int amount)
        {
            if (amount <= 0)
                return;

            attackPower += amount;

            Debug.Log(
                characterName + "'s Attack Power increased to " + attackPower + "!"
            );
        }
    }
}
