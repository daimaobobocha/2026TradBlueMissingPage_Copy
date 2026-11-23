
using UnityEngine;

namespace Exploration
{
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;
        [SerializeField] private CameraArea2D currentArea;

        [Header("Camera Settings")]
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private float teleportThreshold = 5f;

        private float velocityX;
        private float velocityY;
        private Camera cam;
        private bool snapNextFrame;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        public void SetCameraArea(CameraArea2D newArea)
        {
            if (newArea == null)
            {
                Debug.LogWarning("Cannot switch camera to a missing CameraArea2D!");
                return;
            }

            currentArea = newArea;
            velocityX = 0f;
            velocityY = 0f;
            snapNextFrame = true;
        }

        private void LateUpdate()
        {
            if (target == null || cam == null)
            {
                return;
            }

            Vector3 targetPosition = target.position;

            if (currentArea != null)
            {
                Bounds bounds = currentArea.Bounds;

                float cameraHalfWidth = cam.orthographicSize * cam.aspect;
                float cameraHalfHeight = cam.orthographicSize;

                float minX = bounds.min.x + cameraHalfWidth;
                float maxX = bounds.max.x - cameraHalfWidth;
                float minY = bounds.min.y + cameraHalfHeight;
                float maxY = bounds.max.y - cameraHalfHeight;

                if (minX > maxX)
                {
                    minX = maxX = bounds.center.x;
                }

                if (minY > maxY)
                {
                    minY = maxY = bounds.center.y;
                }

                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            }

            if (snapNextFrame ||
                Vector2.Distance(transform.position, target.position) > teleportThreshold)
            {
                velocityX = 0f;
                velocityY = 0f;
                snapNextFrame = false;

                transform.position = new Vector3(
                    targetPosition.x,
                    targetPosition.y,
                    transform.position.z
                );

                return;
            }

            float newX = Mathf.SmoothDamp(
                transform.position.x,
                targetPosition.x,
                ref velocityX,
                smoothTime
            );

            float newY = Mathf.SmoothDamp(
                transform.position.y,
                targetPosition.y,
                ref velocityY,
                smoothTime
            );

            transform.position = new Vector3(
                newX,
                newY,
                transform.position.z
            );
        }
    }
}
