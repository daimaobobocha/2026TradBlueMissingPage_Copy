
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exploration
{
    public class DialogueChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject selectionArrow;
        [SerializeField] private Button button;

        private DialogueManager dialogueManager;
        private int choiceIndex;

        private void Awake()
        {
            selectionArrow.SetActive(false);
        }

        public void Setup(DialogueManager manager, int index)
        {
            selectionArrow.SetActive(false);
            dialogueManager = manager;
            choiceIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button.interactable)
            {
                selectionArrow.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            selectionArrow.SetActive(false);
        }

        public void SelectChoice()
        {
            if (button.interactable && dialogueManager != null)
            {
                dialogueManager.SelectChoice(choiceIndex);
            }
        }
    }
}
