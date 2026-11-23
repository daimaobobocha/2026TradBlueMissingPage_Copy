
using UnityEngine;

namespace Exploration
{
    public class CameraArea2D : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D cameraBounds;

        public Bounds Bounds
        {
            get
            {
                if (cameraBounds == null)
                {
                    return new Bounds(transform.position, Vector3.zero);
                }

                return cameraBounds.bounds;
            }
        }

        private void Awake()
        {
            if (cameraBounds == null)
            {
                cameraBounds = GetComponent<BoxCollider2D>();
            }

            if (cameraBounds == null)
            {
                Debug.LogWarning("Camera Area is missing its BoxCollider2D!");
            }
            else
            {
                cameraBounds.isTrigger = true;
            }
        }
    }
}
