using UnityEngine;

namespace ss
{
    public interface IEnemyWeaponStrategy
    {
        void StrategyUpdate(PlayerController player);
    }
}
