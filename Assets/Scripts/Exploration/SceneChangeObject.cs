using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exploration
{
    public class SceneChangeObject : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void Interact()
        {
            if (string.IsNullOrEmpty(sceneName))
                return;

            SceneManager.LoadScene(sceneName);
        }
    }
}
