using System;
using System.Collections.Generic;
using UnityEngine;

namespace ss
{
    /// <summary>
    /// A simple B-spline abstraction based on https://github.com/Tagussan/BSpline
    /// </summary>
    public sealed class BSpline : MonoBehaviour
    {
        [SerializeField]
        private int degree = 2;

        [SerializeField]
        private List<Vector3> points = new List<Vector3>();

        private int baseFuncRangeInt;
        private Func<float, float> baseFunc;

        private void Awake()
        {
            if (degree == 2)
            {
                baseFunc = BasisDegree2;
                baseFuncRangeInt = 2;
            }
            else
            {
                baseFunc = BasisDegree3;
                baseFuncRangeInt = 2;
            }
        }

        public int NumberOfControlPoints { get => points.Count; }

        public int AddControlPoint(Vector3 newPoint)
        {
            var index = points.Count;
            points.Add(newPoint);
            return index;
        }

        public void RemoveControlPointAt(int index)
        {
            points.RemoveAt(index);
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 newValue)
        {
            points[index] = newValue;
        }

        public Vector3[] Sample(int numberOfPoints)
        {
            var result = new Vector3[numberOfPoints];
            var tStep = 1.0f / (numberOfPoints - 1.0f);

            for (var i = 0u; i < numberOfPoints; ++i)
            {
                var t = i * tStep;
                result[i] = SampleAt(t);
            }

            return result;
        }

        public Vector3 SampleAt(float t)
        {
            var t2 = t * ((degree + 1.0f) * 2.0f + points.Count); // t must be in [0, 1]

            return new Vector3(
                GetInterpolated(GetSequenceAtFunc(0), t2),
                GetInterpolated(GetSequenceAtFunc(1), t2),
                GetInterpolated(GetSequenceAtFunc(2), t2)
            );
        }

        private Func<int, float> GetSequenceAtFunc(int dimension)
        {
            var margin = degree + 1;

            return (int n) =>
            {
                if (n < margin)
                {
                    return points[0][dimension];
                }
                else if (points.Count + margin <= n)
                {
                    return points[points.Count - 1][dimension];
                }

                return points[n - margin][dimension];
            };
        }

        private float GetInterpolated(Func<int, float> sequence, float t)
        {
            var tInt = Mathf.FloorToInt(t);
            var result = 0.0f;

            for (var i = tInt - baseFuncRangeInt; i <= tInt + baseFuncRangeInt; ++i)
            {
                result += sequence(i) * baseFunc(t - i);
            }

            return result;
        }

        private static float BasisDegree2(float x)
        {
            if (-0.5f <= x && x < 0.5f)
            {
                return 0.75f - x * x;
            }
            else if (0.5f <= x && x <= 1.5f)
            {
                return 1.125f + (-1.5f + x / 2.0f) * x;
            }
            else if (-1.5f <= x && x < -0.5f)
            {
                return 1.125f + (1.5f + x / 2.0f) * x;
            }

            return 0.0f;
        }

        private static float BasisDegree3(float x)
        {
            if (-1.0f <= x && x < 0.0f)
            {
                return 2.0f / 3.0f + (-1.0f - x / 2.0f) * x * x;
            }
            else if (1.0f <= x && x <= 2.0f)
            {
                return 4.0f / 3.0f + x * (-2.0f + (1.0f - x / 6.0f) * x);
            }
            else if (-2.0f <= x && x < -1.0f)
            {
                return 4.0f / 3.0f + x * (2.0f + (1.0f + x / 6.0f) * x);
            }
            else if (0.0f <= x && x < 1.0f)
            {
                return 2.0f / 3.0f + (-1.0f + x / 2.0f) * x * x;
            }

            return 0.0f;
        }
    }
}
