using UnityEngine;

namespace ss
{
    public sealed class CameraLimits : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraTransform;

        private const float leftLimit = 10.0f;
        private const float bottomLimit = 8.0f;

        private void Update()
        {
            var newPosition = cameraTransform.position;

            if (newPosition.x < leftLimit)
            {
                newPosition.x = leftLimit;
            }

            if (newPosition.y < bottomLimit)
            {
                newPosition.y = bottomLimit;
            }

            transform.position = newPosition;
        }
    }
}
