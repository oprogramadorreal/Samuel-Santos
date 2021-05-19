using UnityEngine;

namespace ss
{
    public sealed class Gun : Weapon
    {
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private GameObject gunFireParticlesPrefab;

        [SerializeField]
        private Transform bulletOrigin;

        private readonly Vector2 defaultArmsDirection = Vector2.right + Vector2.up * 0.1f;

        private float cooldownTimeAcc = 0.0f;

        private void Start()
        {
            defaultArmsDirection.Normalize();
        }

        private void Update()
        {
            cooldownTimeAcc -= Time.deltaTime;
        }

        public override void UpdateWeaponArms(ArmRotator[] weaponArms)
        {
            foreach (var arm in weaponArms)
            {
                arm.RotateToDirection(defaultArmsDirection, 1.0f);
            }
        }

        public override bool Fire()
        {
            if (cooldownTimeAcc > 0.0f)
            {
                return false;
            }

            var bullet = Instantiate(bulletPrefab, transform);
            bullet.transform.SetPositionAndRotation(bulletOrigin.position, bulletOrigin.rotation);

            var bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.velocity = Vector2.zero;
            bulletBody.AddForce(bullet.transform.TransformVector(Vector2.right) * 50.0f, ForceMode2D.Impulse);

            var bulletComp = bullet.GetComponent<GunBullet>();
            bulletComp.SetSourceGun(this);

            var gunFireParticles = Instantiate(gunFireParticlesPrefab, transform);
            gunFireParticles.transform.position = bulletOrigin.position;
            Destroy(gunFireParticles, 1.0f);

            cooldownTimeAcc = 0.1f;

            AudioManager.Instance.CreateTemporaryAudioSourceAt("GunShot", bulletOrigin.position);

            return true;
        }

        public override bool HasTag(string tag)
        {
            return base.HasTag(tag)
                || tag.Equals("Gun");
        }

        protected override void OnEquipped()
        {
            base.OnEquipped();
            AudioManager.Instance.CreateTemporaryAudioSourceAt("GunEquip", transform.position);
        }
    }
}
