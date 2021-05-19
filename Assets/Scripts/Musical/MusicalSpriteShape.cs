using UnityEngine;
using UnityEngine.U2D;

namespace ss
{
    /// <summary>
    /// This is an attempt to use the spline from SpriteShapeController instead of my own BSpline class.
    /// I might be doing something wrong but this seems to be too slow (FPS drops significantly when using this).
    /// </summary>
    public sealed class MusicalSpriteShape : MonoBehaviour
    {
        [SerializeField]
        private AudioSampler audioSampler;

        [SerializeField]
        private SpriteShapeController spriteShape;

        [SerializeField]
        private float curveSpacingX = 1;

        [SerializeField]
        private float audioSamplerScaler = 50.0f;

        [SerializeField]
        private float meshThickness = 20.0f;

        private Spline curve;

        private void Start()
        {
            curve = spriteShape.spline;

            var points = CreateCurvePoints(audioSampler.NumberOfUserSamples);
            CreateCurvePoints(points);
        }

        private void LateUpdate()
        {
            var audioSamples = audioSampler.UserSamples;

            for (var i = 0; i < audioSamples.Length; ++i)
            {
                var pointIndex = i + 1; // curve has two extra points: at the beginning and end
                var point = curve.GetPosition(pointIndex);
                var newPoint = new Vector3(point.x, audioSamples[i] * audioSamplerScaler, point.z);

                curve.SetPosition(pointIndex, newPoint);
            }
        }

        private Vector3[] CreateCurvePoints(int numberOfCurvePoints)
        {
            var result = new Vector3[numberOfCurvePoints];

            var spectrumLength = numberOfCurvePoints * curveSpacingX;
            var midPoint = spectrumLength / 2;

            for (var i = 0; i < numberOfCurvePoints; ++i)
            {
                result[i] = new Vector3(i * curveSpacingX - midPoint, 0, 0);
            }

            return result;
        }

        private void CreateCurvePoints(Vector3[] points)
        {
            curve.Clear();

            AddCurvePoint(0, points[0] + Vector3.down * meshThickness);

            for (var i = 0; i < points.Length; ++i)
            {
                AddCurvePoint(i + 1, points[i]);
            }

            AddCurvePoint(points.Length + 1, points[points.Length - 1] + Vector3.down * meshThickness);
        }

        private void AddCurvePoint(int pointIndex, Vector3 point)
        {
            curve.InsertPointAt(pointIndex, point);
            curve.SetTangentMode(pointIndex, ShapeTangentMode.Continuous);
        }
    }
}
