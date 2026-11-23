
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Exploration
{
    [ExecuteAlways]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformColliderSync : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private BoxCollider2D platformCollider;

        private void OnEnable()
        {
            UpdateCollider();
        }

        private void OnValidate()
        {
            UpdateCollider();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCollider();
#endif
        }

        private void UpdateCollider()
        {
            if (platformCollider == null)
                platformCollider = GetComponent<BoxCollider2D>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (platformCollider == null || spriteRenderer == null)
                return;

            Bounds bounds = spriteRenderer.bounds;

            platformCollider.size = transform.InverseTransformVector(
                bounds.size
            );

            platformCollider.offset = transform.InverseTransformPoint(
                bounds.center
            );
        }
    }
}
