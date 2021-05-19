using UnityEngine;

namespace ss
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        public static T Instance { get => instance; }

        /// <summary>
        /// WARNING: If you override this method on the subclass, you MUST call this base class method.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); // is this an error?
            }
            else
            {
                instance = this as T;
            }
        }

        /// <summary>
        /// WARNING: If you override this method on the subclass, you MUST call this base class method.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
