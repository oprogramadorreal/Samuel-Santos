using UnityEngine;

namespace ss
{
    public sealed class WeaponToLimbHit : MonoBehaviour
    {
        [SerializeField]
        private Weapon sourceWeapon;

        [SerializeField]
        private float strength = 1.0f;

        [SerializeField]
        private Rigidbody2D rb;

        public float GetStrenth()
        {
            if (sourceWeapon != null && !sourceWeapon.IsEquipped())
            {
                return 0.0f;
            }

            if (rb == null)
            {
                return strength;
            }

            return strength * rb.velocity.magnitude;
        }
    }
}
