using UnityEngine;

namespace ss
{
    public sealed class LevelPartObject : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D body;

        private PlayerController player;

        private GameObject parentLevelPart;

        private const float maxDistanceThreshold = 50.0f;

        public void Setup(LevelPartObject other)
        {
            Setup(other.parentLevelPart, other.player);
        }

        public void Setup(GameObject parentLevelPart, PlayerController player)
        {
            this.parentLevelPart = parentLevelPart;
            this.player = player;
        }

        private void Update()
        {
            if (player != null)
            {
                if (NeedsToBeDestroyed())
                {
                    Destroy(gameObject);
                }
            }
        }

        private bool NeedsToBeDestroyed()
        {
            return parentLevelPart == null // parentLevelPart is null when it's destroyed
                && IsTooFarFromPlayer();
        }

        private bool IsTooFarFromPlayer()
        {
            return GetDistanceToPlayer() > maxDistanceThreshold;
        }

        private float GetDistanceToPlayer()
        {
            return Vector3.Distance(player.BodyPosition, GetPosition());
        }

        private Vector2 GetPosition()
        {
            if (body != null)
            {
                return body.position;
            }

            return transform.position;
        }
    }
}
