using UnityEngine;

namespace ss
{
    public sealed class Grabbable : MonoBehaviour
    {
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Grab(Joint2D joint)
        {
            if (rb != null)
            {
                joint.connectedBody = rb;
            }
        }
    }
}
