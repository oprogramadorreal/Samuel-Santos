using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://sharpcoderblog.com/blog/drag-rigidbody-with-mouse-cursor-unity-3d-tutorial
    /// </summary>
    public sealed class Rigidbody2DDragger : MonoBehaviour
    {
        [SerializeField]
        private float forceAmount = 500;

        [SerializeField]
        private Camera targetCamera;

        private Rigidbody2D selectedRigidbody;
        private Vector3 originalScreenTargetPosition;
        private Vector3 originalRigidbodyPos;
        private float selectionDistance;

        private void Update()
        {
            if (!targetCamera)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Check if we are hovering over Rigidbody, if so, select it
                selectedRigidbody = GetRigidbodyFromMouseClick();
            }

            if (Input.GetMouseButtonUp(0) && selectedRigidbody)
            {
                //Release selected Rigidbody if there any
                selectedRigidbody = null;
            }
        }

        private void FixedUpdate()
        {
            if (selectedRigidbody)
            {
                var mousePositionOffset = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance)) - originalScreenTargetPosition;
                selectedRigidbody.velocity = (originalRigidbodyPos + mousePositionOffset - selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
            }
        }

        private Rigidbody2D GetRigidbodyFromMouseClick()
        {
            var ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit)
            {
                if (hit.collider.gameObject.GetComponent<Rigidbody2D>())
                {
                    selectionDistance = Vector3.Distance(ray.origin, hit.point);
                    originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance));
                    originalRigidbodyPos = hit.collider.transform.position;

                    return hit.collider.gameObject.GetComponent<Rigidbody2D>();
                }
            }

            return null;
        }
    }
}
