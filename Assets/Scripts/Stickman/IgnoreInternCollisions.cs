using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/-F6OMqo52qY
    /// </summary>
    public sealed class IgnoreInternCollisions : MonoBehaviour
    {
        private void Start()
        {
            var colliders = GetComponentsInChildren<Collider2D>();

            for (var i = 0; i < colliders.Length; ++i)
            {
                for (var k = i + 1; k < colliders.Length; ++k)
                {
                    Physics2D.IgnoreCollision(colliders[i], colliders[k]);
                }
            }
        }
    }
}
