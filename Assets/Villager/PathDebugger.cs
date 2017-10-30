using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Villager {
    public class PathDebugger : MonoBehaviour {
        public float radius;
        public Color lineColor;
        public Color sphereColor;

        public float distance;
        public int cornersLength;

        private NavMeshAgent _agent;
        private Vector3 _point;
        private Color _pointColor = Color.green;

        void Start () {
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update () {
            distance = Util.Distance(_agent.path);
            cornersLength = _agent.path.corners.Length;

            if (Input.GetMouseButton(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<LayerMask.NameToLayer("Default"))) {
                    _agent.SetDestination(hit.point);
                    _point = hit.point;
                    _pointColor = Color.green;
                } else {
                    _pointColor = Color.black;
                }
            }
        }

        void OnDrawGizmos () {
            if (Application.isPlaying && _agent.path.corners.Length > 0) {
                Gizmos.DrawSphere(_agent.path.corners[0], radius);
                for (int i=1; i<_agent.path.corners.Length; i++) {
                    Gizmos.color = lineColor;
                    Gizmos.DrawLine(_agent.path.corners[i-1], _agent.path.corners[i]);
                    Gizmos.color = sphereColor;
                    Gizmos.DrawSphere(_agent.path.corners[i], radius);
                }
            }
            Gizmos.color = _pointColor;
            Gizmos.DrawSphere(_point, radius * 1.5f);
        }
    }
}
