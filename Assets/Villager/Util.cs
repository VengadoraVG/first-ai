using UnityEngine;
using UnityEngine.AI;

namespace Villager {
    public class Util {
        public static float Distance (NavMeshPath path) {
            return ClampedDistance(path);
        }

        public static float ClampedDistance (NavMeshPath path, float maxDistance=Mathf.Infinity) {
            float distance = 0;

            for (int i=1; i<path.corners.Length && distance < maxDistance; i++) {
                distance += Vector3.Distance(path.corners[i], path.corners[i-1]);
            }

            return distance;
        }
                      
        public static float FullAngle (Vector2 v) {
            float angle = Mathf.PI + Mathf.Atan(v.y / v.x);
            if (v.x > 0) {
                angle += 2 * Mathf.PI;
            } else {
                angle += Mathf.PI;
            }
            return angle % (Mathf.PI * 2);
        }

        public static float Distance (Vector3 a, Vector3 b) {
            return (a - b).magnitude;
        }

        public static T FindParentWithComponent<T> (Transform child) where T : Behaviour {
            if (child == null)
                return null;
            T component = child.gameObject.GetComponent<T>();
            if (component != null)
                return component;
            return FindParentWithComponent<T>(child.parent);
        }
    }
}
