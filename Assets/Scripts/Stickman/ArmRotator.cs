using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/uVtDyPmorA0
    /// </summary>
    public sealed class ArmRotator : MonoBehaviour, IDisableableLimbComponent
    {
        [SerializeField]
        private int speed = 300;

        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void RotateToDirection(Vector2 direction, float strength)
        {
            if (enabled)
            {
                var rotationZ = CalculateRotation(direction);
                rb.MoveRotation(Mathf.LerpAngle(rb.rotation, rotationZ, speed * strength * Time.deltaTime));
            }
        }

        private float CalculateRotation(Vector2 joystickDir)
        {
            return Mathf.Atan2(-joystickDir.x, joystickDir.y) * Mathf.Rad2Deg;
        }

        void IDisableableLimbComponent.DisableLimbComponent()
        {
            enabled = false;
        }
    }
}
