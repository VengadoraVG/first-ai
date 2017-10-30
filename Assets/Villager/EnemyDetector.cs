using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace Villager {
    public class EnemyDetector : MonoBehaviour {
        public delegate void TargetMoveDelegate (Vector3 newPosition, Vector3 oldPosition);
        public event TargetMoveDelegate OnTargetMove;

        public delegate void TargetSwitchDelegate (Collider newTarget, Collider oldTarget);
        public event TargetSwitchDelegate OnTargetSwitch;
        public Collider Target;

        private NavMeshAgent _agent;
        private Dictionary<int, Enemy> _enemies = new Dictionary<int, Enemy>();

        private class Enemy {
            public Collider TheCollider;
            public bool IsBlacklisted;

            public Enemy (Collider collider, bool blacklisted = false) {
                TheCollider = collider;
                IsBlacklisted = blacklisted;
            }
        }


        void Start () {
            _agent = transform.parent.GetComponent<NavMeshAgent>();
        }

        void OnTriggerEnter (Collider c) {
            _enemies[c.GetInstanceID()] = new Enemy(c);
            c.GetComponent<MovementDetector>().OnMovement += _NonTargetMovedHandler;
            if (_IsCloserThanTarget(c)) {
                SetTarget(c);
            }
        }

        void OnTriggerExit (Collider c) {
            _enemies.Remove(c.GetInstanceID());
            c.GetComponent<MovementDetector>().OnMovement -= _NonTargetMovedHandler;
            if (Target == c) {
                SetTarget(_GetBestTarget());
            }
        }

        private bool _IsCloserThanTarget (Collider applicant) {
            if (Target == null) return true;
            NavMeshPath path = new NavMeshPath();

            _agent.CalculatePath(Target.transform.position, path);
            float currentDistance = Util.Distance(path);

            _agent.CalculatePath(applicant.transform.position, path);
            float newDistance = Util.ClampedDistance(path, currentDistance);

            return newDistance < currentDistance;
        }

        private Collider _GetBestTarget (Collider bestSoFar = null) {
            Collider best = bestSoFar;
            NavMeshPath pathFound = new NavMeshPath();
            float shortestDistance = Mathf.Infinity;

            if (best != null) {
                _agent.CalculatePath(best.transform.position, pathFound);
                shortestDistance = Util.Distance(pathFound);
            }

            foreach (KeyValuePair<int, Enemy> entry in _enemies) {

                if (!entry.Value.IsBlacklisted) {
                    _agent.CalculatePath(entry.Value.TheCollider.transform.position, pathFound);
                    float currentDistance = Util.ClampedDistance(pathFound, shortestDistance);
                    if (currentDistance < shortestDistance) {
                        best = entry.Value.TheCollider;
                        shortestDistance = currentDistance;
                    }
                }
            }

            return best;
        }

        private void _TargetMovedHandler (MovementDetector detector, Vector3 oldPosition) {
            Collider oldTarget = Target;
            SetTarget(_GetBestTarget(Target));

            if (oldTarget == Target && OnTargetMove != null && Target != null) {
                OnTargetMove(detector.transform.position, oldPosition);
            }
        }

        private void _NonTargetMovedHandler (MovementDetector detector, Vector3 oldPosition) {
            Collider movedOne = detector.GetComponent<Collider>();

            if (_IsCloserThanTarget(movedOne)) {
                SetTarget(movedOne);
            }
        }
        
        public void UnBlacklist (GameObject enemy) {
            Collider c = enemy.GetComponent<Collider>();
            if (_enemies.ContainsKey(c.GetInstanceID())) {
                _enemies[c.GetInstanceID()].IsBlacklisted = false;
            }

            SetTarget(_GetBestTarget(Target));
        }

        public void PutInBlacklist (GameObject enemy) {
            Collider c = enemy.GetComponent<Collider>();
            if (_enemies.ContainsKey(c.GetInstanceID())) {
                _enemies[c.GetInstanceID()].IsBlacklisted = true;
            }

            if (c == Target) {
                SetTarget(_GetBestTarget());
            }
        }

        public void SetTarget (Collider newTarget) {
            Collider oldTarget = Target;
            Target = newTarget;

            if (oldTarget != null) {
                oldTarget.GetComponent<MovementDetector>().OnMovement -= _TargetMovedHandler;
            }

            if (Target != null) {
                MovementDetector detector = Target.GetComponent<MovementDetector>();
                detector.OnMovement -= _NonTargetMovedHandler;
                detector.OnMovement += _TargetMovedHandler;
            }

            if (newTarget != oldTarget && OnTargetSwitch != null) OnTargetSwitch(newTarget, oldTarget);
        }
    }
}
