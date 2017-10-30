using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Villager {

    [System.Serializable]
    public class Spot {
        public Vector3 Position {
            get { return Owner.transform.position + RelativePosition; }
        }        
        public Vector3 RelativePosition;
        public Surroundings Owner;
        public Surrounder Occupier;

        public Spot (Surroundings owner, Vector3 position) {
            RelativePosition = position;
            Owner = owner;
        }

        public void SetOccupier (Surrounder newOccupier) {
            if (Occupier != null) {
                Occupier.GetDumped(Owner);
            }

            Occupier = newOccupier;
            Occupier.CurrentSpot = this;
        }

        public void GetAbandoned () {
            Occupier = null;
            Owner.SpotFreedHandler(this);
        }

        public float GetDistance (float clamp = Mathf.Infinity) {
            NavMeshPath path = new NavMeshPath();
            Occupier.GetComponent<NavMeshAgent>().CalculatePath(Position, path);
            return Util.ClampedDistance(path, clamp);
        }

        public bool ShouldOccupy (Surrounder newCommer) {
            if (Occupier == null) {
                return true;
            }

            float oldDistance = GetDistance();
            float newDistance = Util.ClampedDistance(newCommer.CalculatePath(Position), oldDistance);

            return newDistance < oldDistance;
        }
    }

}
