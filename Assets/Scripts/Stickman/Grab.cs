using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/uVtDyPmorA0
    /// </summary>
    public sealed class Grab : MonoBehaviour, IDisableableLimbComponent
    {
        private bool grab = false;

        private Joint2D joint = null;

        public void EnableGrabbing(bool enable)
        {
            grab = enable;

            if (!grab)
            {
                DestroyCurrentJoint();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (grab && enabled)
            {
                var grabbable = collision.transform.GetComponent<Grabbable>();

                if (grabbable != null)
                {
                    if (joint == null)
                    {
                        AudioManager.Instance.CreateTemporaryAudioSourceAt("Grab", transform.position);
                    }

                    DestroyCurrentJoint();

                    joint = gameObject.AddComponent<FixedJoint2D>();
                    grabbable.Grab(joint);
                }
            }
        }

        private void DestroyCurrentJoint()
        {
            if (joint != null)
            {
                Destroy(joint);
                joint = null;
            }
        }

        void IDisableableLimbComponent.DisableLimbComponent()
        {
            DestroyCurrentJoint();
            enabled = false;
        }
    }
}
