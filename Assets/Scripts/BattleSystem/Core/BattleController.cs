
using System.Collections.Generic;
using System.Linq;

namespace BattleSystem
{
    public class BattleController
    {
        public BattleState State { get; private set; }
        public BattleResult Result { get; private set; }

        public List<BattleUnit> Players { get; }
        public List<BattleUnit> Enemies { get; }

        public BattleUnit CurrentUnit { get; private set; }

        public BattleEvents Events { get; }

        private BattleRules rules;

        private int playerTurnIndex;
        private int enemyTurnIndex;

        private Dictionary<BattleUnit, EnemyCombatAI> enemyAIs =
            new Dictionary<BattleUnit, EnemyCombatAI>();

        public BattleController(
            List<BattleUnit> players,
            List<BattleUnit> enemies,
            BattleRules rules = null)
        {
            Players = players;
            Enemies = enemies;

            this.rules = rules ?? new DefaultBattleRules();

            Events = new BattleEvents();

            State = BattleState.NotStarted;
            Result = BattleResult.None;
        }

        // START BATTLE

        public void StartBattle()
        {
            State = BattleState.PlayerTurn;

            playerTurnIndex = 0;
            enemyTurnIndex = 0;

            rules.OnBattleStart(this);

            StartNextPlayerTurn();
        }

        // PLAYER ACTION

        public void ExecutePlayerAction(BattleAction action)
        {
            if (State != BattleState.PlayerTurn)
                return;

            if (CurrentUnit == null || !CurrentUnit.IsAlive)
            {
                StartNextPlayerTurn();
                return;
            }

            if (action == null)
                return;

            if (!rules.CanUseAction(this, action))
                return;

            bool actionSucceeded = action.Execute(this);

            if (!actionSucceeded)
                return;

            if (CheckBattleEnd())
                return;

            EndCurrentTurn();
        }

        // PLAYER TURN

        private void StartNextPlayerTurn()
        {
            while (playerTurnIndex < Players.Count)
            {
                BattleUnit unit = Players[playerTurnIndex];

                playerTurnIndex++;

                if (!unit.IsAlive)
                    continue;

                StartTurn(unit);
                return;
            }

            playerTurnIndex = 0;
            StartEnemyPhase();
        }

        // ENEMY PHASE

        private void StartEnemyPhase()
        {
            State = BattleState.EnemyTurn;
            enemyTurnIndex = 0;

            StartNextEnemyTurn();
        }

        private void StartNextEnemyTurn()
        {
            while (enemyTurnIndex < Enemies.Count)
            {
                BattleUnit enemy = Enemies[enemyTurnIndex];

                enemyTurnIndex++;

                if (!enemy.IsAlive)
                    continue;

                StartTurn(enemy);
                return;
            }

            enemyTurnIndex = 0;
            playerTurnIndex = 0;

            State = BattleState.PlayerTurn;

            StartNextPlayerTurn();
        }

        // ENEMY ACTION



        public void ExecuteEnemyTurn()
        {
            if (State != BattleState.EnemyTurn)
                return;

            if (CurrentUnit == null || !CurrentUnit.IsAlive)
            {
                EndCurrentTurn();
                return;
            }

            BattleUnit target = GetAlivePlayers().FirstOrDefault();

            if (target == null)
            {
                CheckBattleEnd();
                return;
            }

            EnemyCombatAI enemyAI = GetEnemyAI(CurrentUnit);

            if (enemyAI != null)
            {
                // Release a previously charged powerful attack.
                if (enemyAI.IsPowerfulAttackCharged())
                {
                    int attackPower = enemyAI.ReleasePowerfulAttack();

                    ExecuteEnemyAction(
                        new SpecialAttackAction(
                            CurrentUnit,
                            target,
                            attackPower
                        )
                    );

                    return;
                }

                // Release a previously charged summon attack.
                if (enemyAI.IsSummonAttackCharged())
                {
                    if (GetAliveEnemies().Count > 1)
                    {
                        enemyAI.ReleaseSummonAttack();

                        ExecuteEnemyAction(
                            new AttackAction(CurrentUnit, target)
                        );

                        return;
                    }

                    ExecuteEnemyAction(
                        new EnemySummonAction(CurrentUnit)
                    );

                    return;
                }

                // Possibly charge a powerful attack.
                if (enemyAI.ShouldChargePowerfulAttack())
                {
                    ExecuteEnemyAction(
                        new EnemyChargeAction(CurrentUnit)
                    );

                    return;
                }

                // Possibly charge a summon attack.
                // Only summon if no other enemies are alive.
                if (GetAliveEnemies().Count <= 1 &&
                    enemyAI.ShouldChargeSummonAttack())
                {
                    ExecuteEnemyAction(
                        new EnemySummonChargeAction(CurrentUnit)
                    );

                    return;
                }

                // Possibly use a normal special attack.
                if (enemyAI.ShouldUseSpecialAttack())
                {
                    ExecuteEnemyAction(
                        new SpecialAttackAction(
                            CurrentUnit,
                            target,
                            enemyAI.GetSpecialAttackPower()
                        )
                    );

                    return;
                }
            }

            ExecuteEnemyAction(
                new AttackAction(CurrentUnit, target)
            );
        }



