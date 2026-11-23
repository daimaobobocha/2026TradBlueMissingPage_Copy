
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Exploration
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text speakerName;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private GameObject continueIndicator;

        [Header("Choice UI")]
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private TMP_Text choice1Text;
        [SerializeField] private TMP_Text choice2Text;
        [SerializeField] private DialogueChoiceButton choice1Button;
        [SerializeField] private DialogueChoiceButton choice2Button;

        [Header("Dialogue Settings")]
        [SerializeField] private int maxCharacters = 200;
        [SerializeField] private char manualBreakSymbol = '|';

        private List<string> dialogueSections = new List<string>();
        private int currentSection;

        private string choice1Response;
        private string choice2Response;
        private string choice3Response;
        private DialogueChoiceButton choice3Button;
        private TMP_Text choice3Text;
        private bool hasThirdChoice;
        private Vector2 choice1Position;
        private Vector2 choice2Position;
        private Vector2 choice1Size;
        private Vector2 choice2Size;

        private bool waitingForChoice;
        private bool choicesPending;
        private bool waitForInteractRelease;

        public bool IsDialogueActive { get; private set; }

        public event Action OnDialogueEnded;

        private Action<int> choiceSelectedCallback;
        private Action<int> pendingChoiceCallback;
        private int pendingChoiceIndex;
        private bool pendingChoiceSelected;

        private void Awake()
        {
            var firstRect = (RectTransform)choice1Button.transform;
            var secondRect = (RectTransform)choice2Button.transform;
            choice1Position = firstRect.anchoredPosition;
            choice2Position = secondRect.anchoredPosition;
            choice1Size = firstRect.sizeDelta;
            choice2Size = secondRect.sizeDelta;
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(false);
        }


        private void Update()
        {
            if (!IsDialogueActive || waitingForChoice)
                return;

            bool interactPressed =
                (Keyboard.current != null &&
                 Keyboard.current.eKey.wasPressedThisFrame) ||
                (Mouse.current != null &&
                 Mouse.current.leftButton.wasPressedThisFrame);

            bool interactHeld =
                (Keyboard.current != null &&
                 Keyboard.current.eKey.isPressed) ||
                (Mouse.current != null &&
                 Mouse.current.leftButton.isPressed);

            if (waitForInteractRelease)
            {
                if (!interactHeld)
                    waitForInteractRelease = false;

                return;
            }

            if (interactPressed)
                AdvanceDialogue();
        }


        public void ShowDialogue(string speaker, string dialogue)
        {
            if (string.IsNullOrWhiteSpace(dialogue))
                return;

            speakerName.text = speaker;

            dialogueSections = SplitDialogue(dialogue);
            currentSection = 0;

            IsDialogueActive = true;
            waitingForChoice = false;
            choicesPending = false;
            pendingChoiceSelected = false;
            waitForInteractRelease = true;

            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);
            continueIndicator.SetActive(true);

            DisplayCurrentSection();
        }

        public void ShowChoiceDialogue(
            string speaker,
            string question,
            string choice1,
            string response1,
            string choice2,
            string response2,
            Action<int> onChoiceSelected = null)
        {
            SetThirdChoiceVisible(false);
            choiceSelectedCallback = onChoiceSelected;

            IsDialogueActive = true;
            waitingForChoice = false;
            choicesPending = true;
            pendingChoiceSelected = false;
            waitForInteractRelease = true;

            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);
            continueIndicator.SetActive(true);

            speakerName.text = speaker;

            choice1Text.text = choice1;
            choice2Text.text = choice2;

            choice1Response = response1;
            choice2Response = response2;

            dialogueSections = SplitDialogue(question);
            currentSection = 0;

            DisplayCurrentSection();
        }

        public void ShowThreeChoiceDialogue(
            string speaker,
            string question,
            string choice1,
            string response1,
            string choice2,
            string response2,
            string choice3,
            string response3,
            Action<int> onChoiceSelected = null)
        {
            ShowChoiceDialogue(speaker, question, choice1, response1,
                choice2, response2, onChoiceSelected);

            if (choice3Button == null)
            {
                // Reuse the existing button styling and keep the original two-choice UI.
                choice3Button = Instantiate(choice2Button, choice2Button.transform.parent);
                choice3Button.name = "Choice3";
                choice3Text = choice3Button.GetComponentInChildren<TMP_Text>(true);
                Button button = choice3Button.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(choice3Button.SelectChoice);
            }

            choice3Text.text = choice3;
            choice3Response = response3;
            SetThirdChoiceVisible(true);
        }

        private void SetThirdChoiceVisible(bool visible)
        {
            hasThirdChoice = visible;
            if (choice3Button != null)
                choice3Button.gameObject.SetActive(visible);

            var firstRect = (RectTransform)choice1Button.transform;
            var secondRect = (RectTransform)choice2Button.transform;
            firstRect.anchoredPosition = choice1Position;
            secondRect.anchoredPosition = choice2Position;
            firstRect.sizeDelta = choice1Size;
            secondRect.sizeDelta = choice2Size;

            if (!visible)
                return;

            // Fit three non-overlapping buttons into the existing choice region.
            float gap = Mathf.Abs(choice1Position.y - choice2Position.y) * 0.5f;
            float height = Mathf.Min(choice1Size.y, choice2Size.y, gap * 0.85f);
            firstRect.sizeDelta = new Vector2(choice1Size.x, height);
            secondRect.sizeDelta = new Vector2(choice2Size.x, height);
            secondRect.anchoredPosition = Vector2.Lerp(choice1Position, choice2Position, 0.5f);
            var thirdRect = (RectTransform)choice3Button.transform;
            thirdRect.anchoredPosition = choice2Position;
            thirdRect.sizeDelta = new Vector2(choice2Size.x, height);
        }

        private List<string> SplitDialogue(string dialogue)
        {
            List<string> sections = new List<string>();

            string[] manualSections = dialogue.Split(manualBreakSymbol);

            foreach (string manualSection in manualSections)
            {
                string remaining = manualSection.Trim();

                while (remaining.Length > maxCharacters)
                {
                    int splitIndex = -1;

                    for (int i = maxCharacters - 1; i >= 0; i--)
                    {
                        if (remaining[i] == '.' ||
                            remaining[i] == '!' ||
                            remaining[i] == '?')
                        {
                            splitIndex = i + 1;
                            break;
                        }
                    }

                    if (splitIndex == -1)
                    {
                        splitIndex = remaining.LastIndexOf(
                            ' ',
                            maxCharacters - 1
                        );
                    }

                    if (splitIndex <= 0)
                        splitIndex = maxCharacters;

                    sections.Add(
                        remaining.Substring(0, splitIndex).Trim()
                    );

                    remaining = remaining.Substring(splitIndex).Trim();
                }

                if (!string.IsNullOrEmpty(remaining))
                    sections.Add(remaining);
            }

            return sections;
        }

        private void DisplayCurrentSection()
        {
            if (currentSection >= 0 &&
                currentSection < dialogueSections.Count)
            {
                dialogueText.text = dialogueSections[currentSection];
            }
        }

        private void AdvanceDialogue()
        {
            if (currentSection < dialogueSections.Count - 1)
            {
                currentSection++;
                DisplayCurrentSection();
                return;
            }

            if (choicesPending)
            {
                ShowChoices();
                return;
            }

            if (pendingChoiceSelected)
            {
                Action<int> callback = pendingChoiceCallback;
                int selectedChoice = pendingChoiceIndex;

                pendingChoiceCallback = null;
                pendingChoiceSelected = false;

                EndDialogue();
                callback?.Invoke(selectedChoice);
                return;
            }

            EndDialogue();
        }

        private void ShowChoices()
        {
            choicesPending = false;
            waitingForChoice = true;

            choicePanel.SetActive(true);
            continueIndicator.SetActive(false);

            if (hasThirdChoice)
                FitThreeChoiceButtons();

            choice1Button.Setup(this, 0);
            choice2Button.Setup(this, 1);
            if (hasThirdChoice)
                choice3Button.Setup(this, 2);
        }

        private void FitThreeChoiceButtons()
        {
            Canvas.ForceUpdateCanvases();
            var canvasRect = (RectTransform)choicePanel.GetComponentInParent<Canvas>().rootCanvas.transform;
            var buttons = new[] { choice1Button, choice2Button, choice3Button };
            var corners = new Vector3[4];
            float rightEdge = float.NegativeInfinity;
            foreach (var choice in buttons)
            {
                ((RectTransform)choice.transform).GetWorldCorners(corners);
                rightEdge = Mathf.Max(rightEdge, canvasRect.InverseTransformPoint(corners[2]).x);
            }

            float shift = Mathf.Min(0f, canvasRect.rect.xMax - 24f - rightEdge);
            Vector3 offset = canvasRect.TransformVector(new Vector3(shift, 0f, 0f));
            foreach (var choice in buttons)
                choice.transform.position += offset;
        }

        public void SelectChoice(int index)
        {
            if (!waitingForChoice || index < 0 || index > (hasThirdChoice ? 2 : 1))
                return;

            pendingChoiceCallback = choiceSelectedCallback;
            pendingChoiceIndex = index;
            pendingChoiceSelected = true;
            choiceSelectedCallback = null;

            waitingForChoice = false;
            choicesPending = false;

            choicePanel.SetActive(false);
            continueIndicator.SetActive(true);

            string response = index == 0
                ? choice1Response
                : index == 1 ? choice2Response : choice3Response;

            dialogueSections = SplitDialogue(response);
            currentSection = 0;

            waitForInteractRelease = true;

            DisplayCurrentSection();
        }

        public void EndDialogue()
        {
            IsDialogueActive = false;
            waitingForChoice = false;
            choicesPending = false;

            dialoguePanel.SetActive(false);
            choicePanel.SetActive(false);

            OnDialogueEnded?.Invoke();
        }
    }
}
