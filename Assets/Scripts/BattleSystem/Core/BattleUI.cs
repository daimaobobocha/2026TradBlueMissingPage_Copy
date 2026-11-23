
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Items;

namespace BattleSystem
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Book Pages")]
        [SerializeField] private GameObject mainPage;
        [SerializeField] private GameObject attackPage;
        [SerializeField] private GameObject defendPage;
        [SerializeField] private GameObject magicPage;
        [SerializeField] private GameObject itemPage;

        [Header("Main Buttons")]
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button magicButton;
        [SerializeField] private Button itemButton;

        [Header("Back Buttons")]
        [SerializeField] private Button attackBackButton;
        [SerializeField] private Button defendBackButton;
        [SerializeField] private Button magicBackButton;
        [SerializeField] private Button itemBackButton;

        [Header("Target Selection")]
        [SerializeField] private GameObject enemyTargets;
        [SerializeField] private Button enemyTargetButtonTemplate;
        [SerializeField] private Button playerTargetButton;
        [SerializeField] private TMP_Text targetPrompt;

        [Header("Permanent Enemy Status")]
        [SerializeField] private Transform enemyStatusContainer;
        [SerializeField] private GameObject enemyStatusTemplate;

        [Header("Permanent Player Status")]
        [SerializeField] private TMP_Text playerStatusText;
        [SerializeField] private Image playerHealthBarFill;
        [SerializeField] private Image playerManaBarFill;

        [Header("Battle Log")]
        [SerializeField] private TMP_Text actionLogText;
        [SerializeField] private int maxLogMessages = 6;

        [Header("Magic Menu")]
        [SerializeField] private Transform spellListContainer;
        [SerializeField] private Button spellButtonTemplate;

        [Header("Item Menu")]
        [SerializeField] private Transform itemListContainer;
        [SerializeField] private Button itemButtonTemplate;

        private readonly List<GameObject> generatedSpellButtons =
            new List<GameObject>();

        private readonly List<GameObject> generatedItemButtons =
            new List<GameObject>();

        private Spell selectedSpell;
        private PlayerInventory inventory;

        private BattleController battle;

        private readonly List<GameObject> generatedTargetButtons =
            new List<GameObject>();

        private readonly Dictionary<BattleUnit, EnemyStatusView> enemyStatusViews =
            new Dictionary<BattleUnit, EnemyStatusView>();

        private readonly List<string> battleMessages = new List<string>();

        public event Action<BattleUnit> AttackTargetSelected;
        public event Action DefendConfirmed;
        public event Action<Spell, BattleUnit> SpellTargetSelected;
        public event Action<ItemData> ItemSelected;

        private class EnemyStatusView
        {
            public TMP_Text NameText;
            public TMP_Text HPText;
            public Image HealthFill;
        }

        public void Initialize(BattleController controller)
        {
            battle = controller;

            attackButton.onClick.RemoveAllListeners();
            defendButton.onClick.RemoveAllListeners();
            magicButton.onClick.RemoveAllListeners();
            itemButton.onClick.RemoveAllListeners();

            attackButton.onClick.AddListener(OpenAttackPage);
            defendButton.onClick.AddListener(OpenDefendPage);
            magicButton.onClick.AddListener(OpenMagicPage);
            itemButton.onClick.AddListener(OpenItemPage);

            attackBackButton.onClick.RemoveAllListeners();
            defendBackButton.onClick.RemoveAllListeners();
            magicBackButton.onClick.RemoveAllListeners();
            itemBackButton.onClick.RemoveAllListeners();

            attackBackButton.onClick.AddListener(ReturnToMainPage);
            defendBackButton.onClick.AddListener(ReturnToMainPage);
            magicBackButton.onClick.AddListener(ReturnToMainPage);
            itemBackButton.onClick.AddListener(ReturnToMainPage);

            playerTargetButton.onClick.RemoveAllListeners();
            playerTargetButton.onClick.AddListener(HandlePlayerTargetSelected);

            enemyTargetButtonTemplate.gameObject.SetActive(false);
            spellButtonTemplate.gameObject.SetActive(false);
            itemButtonTemplate.gameObject.SetActive(false);
            enemyStatusTemplate.SetActive(false);
            enemyTargets.SetActive(false);
            playerTargetButton.gameObject.SetActive(false);

            BuildPermanentEnemyStatus();
            RefreshStatus(controller);
            ShowPage(mainPage);

            battleMessages.Clear();
            UpdateBattleLog();
        }

        private void OpenAttackPage()
        {
            selectedSpell = null;

            if (!CanChooseAction())
                return;

            ShowPage(attackPage);
            targetPrompt.text = "Select an enemy to attack.";

            playerTargetButton.gameObject.SetActive(false);

            BuildEnemyTargets();
            enemyTargets.SetActive(true);
            enemyTargets.transform.SetAsLastSibling();
        }

        private void OpenDefendPage()
        {
            selectedSpell = null;

            if (!CanChooseAction())
                return;

            ShowPage(defendPage);
            targetPrompt.text = "Select yourself to defend.";

            enemyTargets.SetActive(false);

            playerTargetButton.gameObject.SetActive(true);
            playerTargetButton.interactable = true;

            playerTargetButton.transform.SetAsLastSibling();
        }

        private void OpenMagicPage()
        {
            if (!CanChooseAction())
                return;

            ShowPage(magicPage);
            HideTargets();
        }

        private void OpenItemPage()
        {
            selectedSpell = null;

            if (!CanChooseAction())
                return;

            ShowPage(itemPage);
            HideTargets();

            BuildItemButtons();
        }

        public void SetAvailableItems(PlayerInventory playerInventory)
        {
            inventory = playerInventory;
            BuildItemButtons();
        }

        private void BuildItemButtons()
        {
            ClearGeneratedItemButtons();

            if (inventory == null ||
                itemListContainer == null ||
                itemButtonTemplate == null)
                return;

            foreach (ItemStack stack in inventory.Items)
            {
                if (stack == null || stack.Item == null ||
                    stack.Quantity <= 0)
                    continue;

                ItemData selectedItem = stack.Item;

                Button button = Instantiate(
                    itemButtonTemplate,
                    itemListContainer
                );

                button.gameObject.SetActive(true);

                TMP_Text buttonText =
                    button.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                    buttonText.text =
                        selectedItem.ItemName + " x" + stack.Quantity;
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(
                    () => SelectItem(selectedItem)
                );

                generatedItemButtons.Add(button.gameObject);
            }
        }

        private void ClearGeneratedItemButtons()
        {
            foreach (GameObject button in generatedItemButtons)
                Destroy(button);

            generatedItemButtons.Clear();
        }

        private void SelectItem(ItemData item)
        {
            if (!CanChooseAction() || item == null)
                return;

            HideTargets();
            ShowPage(mainPage);

            ItemSelected?.Invoke(item);
        }

        private void BuildPermanentEnemyStatus()
        {
            ClearPermanentEnemyStatus();

            if (battle == null || enemyStatusContainer == null ||
                enemyStatusTemplate == null)
                return;

            foreach (BattleUnit enemy in battle.Enemies)
            {
                GameObject row = Instantiate(
                    enemyStatusTemplate,
                    enemyStatusContainer
                );

                row.name = enemy.UnitName + " Status";
                row.SetActive(true);

                EnemyStatusView view = new EnemyStatusView
                {
                    NameText = FindText(row.transform, "EnemyStatusNameText"),
                    HPText = FindText(row.transform, "EnemyStatusHPText"),
                    HealthFill = FindImage(row.transform, "EnemyStatusHealthFill")
                };

                enemyStatusViews[enemy] = view;
            }
        }

        private void ClearPermanentEnemyStatus()
        {
            if (enemyStatusContainer != null)
            {
                foreach (Transform child in enemyStatusContainer)
                {
                    if (child.gameObject != enemyStatusTemplate)
                        Destroy(child.gameObject);
                }
            }

            enemyStatusViews.Clear();
        }

        private void BuildEnemyTargets()
        {
            ClearGeneratedTargets();

            foreach (BattleUnit enemy in battle.GetAliveEnemies())
            {
                BattleUnit selectedEnemy = enemy;

                Button targetButton = Instantiate(
                    enemyTargetButtonTemplate,
                    enemyTargets.transform
                );

                targetButton.gameObject.SetActive(true);
                targetButton.interactable = true;

                TMP_Text buttonText =
                    targetButton.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                    buttonText.text = selectedEnemy.UnitName;

                targetButton.onClick.RemoveAllListeners();
                targetButton.onClick.AddListener(
                    () => SelectEnemy(selectedEnemy)
                );

                generatedTargetButtons.Add(targetButton.gameObject);
            }
        }

        private void SelectEnemy(BattleUnit enemy)
        {
            if (selectedSpell != null)
            {
                Spell spell = selectedSpell;
                selectedSpell = null;

                HideTargets();
                ShowPage(mainPage);

                SpellTargetSelected?.Invoke(spell, enemy);
                return;
            }

            HideTargets();
            ShowPage(mainPage);
            AttackTargetSelected?.Invoke(enemy);
        }

        private void ConfirmDefend()
        {
            if (!CanChooseAction())
                return;

            HideTargets();
            ShowPage(mainPage);
            DefendConfirmed?.Invoke();
        }

        private void ReturnToMainPage()
        {
            selectedSpell = null;
            HideTargets();
            ShowPage(mainPage);
        }

        private void ShowPage(GameObject page)
        {
            mainPage.SetActive(page == mainPage);
            attackPage.SetActive(page == attackPage);
            defendPage.SetActive(page == defendPage);
            magicPage.SetActive(page == magicPage);
            itemPage.SetActive(page == itemPage);
        }

        private void HideTargets()
        {
            enemyTargets.SetActive(false);
            playerTargetButton.gameObject.SetActive(false);
        }

        private void ClearGeneratedTargets()
        {
            foreach (GameObject target in generatedTargetButtons)
                Destroy(target);

            generatedTargetButtons.Clear();
        }

        private bool CanChooseAction()
        {
            return battle != null &&
                   battle.State == BattleState.PlayerTurn &&
                   battle.CurrentUnit != null &&
                   battle.CurrentUnit.Team == BattleTeam.Player;
        }

        public void RefreshStatus(BattleController controller)
        {
            if (controller == null)
                return;

            if (controller.Players.Count > 0)
            {
                BattleUnit player = controller.Players[0];

                if (playerStatusText != null)
                {
                    playerStatusText.text =
                        player.UnitName + "\n" +
                        "HP: " + player.HP + " / " + player.MaxHP + "\n" +
                        "MP: " + player.Mana + " / " + player.MaxMana;
                }

                SetBar(playerHealthBarFill, player.HP, player.MaxHP);
                SetBar(playerManaBarFill, player.Mana, player.MaxMana);
            }

            foreach (KeyValuePair<BattleUnit, EnemyStatusView> entry in enemyStatusViews)
            {
                BattleUnit enemy = entry.Key;
                EnemyStatusView view = entry.Value;

                if (view.NameText != null)
                    view.NameText.text = enemy.UnitName;

                if (view.HPText != null)
                {
                    view.HPText.text =
                        enemy.HP + " / " + enemy.MaxHP + " HP";
                }

                SetBar(view.HealthFill, enemy.HP, enemy.MaxHP);
            }
        }

        private void SetBar(Image bar, int current, int maximum)
        {
            if (bar == null)
                return;

            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
            bar.fillAmount = maximum > 0
                ? Mathf.Clamp01((float)current / maximum)
                : 0f;
        }

        private TMP_Text FindText(Transform parent, string objectName)
        {
            Transform child = parent.Find(objectName);
            return child != null ? child.GetComponent<TMP_Text>() : null;
        }

        private Image FindImage(Transform parent, string objectName)
        {
            Transform child = parent.Find(objectName);
            return child != null ? child.GetComponent<Image>() : null;
        }

        public void AddBattleMessage(string newMessage)
        {
            if (string.IsNullOrWhiteSpace(newMessage))
                return;

            battleMessages.Add(newMessage);

            while (battleMessages.Count > Mathf.Max(1, maxLogMessages))
                battleMessages.RemoveAt(0);

            UpdateBattleLog();
        }

        private void UpdateBattleLog()
        {
            if (actionLogText != null)
                actionLogText.text = string.Join("\n", battleMessages);
        }

        public void SetAvailableSpells(List<Spell> spells)
        {
            foreach (GameObject button in generatedSpellButtons)
                Destroy(button);

            generatedSpellButtons.Clear();

            if (spellListContainer == null || spellButtonTemplate == null)
            {
                Debug.LogError("Magic menu references are not assigned.");
                return;
            }

            foreach (Spell spell in spells)
            {
                if (spell == null)
                    continue;

                Spell selected = spell;

                Button button = Instantiate(
                    spellButtonTemplate,
                    spellListContainer
                );

                button.gameObject.SetActive(true);

                TMP_Text buttonText =
                    button.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                    buttonText.text =
                        selected.SpellName + " - " +
                        selected.ManaCost + " MP";
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSpell(selected));

                generatedSpellButtons.Add(button.gameObject);
            }
        }

        private void SelectSpell(Spell spell)
        {
            if (!CanChooseAction())
                return;

            selectedSpell = spell;

            if (spell.TargetType == SpellTargetType.AllEnemies)
            {
                selectedSpell = null;

                HideTargets();
                ShowPage(mainPage);

                SpellTargetSelected?.Invoke(spell, null);
                return;
            }

            if (spell.TargetType == SpellTargetType.SingleEnemy)
            {
                targetPrompt.text = "Choose an enemy to target";

                playerTargetButton.gameObject.SetActive(false);
                BuildEnemyTargets();
                enemyTargets.SetActive(true);
                enemyTargets.transform.SetAsLastSibling();
            }
            else if (spell.TargetType == SpellTargetType.Self)
            {
                targetPrompt.text = "Choose yourself";

                enemyTargets.SetActive(false);
                playerTargetButton.gameObject.SetActive(true);
                playerTargetButton.interactable = true;
                playerTargetButton.transform.SetAsLastSibling();
            }
        }

        private void HandlePlayerTargetSelected()
        {
            if (!CanChooseAction())
                return;

            if (selectedSpell != null)
            {
                Spell spell = selectedSpell;
                BattleUnit target = battle.CurrentUnit;

                selectedSpell = null;

                HideTargets();
                ShowPage(mainPage);

                SpellTargetSelected?.Invoke(spell, target);
                return;
            }

            ConfirmDefend();
        }

        public void RefreshEnemyDisplays(BattleController controller)
        {
            if (controller == null)
                return;

            battle = controller;

            BuildPermanentEnemyStatus();
            ClearGeneratedTargets();

            RefreshStatus(controller);
        }
    }
}
