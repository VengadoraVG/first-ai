using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Villager {
    public class AI : MonoBehaviour {
        private NavMeshAgent _agent;
        private EnemyDetector _detector;

        void Start () {
            _agent = GetComponent<NavMeshAgent>();
            _detector = transform.Find("enemy detector").GetComponent<EnemyDetector>();
            _detector.OnTargetSwitch += _TargetSwitchHandler;
        }

        private void _TargetSwitchHandler (Collider newTarget, Collider oldTarget) {
            if (newTarget != null) {
                _agent.SetDestination(newTarget.transform.position);
            }
        }
   }
}
