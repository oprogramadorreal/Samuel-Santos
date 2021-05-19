using UnityEngine;

namespace ss
{
    public sealed class SwordStrategy : MonoBehaviour, IEnemyWeaponStrategy
    {
        [SerializeField]
        private Weapon sword;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Vector2 inputAxisForArmsMovement;

        private int swordAttackHash;

        private const float attackDistanceThreshold = 10.0f;
        private const float walkMaxDistanceThreshold = 25.0f;
        private const float walkMinDistanceThreshold = 6.0f;

        private void Start()
        {
            swordAttackHash = Animator.StringToHash("SwordAttack");
        }

        void IEnemyWeaponStrategy.StrategyUpdate(PlayerController player)
        {
            if (!sword.IsEquipped())
            {
                return;
            }

            var movement = sword.CarrierMovement;
            var position = movement.BodyPosition;
            var playerPosition = player.BodyPosition;           

            var playerDistance = Vector3.Distance(playerPosition, position);
            var playerDirection = playerPosition.x < position.x ? -1.0f : 1.0f;

            if (playerDistance < attackDistanceThreshold)
            {
                animator.Play(swordAttackHash);
                movement.SetInputAxisForArmsMovement(new Vector2(playerDirection, inputAxisForArmsMovement.y));
            }

            if (playerDistance > walkMinDistanceThreshold && playerDistance < walkMaxDistanceThreshold)
            {
                movement.SetInputAxisForBodyMovement(new Vector2(playerDirection, 0.0f));
            }
        }
    }
}
