
using UnityEngine;

namespace Exploration
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("You interacted with this object!");
        }
    }
}
