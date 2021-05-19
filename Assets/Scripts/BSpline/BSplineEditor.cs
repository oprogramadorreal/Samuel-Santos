using UnityEngine;
using UnityEngine.Events;

namespace ss
{
    public sealed class BSplineEditor : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private BSpline curve;

        [SerializeField]
        private GameObject controlPointPrefab;

        [SerializeField]
        private float controlPointSize = 3.5f;

        [SerializeField]
        private UnityEvent CurveModifiedEvent;

        private int controlPointsLayer;

        private readonly Plane curvePlane = new Plane(Vector3.forward, 0.0f);

        private int? selectedControlPointIndex = null;
        private GameObject selectedControlPoint = null;


        private void Awake()
        {
            controlPointsLayer = LayerMask.NameToLayer("BSplineControlPoints");
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                UnselectControlPoint();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                var mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RemoveControlPoint(mouseRay);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                var mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                TryToSelectOrCreateControlPoint(mouseRay);
            }
            else if (Input.GetMouseButton(0) && selectedControlPointIndex.HasValue)
            {
                var mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                MoveSelectedControlPoint(mouseRay);
            }
        }

        private void RemoveControlPoint(Ray mouseRay)
        {
            if (TryToSelectControlPoint(mouseRay))
            {
                Destroy(selectedControlPoint);
                curve.RemoveControlPointAt(selectedControlPointIndex.Value);

                UnselectControlPoint();

                OnCurveModifiedEvent();
            }
        }

        private void TryToSelectOrCreateControlPoint(Ray mouseRay)
        {
            if (!TryToSelectControlPoint(mouseRay))
            {
                CreateControlPoint(mouseRay);
            }
        }

        private bool TryToSelectControlPoint(Ray mouseRay)
        {
            if (!Physics.Raycast(mouseRay, out var hitInfo, float.MaxValue, 1 << controlPointsLayer))
            {
                return false;
            }

            selectedControlPoint = hitInfo.collider.gameObject;
            selectedControlPointIndex = GetCloseControlPointIndex(selectedControlPoint.transform.position);

            return true;
        }

        private void UnselectControlPoint()
        {
            selectedControlPoint = null;
            selectedControlPointIndex = null;
        }

        private void CreateControlPoint(Ray mouseRay)
        {
            if (curvePlane.Raycast(mouseRay, out var enter))
            {
                var hitPoint = mouseRay.GetPoint(enter);

                selectedControlPointIndex = curve.AddControlPoint(hitPoint);

                var controlPointObject = Instantiate(controlPointPrefab, hitPoint, Quaternion.identity, transform);
                controlPointObject.transform.localScale = new Vector3(controlPointSize, controlPointSize, controlPointSize);

                selectedControlPoint = controlPointObject;

                OnCurveModifiedEvent();
            }
        }

        private void MoveSelectedControlPoint(Ray mouseRay)
        {
            if (curvePlane.Raycast(mouseRay, out var enter))
            {
                var hitPoint = mouseRay.GetPoint(enter);

                selectedControlPoint.transform.position = hitPoint;
                curve.SetControlPoint(selectedControlPointIndex.Value, hitPoint);

                OnCurveModifiedEvent();
            }
        }

        private int? GetCloseControlPointIndex(Vector3 point)
        {
            const float distanceEpsilon = 0.01f;

            for (var i = 0; i < curve.NumberOfControlPoints; ++i)
            {
                if (Vector3.Distance(curve.GetControlPoint(i), point) < distanceEpsilon)
                {
                    return i;
                }
            }

            return null;
        }

        private void OnCurveModifiedEvent()
        {
            CurveModifiedEvent?.Invoke();
        }
    }
}
