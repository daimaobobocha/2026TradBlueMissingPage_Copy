
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public enum SpellEffectType
    {
        Damage,
        Heal
    }

    public enum SpellTargetType
    {
        SingleEnemy,
        AllEnemies,
        Self
    }

    [Serializable]
    public class Spell
    {
        public string SpellName;
        public int ManaCost;
        public int Power;
        public SpellEffectType EffectType;
        public SpellTargetType TargetType;

        public Spell(
            string spellName,
            int manaCost,
            int power,
            SpellEffectType effectType,
            SpellTargetType targetType = SpellTargetType.SingleEnemy)
        {
            SpellName = spellName;
            ManaCost = manaCost;
            Power = power;
            EffectType = effectType;
            TargetType = targetType;
        }
    }

    public class SpellAction : BattleAction
    {
        private readonly Spell spell;
        private readonly BattleUnit target;

        public SpellAction(
            BattleUnit user,
            Spell spell,
            BattleUnit target = null)
            : base(user)
        {
            this.spell = spell;
            this.target = target;
        }

        public override bool Execute(BattleController battle)
        {
            if (!User.IsAlive || spell == null)
                return false;

            if (spell.EffectType == SpellEffectType.Damage)
            {
                if (spell.TargetType != SpellTargetType.AllEnemies &&
                    (target == null || !target.IsAlive))
                    return false;

                if (!User.SpendMana(spell.ManaCost))
                {
                    battle.NotifyActionFailed(User, "Not enough mana!");
                    return false;
                }

                int damage = Mathf.Max(
                    1,
                    User.AttackPower * spell.Power / 100
                );

                if (spell.TargetType == SpellTargetType.AllEnemies)
                {
                    List<BattleUnit> enemies = battle.GetAliveEnemies();

                    if (enemies.Count == 0)
                        return false;

                    foreach (BattleUnit enemy in enemies)
                    {
                        enemy.TakeDamage(damage);

                        battle.NotifySpellCast(
                            User,
                            enemy,
                            spell,
                            damage
                        );
                    }

                    return true;
                }

                target.TakeDamage(damage);

                battle.NotifySpellCast(
                    User,
                    target,
                    spell,
                    damage
                );

                return true;
            }

            if (spell.EffectType == SpellEffectType.Heal)
            {
                BattleUnit healTarget =
                    spell.TargetType == SpellTargetType.Self
                        ? User
                        : target;

                if (healTarget == null || !healTarget.IsAlive)
                    return false;

                if (!User.SpendMana(spell.ManaCost))
                {
                    battle.NotifyActionFailed(User, "Not enough mana!");
                    return false;
                }

                int healing = Mathf.Max(
                    1,
                    User.AttackPower * spell.Power / 100
                );

                int previousHP = healTarget.HP;

                healTarget.Heal(healing);

                int healed = healTarget.HP - previousHP;

                battle.NotifyHealed(
                    User,
                    healTarget,
                    healed
                );

                return true;
            }

            return false;
        }
    }
}
