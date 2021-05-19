using UnityEngine;

namespace ss
{
    public sealed class LevelPart : MonoBehaviour
    {
        [SerializeField]
        private float xOffset = 0.0f;

        public float GetXOffset()
        {
            return xOffset;
        }
    }
}
