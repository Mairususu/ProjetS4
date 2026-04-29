using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class PathFollower : MonoBehaviour
    {
        // Liste statique permettant aux tours de trouver les ennemis actifs
        public static readonly HashSet<PathFollower> ActiveFollowers = new();

        [Tooltip("Distance en dessous de laquelle on passe au waypoint suivant")]
        [SerializeField] private float waypointTolerance = 0.15f;

        private int   currentWaypointIndex;
        private float moveSpeed;
        public float Progress { get; private set; }


        private void OnEnable()
        {
            ActiveFollowers.Add(this);
            currentWaypointIndex = 0;
        }

        private void OnDisable() => ActiveFollowers.Remove(this);

        private void OnDestroy() => ActiveFollowers.Remove(this);

        public void Initialize(float speed)
        {
            moveSpeed = speed;
            transform.position = PathDefinition.Instance.StartPoint;
            currentWaypointIndex = 0;
        }

        private void Update()
        {
            if (PathDefinition.Instance == null) return;

            MoveAlongPath();
            UpdateProgress();
        }

        private void MoveAlongPath()
        {
            if (currentWaypointIndex >= PathDefinition.Instance.WaypointCount) return;

            Vector3 target = PathDefinition.Instance.GetWaypoint(currentWaypointIndex);
            Vector3 dir    = (target - transform.position).normalized;

            transform.position += dir * (moveSpeed * Time.deltaTime);
            if (dir != Vector3.zero)
                transform.forward = new Vector3(dir.x, 0f, dir.z);

            if (Vector3.Distance(transform.position, target) <= waypointTolerance)
            {
                currentWaypointIndex++;

                // Arrivée au point B
                if (currentWaypointIndex >= PathDefinition.Instance.WaypointCount)
                    OnReachedEnd();
            }
        }

        private void UpdateProgress()
        {
            int total = PathDefinition.Instance.WaypointCount;
            if (total <= 1) { Progress = 1f; return; }
            Progress = (float)currentWaypointIndex / (total - 1);
        }

        private void OnReachedEnd()
        {
            EnemyData data = GetComponent<EnemyHealth>()?.Data;
            if (data != null) GameEvents.RaiseEnemyReachedEnd(data);
            Destroy(gameObject);
        }
    }
}
