using System;
using UnityEngine;

namespace BattleSystem
{
    [Serializable]
    public class BattleUnit
    {
        public string UnitName;
        public BattleTeam Team;

        public int MaxHP;
        public int HP;

        public int MaxMana;
        public int Mana;

        public int AttackPower;
        public int DefensePower;

        public bool IsDefending { get; private set; }

        public bool IsAlive => HP > 0;

        public BattleUnit(
            string unitName,
            BattleTeam team,
            int maxHP,
            int maxMana,
            int attackPower,
            int defensePower)
        {
            UnitName = unitName;
            Team = team;

            MaxHP = maxHP;
            HP = maxHP;

            MaxMana = maxMana;
            Mana = maxMana;

            AttackPower = attackPower;
            DefensePower = defensePower;
        }

        public void TakeDamage(int damage)
        {
            damage = Mathf.Max(0, damage);

            if (IsDefending)
            {
                damage = Mathf.CeilToInt(damage * 0.5f);
            }

            HP = Mathf.Max(0, HP - damage);

            if (HP <= 0)
            {
                IsDefending = false;
            }
        }

        public void Heal(int amount)
        {
            HP = Mathf.Min(MaxHP, HP + Mathf.Max(0, amount));
        }

        public bool SpendMana(int amount)
        {
            if (Mana < amount)
                return false;

            Mana -= amount;
            return true;
        }

        public void RestoreMana(int amount)
        {
            Mana = Mathf.Min(MaxMana, Mana + Mathf.Max(0, amount));
        }

        public void SetDefending(bool defending)
        {
            IsDefending = defending;
        }

        public void ResetForNewTurn()
        {
            IsDefending = false;
        }


        public void SetVitals(int hp, int mana)
        {
            HP = Mathf.Clamp(hp, 0, MaxHP);
            Mana = Mathf.Clamp(mana, 0, MaxMana);
        }

    }
}