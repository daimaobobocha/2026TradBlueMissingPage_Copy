
using UnityEngine;
using BattleSystem;

namespace Exploration
{
    public static class BattleEncounterData
    {
        public static CharacterCombatSnapshot Player { get; private set; }
        public static CharacterCombatSnapshot Enemy { get; private set; }

        public static PlayerCombatProgression PlayerProgression { get; private set; }
        public static EnemyCombatAI EnemyAI { get; private set; }

        public static Sprite PlayerSprite { get; private set; }
        public static Sprite PlayerAttackSprite { get; private set; }
        public static Sprite EnemySprite { get; private set; }
        public static Sprite BattleBackground { get; private set; }

        public static string EnemyEncounterId { get; private set; }

        public static bool HasEncounter { get; private set; }

        public static void SetEncounter(
            PlayerCombatProgression playerProgression,
            CharacterCombatData enemy,
            EnemyCombatAI enemyAI,
            Sprite playerSprite,
            Sprite playerAttackSprite,
            Sprite enemySprite,
            Sprite battleBackground,
            string enemyEncounterId)
        {
            CharacterCombatData player =
                playerProgression.GetComponent<CharacterCombatData>();

            PlayerProgression = playerProgression;
            EnemyAI = enemyAI;

            Player = new CharacterCombatSnapshot(
                player.CharacterName,
                player.MaxHP,
                player.MaxMana,
                player.AttackPower,
                player.DefensePower,
                playerProgression.CurrentHP,
                playerProgression.CurrentMana
            );

            Enemy = new CharacterCombatSnapshot(
                enemy.CharacterName,
                enemy.MaxHP,
                enemy.MaxMana,
                enemy.AttackPower,
                enemy.DefensePower,
                enemy.MaxHP,
                enemy.MaxMana
            );

            PlayerSprite = playerSprite;
            PlayerAttackSprite = playerAttackSprite;
            EnemySprite = enemySprite;
            BattleBackground = battleBackground;

            EnemyEncounterId = enemyEncounterId;
            HasEncounter = true;
        }

        public static void Clear()
        {
            Player = null;
            Enemy = null;

            PlayerProgression = null;
            EnemyAI = null;

            PlayerSprite = null;
            PlayerAttackSprite = null;
            EnemySprite = null;
            BattleBackground = null;

            EnemyEncounterId = null;
            HasEncounter = false;
        }
    }

    public class CharacterCombatSnapshot
    {
        public string CharacterName { get; }
        public int MaxHP { get; }
        public int MaxMana { get; }
        public int AttackPower { get; }
        public int DefensePower { get; }
        public int CurrentHP { get; }
        public int CurrentMana { get; }

        public CharacterCombatSnapshot(
            string characterName,
            int maxHP,
            int maxMana,
            int attackPower,
            int defensePower,
            int currentHP,
            int currentMana)
        {
            CharacterName = characterName;
            MaxHP = maxHP;
            MaxMana = maxMana;
            AttackPower = attackPower;
            DefensePower = defensePower;
            CurrentHP = currentHP;
            CurrentMana = currentMana;
        }
    }
}
