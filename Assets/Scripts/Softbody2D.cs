using UnityEngine;
using UnityEngine.U2D;

namespace ss
{
    /// <summary>
    /// Based on https://www.youtube.com/watch?v=F82BlnW5z6g
    /// </summary>
    public sealed class Softbody2D : MonoBehaviour
    {
        private const float splineOffset = 0.5f;

        [SerializeField]
        private SpriteShapeController spriteShape;

        [SerializeField]
        private Transform[] points;

        private CircleCollider2D[] pointsColliders;

        private void Awake()
        {
            pointsColliders = new CircleCollider2D[points.Length];

            for (var i = 0; i < pointsColliders.Length; ++i)
            {
                pointsColliders[i] = points[i].gameObject.GetComponent<CircleCollider2D>();
            }

            UpdateVertices();
        }

        private void Update()
        {
            UpdateVertices();
        }

        private void UpdateVertices()
        {
            for (var i = 0; i < points.Length; ++i)
            {
                var vertex = (Vector2) points[i].localPosition;
                var towardsCenter = (Vector2.zero - vertex).normalized;
                var colliderRadius = pointsColliders[i].radius;

                try
                {
                    spriteShape.spline.SetPosition(i, vertex - towardsCenter * colliderRadius);
                }
                catch
                {
                    spriteShape.spline.SetPosition(i, vertex - towardsCenter * (colliderRadius + splineOffset));
                }

                var lt = spriteShape.spline.GetLeftTangent(i);

                var newLt = Vector2.Perpendicular(towardsCenter) * lt.magnitude;
                var newRt = Vector2.zero - newLt;

                spriteShape.spline.SetRightTangent(i, newRt);
                spriteShape.spline.SetLeftTangent(i, newLt);
            }
        }
    }
}
