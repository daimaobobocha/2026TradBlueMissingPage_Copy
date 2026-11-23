
using UnityEngine;

namespace Exploration
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProximityPlatform : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;

        [Header("Visibility")]
        [SerializeField] private float revealDistance = 5f;
        [SerializeField] private float fadeWidth = 1.5f;

        private SpriteRenderer spriteRenderer;
        private MaterialPropertyBlock propertyBlock;

        private static readonly int PlayerPosition =
            Shader.PropertyToID("_PlayerPosition");

        private static readonly int RevealDistance =
            Shader.PropertyToID("_RevealDistance");

        private static readonly int FadeWidth =
            Shader.PropertyToID("_FadeWidth");

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        private void LateUpdate()
        {
            if (player == null)
                return;

            spriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetVector(
                PlayerPosition,
                player.position
            );

            propertyBlock.SetFloat(
                RevealDistance,
                revealDistance
            );

            propertyBlock.SetFloat(
                FadeWidth,
                fadeWidth
            );

            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
