using UnityEngine;

namespace ss
{
    public sealed class GunStrategy : MonoBehaviour, IEnemyWeaponStrategy
    {
        [SerializeField]
        private Gun gun;

        private const float walkMinDistanceThreshold = 10.0f;

        private float fireTimeAcc = 0.0f;
        private bool playerWasTooClose = false;

        private void Start()
        {
            fireTimeAcc = GetNextTimeToFire(false);
        }

        void IEnemyWeaponStrategy.StrategyUpdate(PlayerController player)
        {
            if (!gun.IsEquipped())
            {
                return;
            }

            var movement = gun.CarrierMovement;
            var position = movement.BodyPosition;
            var playerPosition = player.BodyPosition;

            var playerDistance = Mathf.Abs(playerPosition.x - position.x); // player distance in X

            var playerIsTooClose = playerDistance < walkMinDistanceThreshold;

            if (playerIsTooClose != playerWasTooClose)
            {
                fireTimeAcc = GetNextTimeToFire(playerIsTooClose);
                playerWasTooClose = playerIsTooClose;
            }
            else
            {
                fireTimeAcc -= Time.deltaTime;

                if (fireTimeAcc <= 0.0f)
                {
                    gun.Fire();
                    fireTimeAcc = GetNextTimeToFire(playerIsTooClose);
                }
            }

            movement.SetInputAxisForArmsMovement((playerPosition - position).normalized);

            if (playerIsTooClose)
            {
                var moveDirection = playerPosition.x < position.x ? 1.0f : -1.0f;
                movement.SetInputAxisForBodyMovement(new Vector2(moveDirection, 0.0f));
            }
        }

        private float GetNextTimeToFire(bool playerIsTooClose)
        {
            if (playerIsTooClose)
            {
                return Random.Range(0.5f, 1.0f);
            }
            else
            {
                return Random.Range(2.0f, 4.0f);
            }
        }
    }
}
