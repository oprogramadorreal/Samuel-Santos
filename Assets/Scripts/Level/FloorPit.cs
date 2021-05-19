using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ss
{
    public sealed class FloorPit : MonoBehaviour
    {
        private IEnumerable<Collider2D> groundColliders;

        private void Awake()
        {
            groundColliders = ObjectFinder.FindObjectsInLayer("Ground")
                .Select(o => o.GetComponent<Collider2D>())
                .Where(o => o != null);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            SetIgnoreCollisions(collider, true);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            SetIgnoreCollisions(collider, false);
        }

        private void SetIgnoreCollisions(Collider2D collider, bool ignore)
        {
            foreach (var c in groundColliders)
            {
                Physics2D.IgnoreCollision(collider, c, ignore);
            }
        }
    }
}
