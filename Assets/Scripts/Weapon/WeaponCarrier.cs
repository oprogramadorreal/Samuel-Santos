using System;
using UnityEngine;
using UnityEngine.Events;

namespace ss
{
    public sealed class WeaponCarrier : MonoBehaviour
    {
        [SerializeField]
        private GameObject hand;

        [SerializeField]
        private ArmRotator[] weaponArms;

        [SerializeField]
        private UnityEvent weaponEquippedEvent;

        [SerializeField]
        private UnityEvent weaponUnequippedEvent;

        private Joint2D joint;
        private Collider2D[] colliders;

        private Weapon currentWeapon;
        private Vector3 currentWeaponToHandleOffset;

        private Action lateUpdateAction;

        private int playerLayer;
        private int weaponLayer;

        public bool EquippingDisabled { get; set; } = false;

        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();

            playerLayer = LayerMask.NameToLayer("Player");
            weaponLayer = LayerMask.NameToLayer("Weapon");
        }

        public bool IsEquipped()
        {
            return currentWeapon != null
                && joint != null;
        }

        public void Unequip(float reequippingCooldown = 2.0f)
        {
            if (!IsEquipped())
            {
                return;
            }
           
            Destroy(joint);
            joint = null;

            currentWeapon.Unequip();
            weaponUnequippedEvent?.Invoke();

            if (reequippingCooldown == 0.0f)
            {
                AllowEquippingAgain();
            }
            else
            {
                Invoke(nameof(AllowEquippingAgain), reequippingCooldown);
            }
        }

        private void AllowEquippingAgain()
        {
            SetIgnoreCollisions(currentWeapon, false);
            currentWeapon = null;
        }

        public bool Equip(Weapon weapon, Vector3 weaponToHandleOffset)
        {
            if (EquippingDisabled)
            {
                return false;
            }

            if (currentWeapon != null)
            {
                return false;
            }

            SetIgnoreCollisions(weapon, true);

            // Sets the desired position and rotation for the weapon
            // which will be kept by the FixedJoint2D (added at LateUpdate).
            weapon.transform.position = hand.transform.position;
            weapon.transform.rotation = hand.transform.rotation;

            // If this weapon was previously equipped by an enemy then Y scale is negative. We need to fix it here.
            weapon.transform.localScale = new Vector3(weapon.transform.localScale.x, Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);

            currentWeapon = weapon;
            currentWeaponToHandleOffset = weaponToHandleOffset;

            lateUpdateAction = () =>
            {
                // Finishes equipping. Needed to be done at LateUpdate to solve a bug with Joint2D.
                // Creates the joint to grab the current weapon.
                joint = CreateHandToWeaponJoint();
                weaponEquippedEvent?.Invoke();
            };

            return true;
        }

        private void LateUpdate()
        {
            if (lateUpdateAction != null)
            {
                lateUpdateAction.Invoke();
                lateUpdateAction = null;
            }
        }

        private Joint2D CreateHandToWeaponJoint()
        {
            var newJoint = hand.AddComponent<FixedJoint2D>();
            newJoint.connectedBody = currentWeapon.GetComponent<Rigidbody2D>();
            newJoint.autoConfigureConnectedAnchor = false;
            newJoint.connectedAnchor = new Vector2(currentWeaponToHandleOffset.x, currentWeaponToHandleOffset.y);
            return newJoint;
        }

        private void SetIgnoreCollisions(Weapon weapon, bool ignore)
        {
            if (gameObject.layer == playerLayer)
            {
                // ignore collisions between player and unequipped weapons
                Physics2D.IgnoreLayerCollision(playerLayer, weaponLayer, ignore);
            }

            // ignore collisions between this carrier and the just equipped weapon
            for (var i = 0; i < colliders.Length; ++i)
            {
                var ci = colliders[i];

                if (ci != null)
                {
                    for (var k = 0; k < weapon.Colliders.Length; ++k)
                    {
                        Physics2D.IgnoreCollision(ci, weapon.Colliders[k], ignore);
                    }
                }
            }
        }

        public void UpdateWeaponArms()
        {
            if (IsEquipped())
            {
                currentWeapon.UpdateWeaponArms(weaponArms);
            }
        }

        public bool CurrentWeaponHasTag(string tag)
        {
            if (!IsEquipped())
            {
                return false;
            }

            return currentWeapon.HasTag(tag);
        }

        public bool FireWeapon()
        {
            if (!IsEquipped())
            {
                return false;
            }

            return currentWeapon.Fire();
        }
    }
}
