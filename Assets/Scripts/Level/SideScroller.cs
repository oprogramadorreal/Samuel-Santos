using System;
using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/zit45k6CUMk
    /// </summary>
    public sealed class SideScroller : MonoBehaviour
    {
        [SerializeField]
        private Renderer myRenderer;

        [SerializeField]
        private GameObject cam;

        [SerializeField]
        private Vector2 parallaxEffect;

        [SerializeField]
        private float intersectionAmount = 0.0f;

        private Vector2 startPos;
        private float length = 0.0f;

        public event EventHandler<ScrollEventArgs> ScrollLeftEvent;

        public event EventHandler<ScrollEventArgs> ScrollRightEvent;

        public event EventHandler<ScrollEventArgs> InitializedEvent;

        private void Start()
        {
            startPos = transform.position;
        }

        public bool Init()
        {
            length = myRenderer.bounds.size.x;

            if (length <= 0.0f)
            {
                return false;
            }

            length *= 1.0f - intersectionAmount;

            var cloneOffset = length / transform.localScale.x;

            var leftClone = CreateChildClone(-cloneOffset);
            var rightClone = CreateChildClone(cloneOffset);

            leftClone.transform.SetParent(transform, false);
            rightClone.transform.SetParent(transform, false);

            InitializedEvent?.Invoke(this, new ScrollEventArgs { PositionX = startPos.x, Offset = length });

            return true;
        }

        private void LateUpdate()
        {
            var temp = cam.transform.position.x * (1.0f - parallaxEffect.x);

            var distX = cam.transform.position.x * parallaxEffect.x;
            var distY = cam.transform.position.y * parallaxEffect.y;

            transform.position = new Vector3(startPos.x + distX, startPos.y + distY, transform.position.z);

            if (temp > startPos.x + length / 2.0f)
            {
                startPos.x += length;
                ScrollRightEvent?.Invoke(this, new ScrollEventArgs { PositionX = startPos.x, Offset = length });
            }
            else if (temp < startPos.x - length / 2.0f)
            {
                startPos.x -= length;
                ScrollLeftEvent?.Invoke(this, new ScrollEventArgs { PositionX = startPos.x, Offset = length });
            }
        }

        private GameObject CreateChildClone(float xOffset)
        {
            var clone = Instantiate(gameObject);
            clone.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            clone.transform.localPosition = new Vector3(xOffset, 0.0f, 0.0f);
            Destroy(clone.GetComponent<SideScroller>()); // removes this script from the clone
            return clone;
        }

        public sealed class ScrollEventArgs : EventArgs
        {
            public float PositionX { get; set; }
            public float Offset { get; set; }
        }
    }
}
