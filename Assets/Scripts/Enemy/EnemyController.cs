using UnityEngine;

namespace ss
{
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private WeaponCarrier weaponCarrier;

        [SerializeField]
        private GameObject weaponPrefab;

        [SerializeField]
        private PlayerController targetPlayer = null;

        private Weapon weapon = null;
        private IEnemyWeaponStrategy weaponStrategy = null;
        
        private void Start()
        {
            CreateAndEquipWeapon();
        }

        private void CreateAndEquipWeapon()
        {
            if (weaponPrefab != null)
            {
                var weaponObject = Instantiate(weaponPrefab);
                EquipWeapon(weaponObject.GetComponent<Weapon>());

                SetupWeaponLevelPart(weaponObject);
            }
        }

        private void SetupWeaponLevelPart(GameObject weaponObject)
        {
            var levelPart = GetComponentInChildren<LevelPartObject>();

            if (levelPart != null)
            {
                var weaponLevelPart = weaponObject.GetComponentInChildren<LevelPartObject>();

                if (weaponLevelPart != null)
                {
                    weaponLevelPart.Setup(levelPart);
                }
            }
        }

        private void EquipWeapon(Weapon newWeapon)
        {
            weapon = newWeapon;
            weapon.Equip(weaponCarrier);

            // Enemies use weapons on their left hand. That's why I negate weapon Y scale here.
            weapon.transform.localScale = new Vector3(weapon.transform.localScale.x, -Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);

            weaponStrategy = weapon.EnemyStrategy;
        }

        private void Update()
        {
            var player = FindPlayer();

            if (CanUpdateStrategy(player))
            {
                weaponStrategy.StrategyUpdate(player);
            }
        }

        private bool CanUpdateStrategy(PlayerController player)
        {
            return weaponStrategy != null
                && weaponCarrier != null
                && weaponCarrier.IsEquipped()
                && player != null; // player can be destroyed
        }

        private PlayerController FindPlayer()
        {
            if (targetPlayer != null)
            {
                return targetPlayer;
            }

            var gameManager = GameManager.Instance;

            if (gameManager != null)
            {
                return gameManager.Player;
            }

            return null;
        }

        public void UnequipWeaponAndDisableEquip()
        {
            if (weaponCarrier != null)
            {
                weaponCarrier.Unequip();
                weaponCarrier.EquippingDisabled = true;
            }
        }
    }
}
