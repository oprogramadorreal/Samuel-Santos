using System;
using System.Linq;
using UnityEngine;

namespace ss
{
    public sealed class MusicalMesh : MonoBehaviour
    {
        [SerializeField]
        private BSpline curve;

        [SerializeField]
        private float curveSpacingX = 1;

        [SerializeField]
        private AudioSampler audioSampler;

        [SerializeField]
        private float audioSamplerScaler = 50.0f;

        [SerializeField]
        private float meshThickness = 20.0f;

        private Mesh mesh;
        private Vector3[] meshContour;
        private Vector2[] meshContour2D;

        public event EventHandler MeshUpdatedEvent;

        public Mesh Mesh { get => mesh; }

        /// <summary>
        /// For 2D colliders.
        /// </summary>
        public Vector2[] MeshContour2D { get => meshContour2D; }

        private void Start()
        {
            var points = CreateCurvePoints(audioSampler.NumberOfUserSamples);
            AddCurvePoints(points);

            UpdateCurveAndMesh();
        }

        private void LateUpdate()
        {
            UpdateCurveAndMesh();
        }

        private void UpdateCurveAndMesh()
        {
            UpdateCurveFromAudio();
            UpdateMeshFromCurve();
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

        private void AddCurvePoints(Vector3[] points)
        {
            curve.AddControlPoint(points[0] + Vector3.down * meshThickness);

            for (var i = 0; i < points.Length; ++i)
            {
                curve.AddControlPoint(points[i]);
            }

            curve.AddControlPoint(points[points.Length - 1] + Vector3.down * meshThickness);
        }

        private void UpdateCurveFromAudio()
        {
            var audioSamples = audioSampler.UserSamples;

            for (var i = 0; i < audioSamples.Length; ++i)
            {
                var pointIndex = i + 1; // curve has two extra points: at the beginning and end
                var point = curve.GetControlPoint(pointIndex);
                var newPoint = new Vector3(point.x, audioSamples[i] * audioSamplerScaler, point.z);

                curve.SetControlPoint(pointIndex, newPoint);
            }
        }

        public void UpdateMeshFromCurve()
        {
            meshContour = curve.Sample(200);
            meshContour2D = CreateMeshContour2d(meshContour);

            var indices = new Triangulator2D(meshContour).Triangulate();
            mesh = CreateMesh(meshContour, indices.ToArray());

            MeshUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }

        private static Mesh CreateMesh(Vector3[] vertices, int[] indices)
        {
            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Vector2[] CreateMeshContour2d(Vector3[] meshContour)
        {
            return meshContour
                .Select(v => new Vector2(v.x, v.y))
                .ToArray();
        }
    }
}
