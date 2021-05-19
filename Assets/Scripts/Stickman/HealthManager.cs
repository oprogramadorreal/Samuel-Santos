using UnityEngine;
using UnityEngine.Events;

namespace ss
{
    public sealed class HealthManager : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D body;

        [SerializeField]
        private WeaponCarrier weaponCarrier;

        [SerializeField]
        private GameObject diedParticlesPrefab;

        [SerializeField]
        private UnityEvent diedEvent;

        private int vitalLimbsCount = 3;

        public void OnBodyDestroyed()
        {
            Die();
        }

        public void OnVitalLimbDestroyed()
        {
            --vitalLimbsCount;

            if (vitalLimbsCount <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            diedEvent?.Invoke();

            if (weaponCarrier != null)
            {
                weaponCarrier.Unequip(0.0f);
            }

            if (diedParticlesPrefab != null)
            {
                var diedParticles = Instantiate(diedParticlesPrefab, body.position, Quaternion.identity, null);
                Destroy(diedParticles, 1.5f);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            AudioManager.Instance.CreateTemporaryAudioSourceAt("StickmanDie", body.position);
        }
    }
}
