using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on http://wiki.unity3d.com/index.php?title=DepthMask
    /// </summary>
    public sealed class MaskObject : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Renderer>().material.renderQueue = 3002;
        }
    }
}
