using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Villager {
    public class AI : MonoBehaviour {
        private NavMeshAgent _agent;
        private EnemyDetector _detector;
        private Surrounder _surrounder;

        void Start () {
            _agent = GetComponent<NavMeshAgent>();
            _detector = transform.Find("enemy detector").GetComponent<EnemyDetector>();
            _detector.OnTargetSwitch += _TargetSwitchHandler;
            _detector.OnTargetMove += _TargetMovementHandler;

            _surrounder = GetComponent<Surrounder>();
            _surrounder.OnDumped += _DumpedHandler;
            _surrounder.OnSpotChange += _SpotChangeHandler;
        }

        private void _Blacklist (GameObject enemy) {
            _detector.PutInBlacklist(enemy);
            enemy.GetComponent<Surroundings>().OnSpotFreed += _SpotFreedHandler;
        }

        private void _TargetMovementHandler (Vector3 newPosition, Vector3 oldPosition) {
            if (_surrounder.CurrentSpot != null) {
                _agent.SetDestination(_surrounder.CurrentSpot.Position);
            }
        }

        private void _TargetSwitchHandler (Collider newTarget, Collider oldTarget) {
            if (newTarget != null) {
                if (newTarget.GetComponent<Surroundings>().AttemptToSurround(_surrounder) == null)
                    _Blacklist(newTarget.gameObject);
            } else {
                _surrounder.AbandonSurrounding();
                _agent.ResetPath();
            }
        }

        private void _SpotChangeHandler (Spot oldSpot, Spot newSpot) {
            if (newSpot == null) {
                _agent.ResetPath();
                return;
            } else {
                _agent.SetDestination(newSpot.Position);
            }
        }

        private void _DumpedHandler (Surroundings dumpedFrom) {
            _Blacklist(dumpedFrom.gameObject);
        }

        private void _SpotFreedHandler (Spot freed) {
            _detector.UnBlacklist(freed.Owner.gameObject);
            freed.Owner.OnSpotFreed -= _SpotFreedHandler;
        }
   }
}
