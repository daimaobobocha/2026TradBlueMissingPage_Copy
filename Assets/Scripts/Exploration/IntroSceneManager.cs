using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

namespace Exploration
{
    public class IntroSceneManager : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "Main3DScene";
        [SerializeField] private TMP_Text storyText;
        [SerializeField] private TMP_Text continueText;

        [SerializeField] private string StoryText;
        [SerializeField] private string ContinueText;


        private void Start()
        {
            storyText.text = StoryText;

            continueText.text = ContinueText;
        }

        private void Update()
        {
            if (Keyboard.current != null &&
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Library 1");
            }
        }
    }
}
