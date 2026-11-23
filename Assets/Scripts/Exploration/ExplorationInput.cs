
using UnityEngine;
using UnityEngine.InputSystem;

namespace Exploration
{
    public class ExplorationInput : MonoBehaviour
    {
        public BattleInputActions Actions { get; private set; }
        private InputAction sprintAction;

        public Vector2 Move =>
            Actions.Exploration.Move.ReadValue<Vector2>();

        public bool JumpPressed =>
            Actions.Exploration.Jump.WasPressedThisFrame();

        public bool InteractPressed => Actions.Exploration.Interact.WasPressedThisFrame();
        public bool SprintHeld => Actions.Exploration.enabled && sprintAction.IsPressed();

        private void Awake()
        {
            Actions = new BattleInputActions();
            sprintAction = new InputAction("Sprint", InputActionType.Button);
            sprintAction.AddBinding("<Keyboard>/leftShift");
            sprintAction.AddBinding("<Keyboard>/rightShift");
        }

        private void OnEnable()
        {
            Actions.Enable();
            sprintAction.Enable();
        }

        private void OnDisable()
        {
            Actions.Disable();
            sprintAction.Disable();
        }

        private void OnDestroy()
        {
            Actions.Dispose();
            sprintAction.Dispose();
        }
    }
}
