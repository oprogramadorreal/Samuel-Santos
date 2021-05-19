using System;
using System.Collections.Generic;
using UnityEngine;

namespace ss
{
    public static class ObjectFinder
    {
        public static IEnumerable<GameObject> FindObjectsInLayer(string layer)
        {
            return FindObjectsInLayer(LayerMask.NameToLayer(layer));
        }

        public static IEnumerable<GameObject> FindObjectsInLayer(int layer)
        {
            var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach (var o in allObjects)
            {
                if (o.layer == layer)
                {
                    yield return o;
                }
            }
        }

        public static T FindComponentInParentOrChildren<T>(GameObject obj) where T : Component
        {
            var comp = obj.GetComponentInParent<T>();

            if (comp == null)
            {
                comp = obj.GetComponentInChildren<T>();
            }

            return comp;
        }

        public static void ForEachObjectInHierarchy(GameObject root, Action<GameObject> action)
        {
            action(root);

            foreach (Transform c in root.transform)
            {
                ForEachObjectInHierarchy(c.gameObject, action);
            }
        }
    }
}
