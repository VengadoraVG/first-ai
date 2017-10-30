using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Villager {
    public class Surrounder : MonoBehaviour {
        public static float Radius = 1f;
        public Spot CurrentSpot {
            get {
                return _currentSpot;
            }
            set {
                if (value != _currentSpot) {
                    _SetSpot(value);
                }
            }
        }

        private Spot _currentSpot;
        private NavMeshAgent _agent;
        private int _maxAttempts = 5;
        private int _attempts;

        public delegate void SpotChangeDelegate (Spot oldSpot, Spot newSpot);
        public event SpotChangeDelegate OnSpotChange;

        public delegate void DumpedDelegate (Surroundings dumpedFrom);
        public event DumpedDelegate OnDumped;

        void Start () {
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update () {
            _attempts = 0;
            if (_currentSpot != null && _currentSpot.Occupier != this) {
                _SetSpot(null);
            }
        }

        private void _SetSpot (Spot newSpot) {
            Spot oldSpot = _currentSpot;
            _currentSpot = newSpot;

            if (OnSpotChange != null) {
                OnSpotChange(oldSpot, newSpot);
            }
        }

        public void AbandonSurrounding () {
            if (_currentSpot != null) {
                _currentSpot.GetAbandoned();
            }
        }

        public NavMeshPath CalculatePath (Vector3 destination) {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(destination, path);
            return path;
        }

        public void GetDumped (Surroundings s) {
            if (_attempts < _maxAttempts) {
                _attempts++;
                CurrentSpot = s.AttemptToSurround(this);
                GetComponent<TestIndicator>().Indicate(Color.green);
            } else {
                GetComponent<TestIndicator>().Indicate(Color.white);
                CurrentSpot = null;
            }

            if (CurrentSpot == null && OnDumped != null) {
                GetComponent<TestIndicator>().Indicate(Color.red);
                OnDumped(s);
            }
        }
    }
}
