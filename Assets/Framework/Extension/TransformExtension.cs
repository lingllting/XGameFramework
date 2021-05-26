using System.Collections.Generic;
using UnityEngine;

namespace XGameFramework
{
    public static class TransformExtension
    {
        public static Transform FindRecursively(this Transform parent, string childName)
        {
            //Breadth-first search
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == childName)
                    return c;
                foreach(Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
 
            /*
            //Depth-first search
            foreach(Transform child in parent)
            {
                if(child.name == childName )
                    return child;
                var result = child.FindRecursively(childName);
                if (result != null)
                    return result;
            }
            return null;
            */
        }

        public static void AddChild(this Transform parent, Transform child)
        {
            child.SetParent(parent, false);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
        }

        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
            {
                return "";
            }

            return (current.parent.GetPath() + "/" + current.name).Trim('/');
        }
    }
}
