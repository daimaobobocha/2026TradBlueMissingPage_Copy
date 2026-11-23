
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Exploration
{
    public class FirstPersonInteraction : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float interactDistance = 5f;

        [Header("Hover Prompt")]
        [SerializeField] private GameObject hoverPrompt;

        private SceneChangeObject hoveredObject;

        private void Awake()
        {
            if (playerCamera == null)
                playerCamera = GetComponentInChildren<Camera>();

            if (hoverPrompt != null)
                hoverPrompt.SetActive(false);
        }

        private void Update()
        {
            UpdateHover();

            if (Mouse.current == null ||
                !Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (hoveredObject != null)
                hoveredObject.Interact();
        }

        private void UpdateHover()
        {
            SceneChangeObject newHoveredObject = null;

            Ray ray = playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                newHoveredObject =
                    hit.collider.GetComponent<SceneChangeObject>();
            }

            hoveredObject = newHoveredObject;

            if (hoverPrompt != null)
                hoverPrompt.SetActive(hoveredObject != null);
        }
    }
}
