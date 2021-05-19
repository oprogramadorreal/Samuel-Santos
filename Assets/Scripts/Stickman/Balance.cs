using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/uVtDyPmorA0
    /// </summary>
    public sealed class Balance : MonoBehaviour, IDisableableLimbComponent
    {
        [SerializeField]
        private float restRotation = 0.0f;

        [SerializeField]
        private float force = 0.0f;

        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (force != 0.0f)
            {
                rb.MoveRotation(Mathf.LerpAngle(rb.rotation, restRotation, force * Time.deltaTime));
            }
        }

        void IDisableableLimbComponent.DisableLimbComponent()
        {
            enabled = false;
        }
    }
}
