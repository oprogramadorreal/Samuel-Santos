using System;
using UnityEngine;

namespace ss
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private MovementController movement;

        [SerializeField]
        private WeaponCarrier weaponCarrier;

        private SlowMotionManager slowMotion;

        private CinemachineShake cameraShake;

        [SerializeField]
        private Grab[] grabs;

        private GameObject[] weaponUI;

        private bool canTriggerMatrixModeEvent = true;
        private int gunBulletLayerMask;

        public Vector2 BodyPosition { get => movement.BodyPosition; }

        private void Start()
        {
            weaponUI = GameObject.FindGameObjectsWithTag("WeaponUI");
            gunBulletLayerMask = 1 << LayerMask.NameToLayer("GunBullet");

            var gameManager = GameManager.Instance;

            if (gameManager != null)
            {
                slowMotion = gameManager.SlowMotion;
                cameraShake = gameManager.CameraShake;
            }
        }

        private void Update()
        {
            var leftJoystickDir = new Vector2(JoystickLeft.positionX, JoystickLeft.positionY);
            movement.SetInputAxisForBodyMovement(leftJoystickDir);

            if (JoystickRight.pressed)
            {
                var rightJoystickDir = new Vector2(JoystickRight.positionX, JoystickRight.positionY);
                movement.SetInputAxisForArmsMovement(rightJoystickDir);
            }

            foreach (var g in grabs)
            {
                g.EnableGrabbing(JoystickRight.pressed && (weaponCarrier == null || !weaponCarrier.IsEquipped()));
            }

            if (weaponCarrier != null)
            {
                weaponCarrier.UpdateWeaponArms();
            }

            TryToActivateMatrixMode();
        }

        private void TryToActivateMatrixMode()
        {
            const float bulletsDetectorRadius = 35.0f;
            var nearbyBullets = Physics2D.OverlapCircleAll(BodyPosition, bulletsDetectorRadius, gunBulletLayerMask);

            if (CheckMatrixModeRequirement(nearbyBullets))
            {
                OnMatrixModeEvent();
            }
        }

        private bool CheckMatrixModeRequirement(Collider2D[] nearbyBullets)
        {
            const int maxNearbyBulletsThreshold = 5;

            return nearbyBullets.Length > maxNearbyBulletsThreshold
                || (movement.CheckMatrixPose() && HasIncomingBullet(nearbyBullets));
        }

        public void FireWeapon()
        {
            if (weaponCarrier != null)
            {
                if (weaponCarrier.FireWeapon() && !slowMotion.IsDoingSlowMotion())
                {
                    cameraShake.ShakeCamera(5.0f, 0.2f);
                }
            }
        }

        public void UnequipWeaponAndDisableEquip()
        {
            UnequipWeapon();

            if (weaponCarrier != null)
            {
                weaponCarrier.EquippingDisabled = true;
            }
        }

        public void UnequipWeapon()
        {
            if (weaponCarrier != null)
            {
                weaponCarrier.Unequip();
                AudioManager.Instance.CreateTemporaryAudioSourceAt("WeaponUnequip", BodyPosition);
            }

            SetWeaponUiActive(false);
        }

        public void OnWeaponEquipped()
        {
            SetWeaponUiActive(true);
        }

        private void SetWeaponUiActive(bool active)
        {
            foreach (var ui in weaponUI)
            {
                foreach (Transform c in ui.transform)
                {
                    if (!active
                        || weaponCarrier.CurrentWeaponHasTag(c.gameObject.tag))
                    {
                        c.gameObject.SetActive(active);
                    }
                }
            }
        }

        private void OnMatrixModeEvent()
        {
            if (canTriggerMatrixModeEvent)
            {
                canTriggerMatrixModeEvent = false;

                slowMotion.DoSlowMotion();

                const float matrixModeEventCooldownTime = 10.0f;
                Invoke(nameof(EnableMatrixModeEvent), matrixModeEventCooldownTime);
            }
        }

        private void EnableMatrixModeEvent()
        {
            canTriggerMatrixModeEvent = true;
        }

        public void OnDied()
        {
            GameManager.Instance.OnPlayerDied();
        }

        private bool HasIncomingBullet(Collider2D[] nearbyBullets)
        {
            foreach (var bullet in nearbyBullets)
            {
                var bulletRb = bullet.GetComponent<Rigidbody2D>();

                if (Vector2.Dot(BodyPosition - bulletRb.position, bulletRb.velocity) > 0.0f)
                {
                    return true;
                }
            }

            return false;
        }

        private static class JoystickRight
        {
            public static bool pressed = false;
            public static int positionX = 0;
            public static int positionY = 0;
        }

        private static class JoystickLeft
        {
            public static bool pressed = false;
            public static int positionX = 0;
            public static int positionY = 0;
        }
    }
}
