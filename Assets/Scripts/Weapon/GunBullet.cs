using UnityEngine;

namespace ss
{
    public sealed class GunBullet : MonoBehaviour
    {
        [SerializeField]
        private GameObject destroyedParticles;

        private Gun sourceGun;

        private bool needsToBeDestroyed = false;

        private const float maxBulletLifeTime = 2.0f;

        private void Start()
        {
            Invoke(nameof(DestroyImpl), maxBulletLifeTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (sourceGun.transform.root != collision.collider.transform.root) // ignore collisions with source gun
            {
                needsToBeDestroyed = true;
            }
        }

        private void LateUpdate()
        {
            if (needsToBeDestroyed)
            {
                DestroyImpl();
            }
        }

        public void SetSourceGun(Gun sourceGun)
        {
            this.sourceGun = sourceGun;
        }

        private void DestroyImpl()
        {
            AudioManager.Instance.CreateTemporaryAudioSourceAt("BulletHit", transform.position);

            var particles = Instantiate(destroyedParticles, transform.position, transform.rotation);
            Destroy(particles, 1.0f);

            Destroy(gameObject);
        }
    }
}
