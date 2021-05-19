using UnityEngine;

namespace ss
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private GameObject handle;

        private IEnemyWeaponStrategy enemyStrategy;

        private Collider2D[] colliders;
        private Vector3 weaponToHandleOffset;

        private WeaponCarrier carrier;
        private MovementController carrierMovement;

        private int weaponLayer;
        private int weaponEquippedLayer;

        public bool IsEquipped()
        {
            return carrier != null
                && carrierMovement != null;
        }

        public IEnemyWeaponStrategy EnemyStrategy { get => enemyStrategy; }

        public MovementController CarrierMovement { get => carrierMovement; }

        public Collider2D[] Colliders { get => colliders; }

        private void Awake()
        {
            enemyStrategy = GetComponent<IEnemyWeaponStrategy>();
            colliders = GetComponentsInChildren<Collider2D>();
            weaponToHandleOffset = handle.transform.position - transform.position; // weapon to handle

            weaponLayer = LayerMask.NameToLayer("Weapon");
            weaponEquippedLayer = LayerMask.NameToLayer("WeaponEquipped");
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsEquipped())
            {
                var bodyPart = collision.collider.GetComponent<HittableLimb>();

                if (bodyPart != null && bodyPart.IsDestroyed)
                {
                    // collided with a dead body part... nothing to do.
                }
                else
                {
                    var newCarrier = ObjectFinder.FindComponentInParentOrChildren<WeaponCarrier>(collision.gameObject);

                    if (newCarrier != null)
                    {
                        Equip(newCarrier);
                    }
                }
            }
        }

        public void Equip(WeaponCarrier newCarrier)
        {
            if (!IsEquipped())
            {
                if (newCarrier.Equip(this, weaponToHandleOffset))
                {
                    carrier = newCarrier;
                    carrierMovement = carrier.GetComponent<MovementController>();

                    OnEquipped();
                }
            }
        }

        public void Unequip()
        {
            if (IsEquipped())
            {
                carrier = null;
                carrierMovement = null;

                OnUnequipped();
            }
        }

        protected virtual void OnEquipped()
        {
            ObjectFinder.ForEachObjectInHierarchy(gameObject, obj => obj.layer = weaponEquippedLayer);
        }

        protected virtual void OnUnequipped()
        {
            ObjectFinder.ForEachObjectInHierarchy(gameObject, obj => obj.layer = weaponLayer);
        }

        public virtual bool Fire()
        {
            return false;
        }

        public virtual void UpdateWeaponArms(ArmRotator[] weaponArms) { }

        public virtual bool HasTag(string tag)
        {
            return tag.Equals("Untagged");
        }
    }
}
