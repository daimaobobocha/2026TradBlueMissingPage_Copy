using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleSystem
{
    public class BattleInput : MonoBehaviour
    {
        public BattleInputActions Actions { get; private set; }

        public bool AttackPressed =>
            Actions.Battle.Attack.WasPressedThisFrame();

        public bool DefendPressed =>
            Actions.Battle.Defend.WasPressedThisFrame();

        public bool Spell1Pressed =>
            Actions.Battle.Spell1.WasPressedThisFrame();

        public bool Spell2Pressed =>
            Actions.Battle.Spell2.WasPressedThisFrame();

        private void Awake()
        {
            Actions = new BattleInputActions();
        }

        private void OnEnable()
        {
            Actions.Enable();
        }

        private void OnDisable()
        {
            Actions.Disable();
        }
    }
}