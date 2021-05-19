using UnityEngine;

namespace ss
{
    public sealed class BSplineRenderer : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private int maxSamplingPoints = 1000;

        [SerializeField]
        private BSpline curve;

        public void UpdateRenderer()
        {
            var samplePoints = Mathf.Min(curve.NumberOfControlPoints * 20, maxSamplingPoints);                
            var points = curve.Sample(samplePoints);

            lineRenderer.positionCount = points.Length;

            for (var i = 0; i < points.Length; ++i)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }
    }
}
