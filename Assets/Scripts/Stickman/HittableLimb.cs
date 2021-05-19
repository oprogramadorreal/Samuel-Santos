using UnityEngine;
using UnityEngine.Events;

namespace ss
{
    /// <summary>
    /// Can be hit by objects with WeaponToLimbHit component.
    /// </summary>
    public sealed class HittableLimb : MonoBehaviour
    {
        [SerializeField]
        private float health = 10.0f;

        [SerializeField]
        private Joint2D jointToDestroy = null;

        [SerializeField]
        private GameObject damagedParticlesPrefab;

        [SerializeField]
        private GameObject destroyedParticlesPrefab;

        [SerializeField]
        private UnityEvent limbDestroyedEvent;

        private float initialHealth;

        private SpriteRenderer spriteRenderer;
        private Color initialColor;
        private readonly Color finalColor = Color.black;

        public bool IsDestroyed { get; private set; } = false;

        private void Start()
        {
            if (jointToDestroy == null)
            {
                jointToDestroy = GetComponent<Joint2D>();
            }

            initialHealth = health;

            spriteRenderer = GetComponent<SpriteRenderer>();
            initialColor = spriteRenderer.color;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryToApplyHit(collision, 1.0f);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryToApplyHit(collision, 0.05f);
        }

        private void Update()
        {
            if (!IsDestroyed)
            {
                health = Mathf.Min(health + initialHealth * 0.025f * Time.deltaTime, initialHealth);
                spriteRenderer.color = CalculateCurrentColor();
            }
        }

        private void TryToApplyHit(Collision2D collision, float strengthFactor)
        {
            if (!isActiveAndEnabled || IsDestroyed)
            {
                return;
            }

            if (health > 0.0f)
            {
                var hit = collision.collider.GetComponent<WeaponToLimbHit>();

                if (hit != null)
                {
                    TryToApplyHit(hit, strengthFactor);
                }
            }
        }

        private void TryToApplyHit(WeaponToLimbHit hit, float strengthFactor)
        {
            var hitStrenth = hit.GetStrenth();

            if (hitStrenth > 0.0f)
            {
                if (strengthFactor >= 1.0f && damagedParticlesPrefab != null)
                {
                    var damageParticles = Instantiate(damagedParticlesPrefab, transform.position, transform.rotation, transform);
                    Destroy(damageParticles, 1.0f);

                    AudioManager.Instance.CreateTemporaryAudioSourceAt("BodyHit", transform.position);
                }

                var finalHitStrenth = hitStrenth * strengthFactor;
                ApplyHit(finalHitStrenth);
            }
        }

        private void ApplyHit(float hitStrenth)
        {
            health = Mathf.Max(0.0f, health - hitStrenth);
            spriteRenderer.color = CalculateCurrentColor();

            if (health <= 0.0f)
            {
                DestroyLimb(true);
            }
        }

        private Color CalculateCurrentColor()
        {
            var healthPercentage = 1.0f - health / initialHealth;
            return Color.Lerp(initialColor, finalColor, healthPercentage);
        }

        public void DestroyLimb(bool destroyJoint)
        {
            if (IsDestroyed)
            {
                return;
            }

            if (destroyJoint && jointToDestroy != null)
            {
                Destroy(jointToDestroy);
                jointToDestroy = null;

                transform.SetParent(null, true);
            }

            var componentsToDisable = GetComponents<IDisableableLimbComponent>();

            foreach (var c in componentsToDisable)
            {
                c.DisableLimbComponent();
            }

            IsDestroyed = true;

            limbDestroyedEvent?.Invoke();

            Invoke(nameof(CreateDestroyedParticles), Random.Range(1.0f, 4.0f));

            AudioManager.Instance.CreateTemporaryAudioSourceAt("LimbDestroyed", transform.position);
        }

        private void CreateDestroyedParticles()
        {
            if (destroyedParticlesPrefab != null)
            {
                var particles = Instantiate(destroyedParticlesPrefab, transform.position, transform.rotation, transform);
                spriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                Invoke(nameof(DestroyLimbImpl), 1.0f);
            }
        }

        private void DestroyLimbImpl()
        {
            Destroy(gameObject);
        }
    }
}
