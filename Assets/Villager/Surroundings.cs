using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Villager {
    public class Surroundings : MonoBehaviour {
        public delegate void SpotFreedDelegate (Spot freed);
        public event SpotFreedDelegate OnSpotFreed;

        public Spot[] Spots;
        public float Radius = 2;
        
        void Start () {
            Spots = new Spot[(int) ((Mathf.PI * Radius) / Surrounder.Radius)];
            float gradesPerSpot = (Mathf.PI * 2) / Spots.Length;

            for (int i=0; i<Spots.Length; i++) {
                Spots[i] = new Spot(this, new Vector3(Mathf.Cos(gradesPerSpot * i) * Radius, 0,
                                                      Mathf.Sin(gradesPerSpot * i) * Radius));
            }
        }

        void OnDrawGizmos () {
            if (Application.isPlaying) {
                for (int i=0; i<Spots.Length; i++) {
                    if (Spots[i].Occupier == null)
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.green;

                    Gizmos.DrawSphere(Spots[i].Position, ((i+1) / (float)Spots.Length));
                }
            }
        }

        private int _AngleToSpot  (float angle) {
            return (int) Mathf.Round((((angle % (Mathf.PI * 2)) / (Mathf.PI * 2)) * Spots.Length) % Spots.Length);
        }

        public void SpotFreedHandler (Spot freed) {
            if (OnSpotFreed != null) {
                OnSpotFreed(freed);
            }
        }

        public Spot AttemptToSurround (Surrounder newCommer) {
            int availableSpot = FindAvailableSpot(newCommer);
            if (availableSpot >= 0) {
                Spots[availableSpot].SetOccupier(newCommer);
                return Spots[availableSpot];
            }

            return null;
        }

        public int FindAvailableSpot (Surrounder newCommer) {
            Vector3 localArrivalPos = GetArrivalPosition(newCommer) - transform.position;
            float angle = Util.FullAngle(new Vector2(localArrivalPos.x, localArrivalPos.z));
            int closest = _AngleToSpot(angle);
            int index = -1;

            for (int i=0; i<=Spots.Length/2; i++) {
                index = (closest + i) % Spots.Length;
                if (Spots[index].ShouldOccupy(newCommer)) return index;

                index = ((closest - i) + Spots.Length) % Spots.Length;
                if (Spots[index].ShouldOccupy(newCommer)) return index;
            }

            return -1;
        }

        public Vector3 GetArrivalPosition (Surrounder newCommer) {
            NavMeshPath path = new NavMeshPath();
            newCommer.GetComponent<NavMeshAgent>().CalculatePath(transform.position, path);

            if (path.corners.Length >= 2) {
                return path.corners[path.corners.Length - 2];
            } else {
                return newCommer.transform.position;
            }
        }
    }
}
