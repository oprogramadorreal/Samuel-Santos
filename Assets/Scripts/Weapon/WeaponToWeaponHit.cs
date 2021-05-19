using UnityEngine;

namespace ss
{
    public sealed class WeaponToWeaponHit : MonoBehaviour
    {
        [SerializeField]
        private GameObject sparkParticles;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var other = collision.collider.GetComponent<WeaponToWeaponHit>();

            if (other != null)
            {
                var particles = Instantiate(sparkParticles, transform, true);
                particles.transform.position = collision.contacts[0].point;
                Destroy(particles, 1.0f);

                AudioManager.Instance.CreateTemporaryAudioSourceAt("SwordHit", transform.position);
            }
        }
    }
}
