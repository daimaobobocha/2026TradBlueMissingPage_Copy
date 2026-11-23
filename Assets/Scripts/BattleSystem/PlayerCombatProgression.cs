
using BattleSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Exploration
{
    [RequireComponent(typeof(CharacterCombatData))]
    public class PlayerCombatProgression : MonoBehaviour
    {
        [Header("Starting Resources")]
        [SerializeField] private int startingMana = 20;

        [Header("Known Spells")]
        [SerializeField] private List<Spell> knownSpells = new List<Spell>();

        private CharacterCombatData combatData;

        private int currentHP;
        private int currentMana;
        private bool initialized;

        public int CurrentHP => currentHP;
        public int CurrentMana => currentMana;

        public int MaxHP => combatData.MaxHP;
        public int MaxMana => combatData.MaxMana;

        public List<Spell> KnownSpells => knownSpells;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized)
                return;

            combatData = GetComponent<CharacterCombatData>();

            currentHP = combatData.MaxHP;
            currentMana = Mathf.Clamp(startingMana, 0, combatData.MaxMana);

            initialized = true;
        }

        public void TakeDamage(int amount)
        {
            currentHP = Mathf.Max(0, currentHP - amount);
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(MaxHP, currentHP + amount);
        }

        public bool SpendMana(int amount)
        {
            if (currentMana < amount)
                return false;

            currentMana -= amount;
            return true;
        }

        public void RestoreMana(int amount)
        {
            currentMana = Mathf.Min(MaxMana, currentMana + amount);
        }

        public void UpdateVitals(int hp, int mana)
        {
            currentHP = Mathf.Clamp(hp, 0, MaxHP);
            currentMana = Mathf.Clamp(mana, 0, MaxMana);
        }

        public bool KnowsSpell(Spell spell)
        {
            return knownSpells.Contains(spell);
        }

        public void LearnSpell(Spell spell)
        {
            if (spell != null && !knownSpells.Contains(spell))
                knownSpells.Add(spell);
        }
    }
}
