
using Exploration;
using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleSystem
{
    public class BattleTest : MonoBehaviour
    {
        [Header("Battle Presentation")]
        [SerializeField] private SpriteRenderer battleBackground;
        [SerializeField] private SpriteRenderer playerBattleSprite;
        [SerializeField] private SpriteRenderer enemyBattleSprite;
        [SerializeField] private BattleUI battleUI;

        [SerializeField] private float actionPause = 1.2f;
        [SerializeField] private float enemyTurnPause = 1.2f;
        [SerializeField] private float battleEndPause = 1.5f;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip playerAttackSound;
        [SerializeField] private AudioClip enemyAttackSound;

        private AudioSource audioSource;
        private BattleController battle;
        private DialogueManager dialogueManager;

        private ItemUseSystem itemUseSystem;
        private PlayerInventory playerInventory;

        private bool waitingForTurn;
        private bool battleDialogueActive;
        private bool hasShownCombatIntro;

        private Sprite playerNormalSprite;
        private Sprite playerAttackSprite;

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            dialogueManager = FindObjectOfType<DialogueManager>();

            if (dialogueManager == null)
            {
                Debug.LogError(
                    "BattleTest could not find the exploration DialogueManager."
                );
            }
            else
            {
                dialogueManager.OnDialogueEnded += HandleBattleDialogueEnded;
            }

            if (!BattleEncounterData.HasEncounter)
            {
                Debug.LogError("BattleTest started without an encounter.");
                return;
            }

            SetupBattlePresentation();
            CreateBattle();
        }

        private void OnDestroy()
        {
            if (dialogueManager != null)
                dialogueManager.OnDialogueEnded -= HandleBattleDialogueEnded;
        }

        private void SetupBattlePresentation()
        {
            playerNormalSprite = BattleEncounterData.PlayerSprite;
            playerAttackSprite = BattleEncounterData.PlayerAttackSprite;

            if (battleBackground != null)
                battleBackground.sprite = BattleEncounterData.BattleBackground;

            if (playerBattleSprite != null)
                playerBattleSprite.sprite = playerNormalSprite;

            if (enemyBattleSprite != null)
                enemyBattleSprite.sprite = BattleEncounterData.EnemySprite;
        }

        private void CreateBattle()
        {
            CharacterCombatSnapshot playerData = BattleEncounterData.Player;
            CharacterCombatSnapshot enemyData = BattleEncounterData.Enemy;

            List<BattleUnit> players = new List<BattleUnit>
            {
                new BattleUnit(
                    playerData.CharacterName,
                    BattleTeam.Player,
                    playerData.MaxHP,
                    playerData.MaxMana,
                    playerData.AttackPower,
                    playerData.DefensePower
                )
            };

            players[0].SetVitals(
                playerData.CurrentHP,
                playerData.CurrentMana
            );

            List<BattleUnit> enemies = new List<BattleUnit>
            {
                new BattleUnit(
                    enemyData.CharacterName,
                    BattleTeam.Enemy,
                    enemyData.MaxHP,
                    enemyData.MaxMana,
                    enemyData.AttackPower,
                    enemyData.DefensePower
                )
            };

            battle = new BattleController(players, enemies);

            battle.RegisterEnemyAI(
                enemies[0],
                BattleEncounterData.EnemyAI
            );

            SubscribeToEvents();

            if (battleUI == null)
            {
                Debug.LogError("BattleUI is not assigned on BattleTest.");
                return;
            }

            battleUI.Initialize(battle);

            battleUI.SetAvailableSpells(
                BattleEncounterData.PlayerProgression.KnownSpells
            );

            itemUseSystem =
                BattleEncounterData.PlayerProgression.GetComponent<ItemUseSystem>();

            playerInventory =
                BattleEncounterData.PlayerProgression.GetComponent<PlayerInventory>();

            if (itemUseSystem == null || playerInventory == null)
            {
                Debug.LogError(
                    "BattleTest could not find the player's item system or inventory."
                );
            }
            else
            {
                battleUI.SetAvailableItems(playerInventory);
            }

            battleUI.AttackTargetSelected += HandleAttackTargetSelected;
            battleUI.DefendConfirmed += HandleDefendConfirmed;
            battleUI.SpellTargetSelected += HandleSpellTargetSelected;
            battleUI.ItemSelected += HandleItemSelected;

            battle.Events.OnMessage += HandleBattleMessage;
            battle.Events.OnEnemyChargeStarted += HandleEnemyChargeStarted;
            battle.Events.OnEnemyAdded += HandleEnemyAdded;

            battleUI.AddBattleMessage("The battle begins!");

            battle.StartBattle();
            battleUI.RefreshStatus(battle);
        }

        private void SubscribeToEvents()
        {
            battle.Events.OnDamageDealt += HandleDamage;
            battle.Events.OnHealed += HandleHeal;
            battle.Events.OnSpellCast += HandleSpell;
            battle.Events.OnDefended += HandleDefend;
            battle.Events.OnActionFailed += HandleActionFailed;
            battle.Events.OnTurnStarted += HandleTurnStarted;
            battle.Events.OnBattleEnded += HandleBattleEnded;
        }

        private void HandleEnemyAdded(BattleUnit enemy)
        {
            battleUI.RefreshEnemyDisplays(battle);

            battleUI.AddBattleMessage(
                enemy.UnitName + " joined the battle!"
            );
        }

        private void HandleDamage(
            BattleUnit attacker,
            BattleUnit target,
            int damage)
        {
            battleUI.AddBattleMessage(
                attacker.UnitName + " hits " + target.UnitName +
                " for " + damage + " damage!"
            );

            battleUI.RefreshStatus(battle);
        }

        private void HandleEnemyChargeStarted(BattleUnit enemy)
        {
            EnemyCombatAI enemyAI = battle.GetEnemyAI(enemy);

            if (enemyAI == null)
                return;

            if (!enemyAI.TryGetChargeTutorial(out string speaker, out string message))
                return;

            ShowBattleDialogue(speaker, message);
        }

        private void HandleBattleMessage(BattleUnit unit, string message)
        {
            battleUI.AddBattleMessage(message);
            battleUI.RefreshStatus(battle);
        }

        private void HandleHeal(
            BattleUnit user,
            BattleUnit target,
            int amount)
        {
            battleUI.AddBattleMessage(
                user.UnitName + " heals " + target.UnitName +
                " for " + amount + " HP!"
            );

            battleUI.RefreshStatus(battle);
        }

        private void HandleSpell(
            BattleUnit user,
            Spell spell,
            BattleUnit target,
            int damage)
        {
            battleUI.AddBattleMessage(
                user.UnitName + " casts " + spell.SpellName +
                " on " + target.UnitName + "!"
            );

            battleUI.RefreshStatus(battle);
        }

        private void HandleDefend(BattleUnit unit)
        {
            battleUI.AddBattleMessage(unit.UnitName + " is defending!");
            battleUI.RefreshStatus(battle);
        }

        private void HandleActionFailed(BattleUnit unit, string reason)
        {
            battleUI.AddBattleMessage(unit.UnitName + ": " + reason);
            battleUI.RefreshStatus(battle);
        }

        private void HandleTurnStarted(BattleUnit unit)
        {
            battleUI.AddBattleMessage(unit.UnitName + "'s turn!");

            if (unit.Team == BattleTeam.Player && !hasShownCombatIntro)
            {
                hasShownCombatIntro = true;

                ShowBattleDialogue(
                    "Combat Tutorial",
                    "It's your turn! Choose Attack to damage an enemy. " +
                    "Choose Defend to reduce incoming damage. " +
                    "You can also use your known spells to perform special abilities. " +
                    "Attacking and defending both restore mana."
                );
            }
        }

        private void ShowBattleDialogue(string speaker, string dialogue)
        {
            if (dialogueManager == null)
            {
                Debug.LogWarning(
                    "Cannot show battle dialogue because DialogueManager is missing."
                );
                return;
            }

            battleDialogueActive = true;
            dialogueManager.ShowDialogue(speaker, dialogue);
        }

        private void HandleBattleDialogueEnded()
        {
            battleDialogueActive = false;
        }

        private void HandleBattleEnded(BattleResult result)
        {
            if (result == BattleResult.Victory)
                battleUI.AddBattleMessage("VICTORY!");
            else if (result == BattleResult.Defeat)
                battleUI.AddBattleMessage("DEFEAT!");

            StartCoroutine(FinishBattle());
        }

        private void Update()
        {
            if (battle == null ||
                waitingForTurn ||
                battleDialogueActive)
                return;

            if (battle.State == BattleState.EnemyTurn)
                StartCoroutine(PresentEnemyTurn());
        }

        private void HandleAttackTargetSelected(BattleUnit target)
        {
            if (battle == null ||
                battle.CurrentUnit == null ||
                waitingForTurn ||
                battleDialogueActive ||
                target == null ||
                !target.IsAlive ||
                battle.CurrentUnit.Team != BattleTeam.Player)
                return;

            StartCoroutine(PerformPlayerAttack(target));
        }

        private void HandleDefendConfirmed()
        {
            if (battle == null ||
                battle.CurrentUnit == null ||
                waitingForTurn ||
                battleDialogueActive ||
                battle.CurrentUnit.Team != BattleTeam.Player)
                return;

            battleUI.AddBattleMessage(
                battle.CurrentUnit.UnitName + " prepares to defend."
            );

            battle.ExecutePlayerAction(
                new DefendAction(battle.CurrentUnit)
            );

            battleUI.RefreshStatus(battle);
        }

        private void HandleItemSelected(ItemData item)
        {
            if (battle == null ||
                battle.CurrentUnit == null ||
                waitingForTurn ||
                battleDialogueActive ||
                item == null ||
                itemUseSystem == null)
                return;

            if (!battle.CurrentUnit.IsAlive ||
                battle.CurrentUnit.Team != BattleTeam.Player)
                return;

            StartCoroutine(PerformPlayerItem(item));
        }

        private IEnumerator PerformPlayerItem(ItemData item)
        {
            waitingForTurn = true;

            BattleUnit player = battle.CurrentUnit;

            yield return new WaitForSeconds(actionPause);

            battle.ExecutePlayerAction(
                new ItemAction(player, itemUseSystem, item)
            );

            battleUI.RefreshStatus(battle);

            yield return new WaitForSeconds(actionPause);

            waitingForTurn = false;
        }

        private IEnumerator PerformPlayerAttack(BattleUnit target)
        {
            waitingForTurn = true;

            BattleUnit player = battle.CurrentUnit;

            battleUI.AddBattleMessage(
                player.UnitName + " attacks " + target.UnitName + "!"
            );

            SetPlayerAttackSprite();
            PlaySound(playerAttackSound);

            yield return new WaitForSeconds(actionPause);

            battle.ExecutePlayerAction(
                new AttackAction(player, target)
            );

            battleUI.RefreshStatus(battle);

            yield return new WaitForSeconds(actionPause);

            RestorePlayerSprite();
            waitingForTurn = false;
        }

        private IEnumerator PerformPlayerSpell(
            Spell spell,
            BattleUnit target)
        {
            waitingForTurn = true;

            BattleUnit player = battle.CurrentUnit;

            if (playerAttackSprite != null)
                playerBattleSprite.sprite = playerAttackSprite;

            if (playerAttackSound != null)
                audioSource.PlayOneShot(playerAttackSound);

            yield return new WaitForSeconds(actionPause);

            battle.ExecutePlayerAction(
                new SpellAction(player, spell, target)
            );

            battleUI.RefreshStatus(battle);

            yield return new WaitForSeconds(actionPause);

            if (playerNormalSprite != null)
                playerBattleSprite.sprite = playerNormalSprite;

            waitingForTurn = false;
        }

        private IEnumerator PresentEnemyTurn()
        {
            waitingForTurn = true;

            BattleUnit enemy = battle.CurrentUnit;

            PlaySound(enemyAttackSound);

            yield return new WaitForSeconds(enemyTurnPause);

            battle.ExecuteEnemyTurn();
            battleUI.RefreshStatus(battle);

            yield return new WaitForSeconds(enemyTurnPause);

            waitingForTurn = false;
        }

        private void SetPlayerAttackSprite()
        {
            if (playerBattleSprite == null || playerAttackSprite == null)
                return;

            playerBattleSprite.sprite = playerAttackSprite;
        }

        private void RestorePlayerSprite()
        {
            if (playerBattleSprite == null)
                return;

            playerBattleSprite.sprite = playerNormalSprite;
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null || audioSource == null)
                return;

            audioSource.PlayOneShot(clip);
        }

        private void HandleSpellTargetSelected(
            Spell spell,
            BattleUnit target)
        {
            if (battle == null ||
                battle.CurrentUnit == null ||
                waitingForTurn ||
                battleDialogueActive ||
                spell == null)
                return;

            if (!battle.CurrentUnit.IsAlive ||
                battle.CurrentUnit.Team != BattleTeam.Player)
                return;

            if (spell.TargetType != SpellTargetType.AllEnemies)
            {
                if (target == null || !target.IsAlive)
                    return;
            }

            StartCoroutine(PerformPlayerSpell(spell, target));
        }

        private IEnumerator FinishBattle()
        {
            yield return new WaitForSeconds(battleEndPause);

            Scene battleScene =
                SceneManager.GetSceneByName("BattleScene");

            if (battleScene.IsValid() && battleScene.isLoaded)
            {
                SceneManager.SetActiveScene(
                    SceneManager.GetSceneByName(
                        EnemyEncounter.PreviousSceneName
                    )
                );

                if (BattleEncounterData.PlayerProgression != null)
                {
                    BattleEncounterData.PlayerProgression.UpdateVitals(
                        battle.Players[0].HP,
                        battle.Players[0].Mana
                    );
                }

                if (battle.Result == BattleResult.Victory)
                {
                    EnemyEncounter.MarkDefeated(
                        BattleEncounterData.EnemyEncounterId
                    );

                    Debug.Log(
                        "Enemy defeated and removed from exploration."
                    );
                }

                BattleEncounterData.Clear();

                yield return SceneManager.UnloadSceneAsync(battleScene);
            }
            else
            {
                Debug.LogError("Could not find the loaded BattleScene.");
            }
        }
    }
}
