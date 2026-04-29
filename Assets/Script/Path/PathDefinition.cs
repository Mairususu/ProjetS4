using UnityEngine;

namespace TowerDefense
{
    public class PathDefinition : MonoBehaviour
    {
        public static PathDefinition Instance { get; private set; }

        [Header("Points du chemin (ordre : A → B)")]
        [SerializeField] private Transform[] waypoints;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public int WaypointCount => waypoints.Length;

        public Vector3 GetWaypoint(int index) =>
            waypoints[Mathf.Clamp(index, 0, waypoints.Length - 1)].position;

        public Vector3 StartPoint => waypoints[0].position;
        public Vector3 EndPoint   => waypoints[waypoints.Length - 1].position;


        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                // Sphères aux waypoints
                Gizmos.color = (i == 0)                    ? Color.green :
                               (i == waypoints.Length - 1) ? Color.red   : Color.yellow;
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                // Ligne entre waypoints
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}
