using UnityEngine;
using System.Collections;

namespace Villager {
    public delegate void MovementDelegate (MovementDetector newPosition, Vector3 oldPosition);

    public class MovementDetector : MonoBehaviour {
        public event MovementDelegate OnMovement;
        public float TimeBetweenChecks = 0.5f;
        public float DistanceTolerance = 0.5f;

        private Vector3 _cachedPosition;

        void Start () {
            _cachedPosition = transform.position;
            StartCoroutine(_CheckForMovement());
        }

        private IEnumerator _CheckForMovement () {
            if ((_cachedPosition - transform.position).magnitude > DistanceTolerance) {
                if (OnMovement != null) OnMovement(this, _cachedPosition);
                _cachedPosition = transform.position;
            }
            yield return new WaitForSeconds(TimeBetweenChecks);
            StartCoroutine(_CheckForMovement());
        }
    }
}
