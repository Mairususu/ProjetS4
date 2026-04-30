using UnityEngine;
using UnityEngine.AI;

namespace TowerDefense
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAnimator))]
    public class PathFollower : MonoBehaviour
    {
        public static readonly System.Collections.Generic.HashSet<PathFollower> ActiveFollowers = new();

        [SerializeField] private float arrivalTolerance = 0.5f;
        [SerializeField] private float rotationSpeed    = 10f;

        [Header("Fix rotation")]
        [SerializeField] private float     modelRotationOffsetY = 180f;
        [SerializeField] private Transform modelRoot;

        public float Progress  { get; private set; }
        public bool  IsArrived { get; private set; }

        private NavMeshAgent  agent;
        private EnemyAnimator anim;
        private int           currentWaypointIndex;
        private bool          initialized;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim  = GetComponent<EnemyAnimator>();

            agent.updateRotation = false;
            agent.updateUpAxis   = false;

            if (modelRoot != null)
                modelRoot.localRotation = Quaternion.Euler(0f, modelRotationOffsetY, 0f);
        }

        private void OnEnable()  => ActiveFollowers.Add(this);
        private void OnDisable() => ActiveFollowers.Remove(this);
        private void OnDestroy() => ActiveFollowers.Remove(this);

        public void Initialize(float speed)
        {
            if (PathDefinition.Instance == null)
            {
                Debug.LogError("[PathFollower] PathDefinition introuvable !");
                return;
            }

            IsArrived            = false;
            initialized          = false;
            currentWaypointIndex = 0;

            agent.speed            = speed;
            agent.stoppingDistance = arrivalTolerance;

            bool warped = agent.Warp(PathDefinition.Instance.StartPoint);
            if (!warped)
            {
                Debug.LogWarning($"[PathFollower] {gameObject.name} hors NavMesh.");
                return;
            }

            initialized = true;
            anim?.SetWalking(true);

            currentWaypointIndex = 1;
            GoToCurrentWaypoint();
        }

        private void Update()
        {
            if (!initialized || IsArrived) return;

            UpdateProgress();
            UpdateRotation();
            CheckArrival();
        }

        private void GoToCurrentWaypoint()
        {
            if (currentWaypointIndex >= PathDefinition.Instance.WaypointCount)
            {
                OnReachedEnd();
                return;
            }

            Vector3 target = PathDefinition.Instance.GetWaypoint(currentWaypointIndex);
            agent.SetDestination(target);
        }

        private void CheckArrival()
        {
            if (agent.pathPending) return;

            if (agent.remainingDistance > arrivalTolerance) return;

            if (currentWaypointIndex >= PathDefinition.Instance.WaypointCount - 1)
            {
                OnReachedEnd();
                return;
            }

            currentWaypointIndex++;
            GoToCurrentWaypoint();
        }

        private void UpdateRotation()
        {
            if (agent.velocity.sqrMagnitude < 0.01f) return;

            Vector3    dir       = new Vector3(agent.velocity.x, 0f, agent.velocity.z).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation   = Quaternion.Lerp(
                transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        private void UpdateProgress()
        {
            if (PathDefinition.Instance.WaypointCount <= 1) { Progress = 1f; return; }
            Progress = Mathf.Clamp01(
                (float)currentWaypointIndex / (PathDefinition.Instance.WaypointCount - 1));
        }

        private void OnReachedEnd()
        {
            IsArrived       = true;
            agent.isStopped = true;
            anim?.SetWalking(false);

            Collider col = GetComponentInChildren<Collider>();
            if (col != null) col.enabled = false;

            EnemyData data = GetComponent<EnemyHealth>()?.Data;
            if (data != null) GameEvents.RaiseEnemyReachedEnd(data);

            Destroy(gameObject, 0.5f);
        }

        private void OnDrawGizmos()
        {
            if (!initialized || agent == null || !agent.hasPath) return;

            Gizmos.color = Color.red;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
                Gizmos.DrawLine(corners[i], corners[i + 1]);
        }
    }
}