        public void ExecuteEnemyAction(BattleAction action)
        {
            if (State != BattleState.EnemyTurn)
                return;

            if (CurrentUnit == null || !CurrentUnit.IsAlive)
            {
                EndCurrentTurn();
                return;
            }

            if (action == null)
            {
                EndCurrentTurn();
                return;
            }

            bool actionSucceeded = action.Execute(this);

            if (!actionSucceeded)
            {
                EndCurrentTurn();
                return;
            }

            if (CheckBattleEnd())
                return;

            EndCurrentTurn();
        }

        // ADD ENEMIES DURING BATTLE



        public void AddEnemy(BattleUnit enemy, EnemyCombatAI enemyAI)
        {
            if (enemy == null)
                return;

            if (enemy.Team != BattleTeam.Enemy)
                return;

            Enemies.Add(enemy);

            if (enemyAI != null)
                RegisterEnemyAI(enemy, enemyAI);

            Events.EnemyAdded(enemy);
        }


        // TURN START

        private void StartTurn(BattleUnit unit)
        {
            CurrentUnit = unit;

            unit.ResetForNewTurn();

            rules.OnTurnStart(this, unit);

            Events.TurnStarted(unit);
        }

        // TURN END

        private void EndCurrentTurn()
        {
            if (CurrentUnit != null)
            {
                rules.OnTurnEnd(this, CurrentUnit);

                Events.TurnEnded(CurrentUnit);
            }

            CurrentUnit = null;

            if (State == BattleState.PlayerTurn)
            {
                StartNextPlayerTurn();
            }
            else if (State == BattleState.EnemyTurn)
            {
                StartNextEnemyTurn();
            }
        }

        // BATTLE END

        private bool CheckBattleEnd()
        {
            return rules.CheckBattleEnd(this, Players, Enemies);
        }

        public void SetResult(BattleResult result)
        {
            Result = result;

            if (result == BattleResult.Victory)
                State = BattleState.Victory;
            else if (result == BattleResult.Defeat)
                State = BattleState.Defeat;

            Events.BattleEnded(result);

            rules.OnBattleEnd(this, result);
        }

        // UNIT HELPERS

        public List<BattleUnit> GetAliveEnemies()
        {
            return Enemies.Where(x => x.IsAlive).ToList();
        }

        public List<BattleUnit> GetAlivePlayers()
        {
            return Players.Where(x => x.IsAlive).ToList();
        }

        // EVENT NOTIFICATIONS

        public void NotifyDamageDealt(
            BattleUnit attacker,
            BattleUnit target,
            int damage)
        {
            Events.DamageDealt(attacker, target, damage);
        }

        public void NotifyHealed(
            BattleUnit user,
            BattleUnit target,
            int amount)
        {
            Events.Healed(user, target, amount);
        }

        public void NotifySpellCast(
            BattleUnit user,
            BattleUnit target,
            Spell spell,
            int damage)
        {
            Events.SpellCast(user, target, spell, damage);
        }

        public void NotifyDefended(BattleUnit unit)
        {
            Events.Defended(unit);
        }

        public void NotifyActionFailed(BattleUnit unit, string reason)
        {
            Events.ActionFailed(unit, reason);
        }

        public void RegisterEnemyAI(
            BattleUnit enemy,
            EnemyCombatAI enemyAI)
        {
            if (enemy == null || enemyAI == null)
                return;

            enemyAIs[enemy] = enemyAI;
        }

        public EnemyCombatAI GetEnemyAI(BattleUnit enemy)
        {
            if (enemy == null)
                return null;

            enemyAIs.TryGetValue(
                enemy,
                out EnemyCombatAI enemyAI
            );

            return enemyAI;
        }

        public void NotifyMessage(BattleUnit unit, string message)
        {
            Events.Message(unit, message);
        }

        public void NotifyEnemyChargeStarted(BattleUnit unit)
        {
            Events.EnemyChargeStarted(unit);
        }


    }
}
